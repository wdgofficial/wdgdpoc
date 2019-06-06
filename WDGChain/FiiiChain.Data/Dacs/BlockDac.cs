// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.Entities;
using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Text;
using FiiiChain.Framework;
using System.Linq;
using System.Data;
using FiiiChain.Entities.ExtensionModels;

namespace FiiiChain.Data
{
    public class BlockDac : DataAccessComponent<BlockDac>
    {
        public virtual int Save(Block block)
        {
            int rows = 0;
            StringBuilder sql = new StringBuilder("BEGIN TRANSACTION;");

            sql.Append("INSERT INTO Blocks " +
                "(Hash, Version, Height, PreviousBlockHash, Bits, Nonce, Timestamp, NextBlockHash, TotalAmount, TotalFee, GeneratorId, BlockSignature, PayloadHash, IsDiscarded, IsVerified) " +
                $"VALUES ('{block.Hash}', {block.Version}, {block.Height}, '{block.PreviousBlockHash}', {block.Bits}, {block.Nonce}, {block.Timestamp}, null, {block.TotalAmount}, {block.TotalFee}, '{block.GeneratorId}', '{block.BlockSignature}', '{block.PayloadHash}', {Convert.ToInt32(block.IsDiscarded)}, {Convert.ToInt32(block.IsVerified)});");

            foreach (var transaction in block.Transactions)
            {
                sql.Append("INSERT INTO Transactions " +
                "(Hash, BlockHash, Version, Timestamp, LockTime, TotalInput, TotalOutput, Fee, Size) " +
                $"VALUES ('{transaction.Hash}', '{transaction.BlockHash}', {transaction.Version}, {transaction.Timestamp}, {transaction.LockTime}, {transaction.TotalInput}, {transaction.TotalOutput}, {transaction.Fee}, {transaction.Size});");

                foreach (var input in transaction.Inputs)
                {
                    sql.Append("INSERT INTO InputList " +
                    "(TransactionHash, OutputTransactionHash, OutputIndex, Size, Amount, UnlockScript, AccountId, BlockHash) " +
                    $"VALUES ('{input.TransactionHash}', '{input.OutputTransactionHash}', {input.OutputIndex}, {input.Size}, {input.Amount}, '{input.UnlockScript}', '{Convert.ToString(input.AccountId)}', '{block.Hash}');");

                    if (input.OutputTransactionHash != Base16.Encode(HashHelper.EmptyHash()))
                    {
                        sql.Append($"UPDATE OutputList SET Spent = 1 WHERE TransactionHash = '{input.OutputTransactionHash}' AND [Index] = {input.OutputIndex};");
                    }
                }

                foreach (var output in transaction.Outputs)
                {
                    sql.Append("INSERT INTO OutputList " +
                    "([Index], TransactionHash, ReceiverId, Amount, Size, LockScript, Spent, BlockHash) " +
                    $"VALUES ({output.Index}, '{output.TransactionHash}', '{output.ReceiverId}', {output.Amount}, {output.Size}, '{output.LockScript}', {Convert.ToInt32(output.Spent)}, '{block.Hash}'); ");
                }
            }

            sql.Append($"UPDATE Blocks SET NextBlockHash = '{block.Hash}' WHERE Hash = '{block.PreviousBlockHash}';");
            sql.Append("END TRANSACTION;");
            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(sql.ToString(), con))
            {

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                rows = cmd.ExecuteNonQuery();
            }
            return rows;
        }

        internal virtual void UpdateNextBlockHash(string currentHash, string nextHash)
        {
            const string SQL_STATEMENT =
                "UPDATE Blocks " +
                "SET NextBlockHash = @NextBlockHash " +
                "WHERE Hash = @Hash ";

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Hash", currentHash);
                cmd.Parameters.AddWithValue("@NextBlockHash", nextHash);
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                cmd.ExecuteNonQuery();
            }
        }

        internal virtual void UpdateBlockStatusToDiscarded(string hash)
        {
            const string TX_SELECT_SQL_STATEMENT =
                "SELECT Hash " +
                "FROM Transactions " +
                "WHERE BlockHash = @BlockHash " +
                "AND IsDiscarded = 0 ";

            const string BLOCK_UPDATE_SQL_STATEMENT =
                "UPDATE Blocks " +
                "SET IsDiscarded = 1 " +
                "WHERE Hash = @Hash ";

            const string TX_UPDATE_SQL_STATEMENT =
                "UPDATE Transactions " +
                "SET IsDiscarded = 1 " +
                "WHERE BlockHash = @BlockHash ";

            const string INPUT_UPDATE_SQL_STATEMENT =
                "UPDATE InputList " +
                "SET IsDiscarded = 1 " +
                "WHERE TransactionHash = @TransactionHash AND BlockHash = @BlockHash";

            const string OUTPUT_UPDATE_SQL_STATEMENT =
                "UPDATE OutputList " +
                "SET IsDiscarded = 1 " +
                "WHERE TransactionHash = @TransactionHash AND BlockHash = @BlockHash ";



            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            {
                con.Open();
                //con.SynchronousNORMAL();
                var txHashList = new List<string>();

                using (SqliteCommand cmd = new SqliteCommand(TX_SELECT_SQL_STATEMENT, con))
                {
                    cmd.Parameters.AddWithValue("@BlockHash", hash);

                    cmd.Connection.Open();

                    using (SqliteDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            txHashList.Add(GetDataValue<string>(dr, "Hash"));
                        }
                    }
                }

                using (var scope = new System.Transactions.TransactionScope())
                {
                    using (SqliteCommand cmd = new SqliteCommand(BLOCK_UPDATE_SQL_STATEMENT, con))
                    {
                        cmd.Parameters.AddWithValue("@Hash", hash);
                        cmd.ExecuteNonQuery();
                    }

                    using (SqliteCommand cmd = new SqliteCommand(TX_UPDATE_SQL_STATEMENT, con))
                    {
                        cmd.Parameters.AddWithValue("@BlockHash", hash);
                        cmd.ExecuteNonQuery();
                    }

                    foreach(var txHash in txHashList)
                    {
                        using (SqliteCommand cmd = new SqliteCommand(INPUT_UPDATE_SQL_STATEMENT, con))
                        {
                            cmd.Parameters.AddWithValue("@TransactionHash", txHash);
                            cmd.Parameters.AddWithValue("@BlockHash", hash);
                            cmd.ExecuteNonQuery();
                        }

                        using (SqliteCommand cmd = new SqliteCommand(OUTPUT_UPDATE_SQL_STATEMENT, con))
                        {
                            cmd.Parameters.AddWithValue("@TransactionHash", txHash);
                            cmd.Parameters.AddWithValue("@BlockHash", hash);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    scope.Complete();
                }
            }
        }

        internal virtual void UpdateBlockStatusToConfirmed(string hash)
        {
            const string SQL_STATEMENT =
                "UPDATE Blocks " +
                "SET IsVerified = 1 " +
                "WHERE Hash = @Hash ";

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Hash", hash);
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                cmd.ExecuteNonQuery();
            }
        }

        public virtual Dictionary<long, Block> Select()
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM Blocks " +
                "WHERE IsDiscarded = 0";

            Dictionary<long, Block> result = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    result = new Dictionary<long, Block>();

                    while (dr.Read())
                    {
                        Block block = new Block();

                        block.Id = GetDataValue<long>(dr, "Id");
                        block.Hash = GetDataValue<string>(dr, "Hash");
                        block.Version = GetDataValue<int>(dr, "Version");
                        block.Height = GetDataValue<long>(dr, "Height");
                        block.PreviousBlockHash = GetDataValue<string>(dr, "PreviousBlockHash");
                        block.Bits = GetDataValue<long>(dr, "Bits");
                        block.Nonce = GetDataValue<long>(dr, "Nonce");
                        block.Timestamp = GetDataValue<long>(dr, "Timestamp");
                        block.NextBlockHash = GetDataValue<string>(dr, "NextBlockHash");
                        block.TotalAmount = GetDataValue<long>(dr, "TotalAmount");
                        block.TotalFee = GetDataValue<long>(dr, "TotalFee");
                        block.GeneratorId = GetDataValue<string>(dr, "GeneratorId");
                        block.BlockSignature = GetDataValue<string>(dr, "BlockSignature");
                        block.PayloadHash = GetDataValue<string>(dr, "PayloadHash");

                        block.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");
                        block.IsVerified = GetDataValue<bool>(dr, "IsVerified");


                        result.Add(block.Id, block);
                    }
                }
            }

            return result;
        }

        public virtual Dictionary<long, Block> SelectByHeightRange(long from, long to)
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM Blocks " +
                "WHERE Id BETWEEN @FromHeight AND @ToHeight " +
                "AND IsDiscarded = 0 ";

            Dictionary<long, Block> result = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@FromHeight", from);
                cmd.Parameters.AddWithValue("@ToHeight", to);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    result = new Dictionary<long, Block>();

                    while (dr.Read())
                    {
                        Block block = new Block();

                        block.Id = GetDataValue<long>(dr, "Id");
                        block.Hash = GetDataValue<string>(dr, "Hash");
                        block.Version = GetDataValue<int>(dr, "Version");
                        block.Height = GetDataValue<long>(dr, "Height");
                        block.PreviousBlockHash = GetDataValue<string>(dr, "PreviousBlockHash");
                        block.Bits = GetDataValue<long>(dr, "Bits");
                        block.Nonce = GetDataValue<long>(dr, "Nonce");
                        block.Timestamp = GetDataValue<long>(dr, "Timestamp");
                        block.NextBlockHash = GetDataValue<string>(dr, "NextBlockHash");
                        block.TotalAmount = GetDataValue<long>(dr, "TotalAmount");
                        block.TotalFee = GetDataValue<long>(dr, "TotalFee");
                        block.GeneratorId = GetDataValue<string>(dr, "GeneratorId");
                        block.BlockSignature = GetDataValue<string>(dr, "BlockSignature");
                        block.PayloadHash = GetDataValue<string>(dr, "PayloadHash");

                        block.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");
                        block.IsVerified = GetDataValue<bool>(dr, "IsVerified");

                        result.Add(block.Id, block);
                    }
                }
            }

            return result;
        }

        public virtual bool HasBlock(long height)
        {
            const string SQL_STATEMENT =
                "SELECT 1 " +
                "FROM Blocks " +
                "WHERE IsDiscarded = 0 " +
                "AND Height = @Height LIMIT 1 ";

            bool hasBlock = false;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Height", height);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    hasBlock = dr.HasRows;
                }
            }

            return hasBlock;
        }

        public virtual Block SelectLast()
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM Blocks " +
                "WHERE IsDiscarded = 0 " +
                "ORDER BY Height DESC,Timestamp DESC LIMIT 1 ";

            Block block = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        block = new Block();

                        block.Id = GetDataValue<long>(dr, "Id");
                        block.Hash = GetDataValue<string>(dr, "Hash");
                        block.Version = GetDataValue<int>(dr, "Version");
                        block.Height = GetDataValue<long>(dr, "Height");
                        block.PreviousBlockHash = GetDataValue<string>(dr, "PreviousBlockHash");
                        block.Bits = GetDataValue<long>(dr, "Bits");
                        block.Nonce = GetDataValue<long>(dr, "Nonce");
                        block.Timestamp = GetDataValue<long>(dr, "Timestamp");
                        block.NextBlockHash = GetDataValue<string>(dr, "NextBlockHash");
                        block.TotalAmount = GetDataValue<long>(dr, "TotalAmount");
                        block.TotalFee = GetDataValue<long>(dr, "TotalFee");
                        block.GeneratorId = GetDataValue<string>(dr, "GeneratorId");
                        block.BlockSignature = GetDataValue<string>(dr, "BlockSignature");
                        block.PayloadHash = GetDataValue<string>(dr, "PayloadHash");

                        block.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");
                        block.IsVerified = GetDataValue<bool>(dr, "IsVerified");
                    }
                }
            }

            return block;
        }

        public virtual Block SelectLastConfirmed()
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM Blocks " +
                "WHERE IsDiscarded = 0 AND IsVerified = 1 " +
                "ORDER BY Height DESC,Timestamp DESC LIMIT 1 ";

            Block block = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        block = new Block();

                        block.Id = GetDataValue<long>(dr, "Id");
                        block.Hash = GetDataValue<string>(dr, "Hash");
                        block.Version = GetDataValue<int>(dr, "Version");
                        block.Height = GetDataValue<long>(dr, "Height");
                        block.PreviousBlockHash = GetDataValue<string>(dr, "PreviousBlockHash");
                        block.Bits = GetDataValue<long>(dr, "Bits");
                        block.Nonce = GetDataValue<long>(dr, "Nonce");
                        block.Timestamp = GetDataValue<long>(dr, "Timestamp");
                        block.NextBlockHash = GetDataValue<string>(dr, "NextBlockHash");
                        block.TotalAmount = GetDataValue<long>(dr, "TotalAmount");
                        block.TotalFee = GetDataValue<long>(dr, "TotalFee");
                        block.GeneratorId = GetDataValue<string>(dr, "GeneratorId");
                        block.BlockSignature = GetDataValue<string>(dr, "BlockSignature");
                        block.PayloadHash = GetDataValue<string>(dr, "PayloadHash");

                        block.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");
                        block.IsVerified = GetDataValue<bool>(dr, "IsVerified");
                    }
                }
            }

            return block;
        }

        public virtual Block SelectById(long id)
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM Blocks " +
                "WHERE Id = @Id " +
                "AND IsDiscarded = 0 ";

            Block block = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Id", id);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        block = new Block();

                        block.Id = GetDataValue<long>(dr, "Id");
                        block.Hash = GetDataValue<string>(dr, "Hash");
                        block.Version = GetDataValue<int>(dr, "Version");
                        block.Height = GetDataValue<long>(dr, "Height");
                        block.PreviousBlockHash = GetDataValue<string>(dr, "PreviousBlockHash");
                        block.Bits = GetDataValue<long>(dr, "Bits");
                        block.Nonce = GetDataValue<long>(dr, "Nonce");
                        block.Timestamp = GetDataValue<long>(dr, "Timestamp");
                        block.NextBlockHash = GetDataValue<string>(dr, "NextBlockHash");
                        block.TotalAmount = GetDataValue<long>(dr, "TotalAmount");
                        block.TotalFee = GetDataValue<long>(dr, "TotalFee");
                        block.GeneratorId = GetDataValue<string>(dr, "GeneratorId");
                        block.BlockSignature = GetDataValue<string>(dr, "BlockSignature");
                        block.PayloadHash = GetDataValue<string>(dr, "PayloadHash");

                        block.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");
                        block.IsVerified = GetDataValue<bool>(dr, "IsVerified");
                    }
                }
            }

            return block;
        }

        public virtual List<Block> SelectByHeight(long height)
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM Blocks " +
                "WHERE Height = @Height " +
                "AND IsDiscarded = 0 ";

            var result = new List<Block>();

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Height", height);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var block = new Block();
                        block.Id = GetDataValue<long>(dr, "Id");
                        block.Hash = GetDataValue<string>(dr, "Hash");
                        block.Version = GetDataValue<int>(dr, "Version");
                        block.Height = GetDataValue<long>(dr, "Height");
                        block.PreviousBlockHash = GetDataValue<string>(dr, "PreviousBlockHash");
                        block.Bits = GetDataValue<long>(dr, "Bits");
                        block.Nonce = GetDataValue<long>(dr, "Nonce");
                        block.Timestamp = GetDataValue<long>(dr, "Timestamp");
                        block.NextBlockHash = GetDataValue<string>(dr, "NextBlockHash");
                        block.TotalAmount = GetDataValue<long>(dr, "TotalAmount");
                        block.TotalFee = GetDataValue<long>(dr, "TotalFee");
                        block.GeneratorId = GetDataValue<string>(dr, "GeneratorId");
                        block.BlockSignature = GetDataValue<string>(dr, "BlockSignature");
                        block.PayloadHash = GetDataValue<string>(dr, "PayloadHash");

                        block.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");
                        block.IsVerified = GetDataValue<bool>(dr, "IsVerified");

                        result.Add(block);
                    }
                }
            }

            return result;
        }

        public virtual List<Block> SelectByHeights(IEnumerable<long> height)
        {
            var match = $" ({string.Join(",", height)}) ";

            string SQL_STATEMENT =
                "SELECT * " +
                "FROM Blocks " +
                "WHERE Height in " + match +
                " AND IsDiscarded = 0 ";

            var result = new List<Block>();
            if (height == null || !height.Any())
            {
                return result;
            }
            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var block = new Block();
                        block.Id = GetDataValue<long>(dr, "Id");
                        block.Hash = GetDataValue<string>(dr, "Hash");
                        block.Version = GetDataValue<int>(dr, "Version");
                        block.Height = GetDataValue<long>(dr, "Height");
                        block.PreviousBlockHash = GetDataValue<string>(dr, "PreviousBlockHash");
                        block.Bits = GetDataValue<long>(dr, "Bits");
                        block.Nonce = GetDataValue<long>(dr, "Nonce");
                        block.Timestamp = GetDataValue<long>(dr, "Timestamp");
                        block.NextBlockHash = GetDataValue<string>(dr, "NextBlockHash");
                        block.TotalAmount = GetDataValue<long>(dr, "TotalAmount");
                        block.TotalFee = GetDataValue<long>(dr, "TotalFee");
                        block.GeneratorId = GetDataValue<string>(dr, "GeneratorId");
                        block.BlockSignature = GetDataValue<string>(dr, "BlockSignature");
                        block.PayloadHash = GetDataValue<string>(dr, "PayloadHash");

                        block.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");
                        block.IsVerified = GetDataValue<bool>(dr, "IsVerified");

                        result.Add(block);
                    }
                }
            }

            return result;
        }

        public virtual Block SelectByHash(string hash)
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM Blocks " +
                "WHERE Hash = @Hash " +
                "AND IsDiscarded = 0 ";

            Block block = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Hash", hash);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        block = new Block();

                        block.Id = GetDataValue<long>(dr, "Id");
                        block.Hash = GetDataValue<string>(dr, "Hash");
                        block.Version = GetDataValue<int>(dr, "Version");
                        block.Height = GetDataValue<long>(dr, "Height");
                        block.PreviousBlockHash = GetDataValue<string>(dr, "PreviousBlockHash");
                        block.Bits = GetDataValue<long>(dr, "Bits");
                        block.Nonce = GetDataValue<long>(dr, "Nonce");
                        block.Timestamp = GetDataValue<long>(dr, "Timestamp");
                        block.NextBlockHash = GetDataValue<string>(dr, "NextBlockHash");
                        block.TotalAmount = GetDataValue<long>(dr, "TotalAmount");
                        block.TotalFee = GetDataValue<long>(dr, "TotalFee");
                        block.GeneratorId = GetDataValue<string>(dr, "GeneratorId");
                        block.BlockSignature = GetDataValue<string>(dr, "BlockSignature");
                        block.PayloadHash = GetDataValue<string>(dr, "PayloadHash");

                        block.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");
                        block.IsVerified = GetDataValue<bool>(dr, "IsVerified");
                    }
                }
            }

            return block;
        }

        public virtual bool BlockHashExist(string hash)
        {
            const string SQL_STATEMENT =
                "SELECT Hash " +
                "FROM Blocks " +
                "WHERE Hash = @Hash " +
                "AND IsDiscarded = 0 Limit 1";

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Hash", hash);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    return dr.HasRows;
                }
            }
        }

        public virtual List<Block> SelectByPreviousHash(string prevHash)
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM Blocks " +
                "WHERE PreviousBlockHash = @PreviousBlockHash " +
                "AND IsDiscarded = 0 ";

            var result = new List<Block>();

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@PreviousBlockHash", prevHash);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var block = new Block();

                        block.Id = GetDataValue<long>(dr, "Id");
                        block.Hash = GetDataValue<string>(dr, "Hash");
                        block.Version = GetDataValue<int>(dr, "Version");
                        block.Height = GetDataValue<long>(dr, "Height");
                        block.PreviousBlockHash = GetDataValue<string>(dr, "PreviousBlockHash");
                        block.Bits = GetDataValue<long>(dr, "Bits");
                        block.Nonce = GetDataValue<long>(dr, "Nonce");
                        block.Timestamp = GetDataValue<long>(dr, "Timestamp");
                        block.NextBlockHash = GetDataValue<string>(dr, "NextBlockHash");
                        block.TotalAmount = GetDataValue<long>(dr, "TotalAmount");
                        block.TotalFee = GetDataValue<long>(dr, "TotalFee");
                        block.GeneratorId = GetDataValue<string>(dr, "GeneratorId");
                        block.BlockSignature = GetDataValue<string>(dr, "BlockSignature");
                        block.PayloadHash = GetDataValue<string>(dr, "PayloadHash");

                        block.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");
                        block.IsVerified = GetDataValue<bool>(dr, "IsVerified");

                        result.Add(block);
                    }
                }
            }

            return result;
        }

        public virtual List<Block> SelectTipBlocks()
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM Blocks " +
                "WHERE NextBlockHash IS NULL AND IsDiscarded = 0";

            var result = new List<Block>();

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var block = new Block();

                        block.Id = GetDataValue<long>(dr, "Id");
                        block.Hash = GetDataValue<string>(dr, "Hash");
                        block.Version = GetDataValue<int>(dr, "Version");
                        block.Height = GetDataValue<long>(dr, "Height");
                        block.PreviousBlockHash = GetDataValue<string>(dr, "PreviousBlockHash");
                        block.Bits = GetDataValue<long>(dr, "Bits");
                        block.Nonce = GetDataValue<long>(dr, "Nonce");
                        block.Timestamp = GetDataValue<long>(dr, "Timestamp");
                        block.NextBlockHash = GetDataValue<string>(dr, "NextBlockHash");
                        block.TotalAmount = GetDataValue<long>(dr, "TotalAmount");
                        block.TotalFee = GetDataValue<long>(dr, "TotalFee");
                        block.GeneratorId = GetDataValue<string>(dr, "GeneratorId");
                        block.BlockSignature = GetDataValue<string>(dr, "BlockSignature");
                        block.PayloadHash = GetDataValue<string>(dr, "PayloadHash");

                        block.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");
                        block.IsVerified = GetDataValue<bool>(dr, "IsVerified");

                        result.Add(block);
                    }
                }
            }

            return result;
        }

        public virtual long CountBranchLength(string blockHash)
        {
            const string SQL_STATEMENT =
                "WITH RECURSIVE down(Hash, PreviousBlockHash, NextBlockHash) AS " +
                "( " +
                "   SELECT Hash, PreviousBlockHash, NextBlockHash " +
                "   FROM Blocks " +
                "   WHERE Hash = @Hash " +
                "   UNION " +
                "   SELECT a.Hash, a.PreviousBlockHash, a.NextBlockHash " +
                "   FROM Blocks a, down b " +
                "   WHERE b.Hash = a.NextBlockHash AND " +
                "   (SELECT COUNT(0) FROM Blocks WHERE PreviousBlockHash = b.Hash) <= 1 " +
                ") SELECT count(0) FROM down WHERE Hash != @Hash ";

            long result = 0;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Hash", blockHash);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                result = Convert.ToInt64(cmd.ExecuteScalar());
            }

            return result;
        }

        public virtual long CountOfDescendants(string hash)
        {
            const string SQL_STATEMENT =
                "WITH RECURSIVE down(Hash, PreviousBlockHash, NextBlockHash) AS " +
                "( " +
                "   SELECT Hash, PreviousBlockHash, NextBlockHash " +
                "   FROM Blocks " +
                "   WHERE Hash = @Hash " +
                "   UNION " +
                "   SELECT a.Hash, a.PreviousBlockHash, a.NextBlockHash " +
                "   FROM Blocks a, down b " +
                "   WHERE b.Hash = a.PreviousBlockHash " +
                ") SELECT COUNT(0) - 1 FROM down ;";

            long result = 0;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Hash", hash);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                result = Convert.ToInt64(cmd.ExecuteScalar());
            }

            return result;
        }

        public virtual List<string> SelectHashesOfDescendants(string hash)
        {
            const string SQL_STATEMENT =
            "WITH RECURSIVE down(Hash) AS " +
            "( " +
            "   SELECT Hash " +
            "   FROM Blocks " +
            "   WHERE Hash = @Hash " +
            "   UNION " +
            "   SELECT a.Hash " +
            "   FROM Blocks a, down b " +
            "   WHERE b.Hash = a.PreviousBlockHash " +
            ") SELECT Hash FROM down WHERE Hash != @Hash ";

            var result = new List<string>();

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Hash", hash);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        result.Add(GetDataValue<string>(dr, "Hash"));
                    }
                }
            }

            return result;
        }

        public virtual List<Block> SelectPreviousBlocks(long lastHeight, int blockCount)
        {
            const string SQL_STATEMENT =
            "WITH RECURSIVE down(Id, Hash, Version, Height, PreviousBlockHash, Bits, Nonce, Timestamp, NextBlockHash, TotalAmount, TotalFee, GeneratorId, BlockSignature, PayloadHash, IsDiscarded, IsVerified, num) AS " +
            "( " +
            "   SELECT Id, Hash, Version, Height, PreviousBlockHash, Bits, Nonce, Timestamp, NextBlockHash, TotalAmount, TotalFee, GeneratorId, BlockSignature, PayloadHash, IsDiscarded, IsVerified, 1 as num " +
            "   FROM Blocks " +
            "   WHERE Height = @LastHeight " +
            "   UNION " +
            "   SELECT a.Id, a.Hash, a.Version, a.Height, a.PreviousBlockHash, a.Bits, a.Nonce, a.Timestamp, a.NextBlockHash, a.TotalAmount, a.TotalFee, a.GeneratorId, a.BlockSignature, a.PayloadHash, a.IsDiscarded, a.IsVerified, b.num + 1 as num " +
            "   FROM Blocks a, down b " +
            "   WHERE b.PreviousBlockHash = a.Hash and b.num < @BlockCount " +
            ") SELECT Id, Hash, Version, Height, PreviousBlockHash, Bits, Nonce, Timestamp, NextBlockHash, TotalAmount, TotalFee, GeneratorId, BlockSignature, PayloadHash, IsDiscarded, IsVerified FROM down; ";

            var result = new List<Block>();

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@LastHeight", lastHeight);
                cmd.Parameters.AddWithValue("@BlockCount", blockCount);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var block = new Block();

                        block.Id = GetDataValue<long>(dr, "Id");
                        block.Hash = GetDataValue<string>(dr, "Hash");
                        block.Version = GetDataValue<int>(dr, "Version");
                        block.Height = GetDataValue<long>(dr, "Height");
                        block.PreviousBlockHash = GetDataValue<string>(dr, "PreviousBlockHash");
                        block.Bits = GetDataValue<long>(dr, "Bits");
                        block.Nonce = GetDataValue<long>(dr, "Nonce");
                        block.Timestamp = GetDataValue<long>(dr, "Timestamp");
                        block.NextBlockHash = GetDataValue<string>(dr, "NextBlockHash");
                        block.TotalAmount = GetDataValue<long>(dr, "TotalAmount");
                        block.TotalFee = GetDataValue<long>(dr, "TotalFee");
                        block.GeneratorId = GetDataValue<string>(dr, "GeneratorId");
                        block.BlockSignature = GetDataValue<string>(dr, "BlockSignature");
                        block.PayloadHash = GetDataValue<string>(dr, "PayloadHash");

                        block.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");
                        block.IsVerified = GetDataValue<bool>(dr, "IsVerified");

                        result.Add(block);
                    }
                }
            }

            return result;
        }
        public virtual List<Block> SelectPreviousBlocks(string blockHash, int blockCount)
        {
            const string SQL_STATEMENT =
            "WITH RECURSIVE down(Id, Hash, Version, Height, PreviousBlockHash, Bits, Nonce, Timestamp, NextBlockHash, TotalAmount, TotalFee, GeneratorId, BlockSignature, PayloadHash, IsDiscarded, IsVerified, num) AS " +
            "( " +
            "   SELECT Id, Hash, Version, Height, PreviousBlockHash, Bits, Nonce, Timestamp, NextBlockHash, TotalAmount, TotalFee, GeneratorId, BlockSignature, PayloadHash, IsDiscarded, IsVerified, 1 as num " +
            "   FROM Blocks " +
            "   WHERE Hash = @BlockHash " +
            "   UNION " +
            "   SELECT a.Id, a.Hash, a.Version, a.Height, a.PreviousBlockHash, a.Bits, a.Nonce, a.Timestamp, a.NextBlockHash, a.TotalAmount, a.TotalFee, a.GeneratorId, a.BlockSignature, a.PayloadHash, a.IsDiscarded, a.IsVerified, b.num + 1 as num " +
            "   FROM Blocks a, down b " +
            "   WHERE b.PreviousBlockHash = a.Hash and b.num < @BlockCount " +
            ") SELECT Id, Hash, Version, Height, PreviousBlockHash, Bits, Nonce, Timestamp, NextBlockHash, TotalAmount, TotalFee, GeneratorId, BlockSignature, PayloadHash, IsDiscarded, IsVerified FROM down; ";

            var result = new List<Block>();

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@BlockHash", blockHash);
                cmd.Parameters.AddWithValue("@BlockCount", blockCount);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var block = new Block();

                        block.Id = GetDataValue<long>(dr, "Id");
                        block.Hash = GetDataValue<string>(dr, "Hash");
                        block.Version = GetDataValue<int>(dr, "Version");
                        block.Height = GetDataValue<long>(dr, "Height");
                        block.PreviousBlockHash = GetDataValue<string>(dr, "PreviousBlockHash");
                        block.Bits = GetDataValue<long>(dr, "Bits");
                        block.Nonce = GetDataValue<long>(dr, "Nonce");
                        block.Timestamp = GetDataValue<long>(dr, "Timestamp");
                        block.NextBlockHash = GetDataValue<string>(dr, "NextBlockHash");
                        block.TotalAmount = GetDataValue<long>(dr, "TotalAmount");
                        block.TotalFee = GetDataValue<long>(dr, "TotalFee");
                        block.GeneratorId = GetDataValue<string>(dr, "GeneratorId");
                        block.BlockSignature = GetDataValue<string>(dr, "BlockSignature");
                        block.PayloadHash = GetDataValue<string>(dr, "PayloadHash");

                        block.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");
                        block.IsVerified = GetDataValue<bool>(dr, "IsVerified");

                        result.Add(block);
                    }
                }
            }

            return result;
        }

        public virtual long SelectIdByHeight(long height)
        {
            const string SQL_STATEMENT =
                "SELECT Id " +
                "FROM Blocks " +
                "WHERE Height = @Height " +
                "AND IsDiscarded = 0 ";

            long blockId = 0;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Height", height);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                blockId = (long)cmd.ExecuteScalar();
            }

            return blockId;
        }

        public virtual List<long> SelectIdByLimit(long blockId, int limit)
        {
            const string SQL_STATEMENT =
                "SELECT Id " +
                "FROM Blocks " +
                "WHERE Id > @Id " +
                "AND IsDiscarded = 0 " +
                "LIMIT @Id, @Limit";

            List<long> result = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Id", blockId);
                cmd.Parameters.AddWithValue("@Limit", limit);
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    result = new List<long>();

                    while (dr.Read())
                    {
                        Block block = new Block();

                        long id = GetDataValue<long>(dr, "Id");

                        result.Add(id);
                    }
                }
            }

            return result;
        }

        public virtual Dictionary<long, Block> SelectByLimit(long blockId, int limit)
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM Blocks " +
                "WHERE Id > @Id " +
                "AND IsDiscarded = 0 " +
                "LIMIT @Id, @Limit ";

            Dictionary<long, Block> result = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Id", blockId);
                cmd.Parameters.AddWithValue("@Limit", limit);
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    result = new Dictionary<long, Block>();

                    while (dr.Read())
                    {
                        Block block = new Block();

                        block.Id = GetDataValue<long>(dr, "Id");
                        block.Hash = GetDataValue<string>(dr, "Hash");
                        block.Version = GetDataValue<int>(dr, "Version");
                        block.Height = GetDataValue<long>(dr, "Height");
                        block.PreviousBlockHash = GetDataValue<string>(dr, "PreviousBlockHash");
                        block.Bits = GetDataValue<long>(dr, "Bits");
                        block.Nonce = GetDataValue<long>(dr, "Nonce");
                        block.Timestamp = GetDataValue<long>(dr, "Timestamp");
                        block.NextBlockHash = GetDataValue<string>(dr, "NextBlockHash");
                        block.TotalAmount = GetDataValue<long>(dr, "TotalAmount");
                        block.TotalFee = GetDataValue<long>(dr, "TotalFee");
                        block.GeneratorId = GetDataValue<string>(dr, "GeneratorId");
                        block.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");
                        block.IsVerified = GetDataValue<bool>(dr, "IsVerified");

                        result.Add(block.Id, block);
                    }
                }
            }

            return result;
        }

        public virtual List<Block> DistinguishBlockVerifiedByHashes(List<string> hashes)
        {
            string SQL_STATEMENT = $"SELECT * FROM Blocks WHERE Hash IN('{string.Join("','", hashes.ToArray())}');";
            List<Block> result = null;
            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    result = new List<Block>();

                    while (dr.Read())
                    {
                        Block block = new Block();

                        block.Id = GetDataValue<long>(dr, "Id");
                        block.Hash = GetDataValue<string>(dr, "Hash");
                        block.Version = GetDataValue<int>(dr, "Version");
                        block.Height = GetDataValue<long>(dr, "Height");
                        block.PreviousBlockHash = GetDataValue<string>(dr, "PreviousBlockHash");
                        block.Bits = GetDataValue<long>(dr, "Bits");
                        block.Nonce = GetDataValue<long>(dr, "Nonce");
                        block.Timestamp = GetDataValue<long>(dr, "Timestamp");
                        block.NextBlockHash = GetDataValue<string>(dr, "NextBlockHash");
                        block.TotalAmount = GetDataValue<long>(dr, "TotalAmount");
                        block.TotalFee = GetDataValue<long>(dr, "TotalFee");
                        block.GeneratorId = GetDataValue<string>(dr, "GeneratorId");
                        block.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");
                        block.IsVerified = GetDataValue<bool>(dr, "IsVerified");

                        result.Add(block);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 确认次数
        /// </summary>
        /// <param name="blockHash"></param>
        /// <param name="height"></param>
        /// <param name="confirmations"></param>
        /// <returns></returns>
        public virtual ListSinceBlock GetSinceBlock(string blockHash, long height, long confirmations)
        {
            string SQL_STATEMENT =
                "SELECT t1.TransactionHash, t1.'Index', t2.IsDiscarded, t1.amount, t4.id AS Address, t4.Tag, t2.TotalInput, t2.Fee, t2.Timestamp, t3.Hash as BlockHash, t3.Timestamp as BlockTime, (@Height - t3.Height + 1) as Confirmations, t1.Spent, t2.LockTime "
                + "FROM OutputList t1 JOIN Transactions t2 ON t1.TransactionHash = t2.Hash "
                + "JOIN Blocks t3 ON t2.BlockHash = t3.Hash "
                + "JOIN Accounts t4 ON t1.ReceiverId = t4.Id "
                + "WHERE t1.IsDiscarded = 0 AND t3.IsDiscarded = 0 AND t3.height > (SELECT Height FROM Blocks WHERE Hash = @Hash);";
            if (string.IsNullOrEmpty(blockHash))
            {
                SQL_STATEMENT =
                    "SELECT t1.TransactionHash, t1.'Index', t2.IsDiscarded, t1.amount, t4.id AS Address, t4.Tag, t2.TotalInput, t2.Fee, t2.Timestamp, t3.Hash as BlockHash, t3.Timestamp as BlockTime, (@Height - t3.Height + 1) as Confirmations, t1.Spent, t2.LockTime "
                + "FROM OutputList t1 JOIN Transactions t2 ON t1.TransactionHash = t2.Hash "
                + "JOIN Blocks t3 ON t2.BlockHash = t3.Hash "
                + "JOIN Accounts t4 ON t1.ReceiverId = t4.Id "
                + "WHERE t1.IsDiscarded = 0 AND t3.IsDiscarded = 0;";
            }


            List<SinceBlock> sinceBlock = null;
            string lastestBlockHash = string.Empty;
            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            {
                con.Open();
                //con.SynchronousNORMAL();
                SqliteTransaction transaction = con.BeginTransaction();
                using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
                {
                    cmd.Parameters.AddWithValue("@Hash", blockHash);
                    cmd.Parameters.AddWithValue("@Height", height);
                    cmd.Parameters.AddWithValue("@Confirmations", confirmations);
                    cmd.Connection.Open();
                    SqliteDataReader dataReader = cmd.ExecuteReader();
                    sinceBlock = (
                        from IDataRecord x in dataReader
                        where (long)x["IsDiscarded"] == 0
                        select new SinceBlock
                        {
                            Account = "",
                            Address = (string)x["Address"],
                            amount = (long)x["Amount"],
                            Confirmations = (long)x["Confirmations"],
                            Category = ((long)x["TotalInput"] == 0 && (long)x["Fee"] == 0 && (long)x["Confirmations"] >= 100) ? "generate" : ((long)x["TotalInput"] == 0 && (long)x["Fee"] == 0) ? "immature" : "receive",
                            Vout = (long)x["Index"],
                            Fee = (long)x["Fee"],
                            LockTime = (long)x["LockTime"],
                            BlockHash = (string)x["BlockHash"],
                            BlockTime = (long)x["BlockTime"],
                            Label = x["Tag"].Equals(DBNull.Value) ? "" : (string)x["Tag"],
                            TxId = (string)x["TransactionHash"],
                            IsSpent = (long)x["Spent"] == 0 ? false : true
                        }).ToList();
                }

            }

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            {
                //获取lastestBlock
                long lastestBlockHeight = height - confirmations + 1;
                if (lastestBlockHeight < 0)
                {
                    return new ListSinceBlock { LastBlock = null, Transactions = sinceBlock?.ToArray() };
                }

                const string BLOCKSQL_STATEMENT =
                    "SELECT Hash " +
                    "FROM Blocks " +
                    "WHERE Height = @Height " +
                    "AND IsDiscarded = 0 limit 1; ";
                using (SqliteCommand cmd = new SqliteCommand(BLOCKSQL_STATEMENT, con))
                {
                    cmd.Parameters.AddWithValue("@Height", lastestBlockHeight);
                    cmd.Connection.Open();
                    using (SqliteDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lastestBlockHash = GetDataValue<string>(dr, "Hash");
                        }
                    }
                }
            }
            return new ListSinceBlock { LastBlock = lastestBlockHash, Transactions = sinceBlock?.ToArray() };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blockHash">当前的BlockHash</param>
        /// <param name="height"></param>
        /// <param name="confirmations">确认次数</param>
        /// <param name="currentPage">当前页</param>
        /// <param name="pageSize">每页显示数</param>
        /// <returns></returns>
        public virtual ListSinceBlock GetPageSinceBlock(string blockHash, long height, long confirmations, int currentPage, int pageSize)
        {
            string SQL_STATEMENT =
                "SELECT t1.TransactionHash, t1.'Index', t2.IsDiscarded, t1.amount, t4.id AS Address, t4.Tag, t2.TotalInput, t2.Fee, t2.Timestamp, t3.Hash as BlockHash, t3.Timestamp as BlockTime, (@Height - t3.Height + 1) as Confirmations, t1.Spent, t2.LockTime "
                + "FROM OutputList t1 JOIN Transactions t2 ON t1.TransactionHash = t2.Hash "
                + "JOIN Blocks t3 ON t2.BlockHash = t3.Hash "
                + "JOIN Accounts t4 ON t1.ReceiverId = t4.Id "
                + "WHERE t1.IsDiscarded = 0 AND t3.IsDiscarded = 0 AND t3.height >= (SELECT Height FROM Blocks WHERE Hash = @Hash)"
                + " LIMIT (@CurrentPage - 1) * @PageSize, @PageSize;";
            if (string.IsNullOrEmpty(blockHash))
            {
                SQL_STATEMENT =
                    "SELECT t1.TransactionHash, t1.'Index', t2.IsDiscarded, t1.amount, t4.id AS Address, t4.Tag, t2.TotalInput, t2.Fee, t2.Timestamp, t3.Hash as BlockHash, t3.Timestamp as BlockTime, (@Height - t3.Height + 1) as Confirmations, t1.Spent, t2.LockTime "
                + "FROM OutputList t1 JOIN Transactions t2 ON t1.TransactionHash = t2.Hash "
                + "JOIN Blocks t3 ON t2.BlockHash = t3.Hash "
                + "JOIN Accounts t4 ON t1.ReceiverId = t4.Id "
                + "WHERE t1.IsDiscarded = 0 AND t3.IsDiscarded = 0;"
                + " LIMIT (@CurrentPage - 1) * @PageSize, @PageSize;";
            }
            
            List<SinceBlock> sinceBlock = null;
            string lastestBlockHash = string.Empty;
            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            {
                con.Open();
                //con.SynchronousNORMAL();
                using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
                {
                    cmd.Parameters.AddWithValue("@Hash", blockHash);
                    cmd.Parameters.AddWithValue("@Height", height);
                    cmd.Parameters.AddWithValue("@CurrentPage", currentPage);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);
                    cmd.Connection.Open();

                    SqliteDataReader dataReader = cmd.ExecuteReader();
                    sinceBlock = (
                        from IDataRecord x in dataReader
                        where (long)x["IsDiscarded"] == 0
                        select new SinceBlock
                        {
                            Account = "",
                            Address = (string)x["Address"],
                            amount = (long)x["Amount"],
                            Confirmations = (long)x["Confirmations"],
                            Category = ((long)x["TotalInput"] == 0 && (long)x["Fee"] == 0 && (long)x["Confirmations"] >= 100) ? "generate" : ((long)x["TotalInput"] == 0 && (long)x["Fee"] == 0) ? "immature" : "receive",
                            Vout = (long)x["Index"],
                            Fee = (long)x["Fee"],
                            LockTime = (long)x["LockTime"],
                            BlockHash = (string)x["BlockHash"],
                            BlockTime = (long)x["BlockTime"],
                            Label = x["Tag"].Equals(DBNull.Value) ? "" : (string)x["Tag"],
                            TxId = (string)x["TransactionHash"],
                            IsSpent = (long)x["Spent"] == 0 ? false : true
                        }).ToList();
                }
            }

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            {
                //获取lastestBlock
                long lastestBlockHeight = height - confirmations + 1;
                if (lastestBlockHeight < 0)
                {
                    return new ListSinceBlock { LastBlock = null, Transactions = sinceBlock?.ToArray() };
                }

                const string BLOCKSQL_STATEMENT =
                    "SELECT Hash " +
                    "FROM Blocks " +
                    "WHERE Height = @Height " +
                    "AND IsDiscarded = 0 limit 1; ";

                con.Open();
                using (SqliteCommand cmd = new SqliteCommand(BLOCKSQL_STATEMENT, con))
                {
                    cmd.Parameters.AddWithValue("@Height", lastestBlockHeight);
                    cmd.Connection.Open();
                    using (SqliteDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lastestBlockHash = GetDataValue<string>(dr, "Hash");
                        }
                    }
                }
            }

            return new ListSinceBlock { LastBlock = lastestBlockHash, Transactions = sinceBlock?.ToArray() };
        }

        public virtual long GetBlockReward(string blockHash)
        {
            
            /*
            const string SQL_STATEMENT =
                "SELECT t1.TransactionHash, t1.'Index', t1.amount, t4.id AS Address, t4.Tag, t2.TotalInput, t2.Fee, t2.Timestamp, t3.Hash as BlockHash, t3.Timestamp as BlockTime, (@Height - t3.Height) as Confirmations "
                + "FROM OutputList t1 JOIN Transactions t2 ON t1.TransactionHash = t2.Hash "
                + "JOIN Blocks t3 ON t2.BlockHash = t3.Hash "
                + "JOIN Accounts t4 ON t1.ReceiverId = t4.Id "
                + "WHERE t1.IsDiscarded = 0 AND t2.IsDiscarded = 0 AND t3.IsDiscarded = 0 AND t3.height > (SELECT Height FROM Blocks WHERE Hash = @Hash) AND t3.height < @Height;";

            List<SinceBlock> sinceBlock = null;
            string lastestBlock = "";
            long blockTime = 0;
            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Hash", blockHash);
                cmd.Parameters.AddWithValue("@Height", height);
                cmd.Connection.Open();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    sinceBlock = new List<SinceBlock>();
                    while (dr.Read())
                    {
                        SinceBlock block = new SinceBlock();
                        block.Account = "";
                        block.Address = GetDataValue<string>(dr, "Address");
                        block.amount = GetDataValue<long>(dr, "Amount");
                        block.Category = (GetDataValue<long>(dr, "TotalInput") == 0 && GetDataValue<long>(dr, "Fee") == 0 && GetDataValue<long>(dr, "Confirmations") > 100) ? "generate"
                            : (GetDataValue<long>(dr, "TotalInput") == 0 && GetDataValue<long>(dr, "Fee") == 0 && GetDataValue<long>(dr, "Confirmations") < 100) ? "immature" : "receive";
                        block.Vout = GetDataValue<long>(dr, "Index");
                        block.Fee = GetDataValue<long>(dr, "Fee");
                        block.Confirmations = GetDataValue<long>(dr, "Confirmations");
                        block.BlockHash = GetDataValue<string>(dr, "BlockHash");
                        block.BlockTime = GetDataValue<long>(dr, "BlockTime");
                        block.Label = GetDataValue<string>(dr, "Tag") ?? "";
                        block.TxId = GetDataValue<string>(dr, "TransactionHash");
                        if (block.BlockTime > blockTime)
                        {
                            lastestBlock = block.BlockHash;
                        }
                        sinceBlock.Add(block);
                    }
                }
            }
            if (sinceBlock == null && sinceBlock.Count == 0)
            {
                return null;
            }
            else
            {
                return new ListSinceBlock { LastBlock = lastestBlock, Transactions = sinceBlock.ToArray() };
            }
            */
            return 0;
        }

        public virtual List<BlockTrans> GetBlocksByTxHash(List<string> hashes)
        {
            if (hashes == null || !hashes.Any())
                return new List<BlockTrans>();
            string SQL_STATEMENT = $"select a.Hash as BlockHash,a.Height,a.[timestamp],b.Hash as TxHash from blocks as a inner join transactions as b on a.Hash = b.BlockHash and b.hash IN( '{string.Join("','", hashes.ToArray())}');";
            List<BlockTrans> result = null;
            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    result = new List<BlockTrans>();

                    while (dr.Read())
                    {
                        BlockTrans bt = new BlockTrans();

                        bt.BlockHash = GetDataValue<string>(dr, "BlockHash");
                        bt.Height = GetDataValue<long>(dr, "Height");
                        bt.Timestamp = GetDataValue<long>(dr, "Timestamp");
                        bt.TransHash = GetDataValue<string>(dr, "TxHash");

                        result.Add(bt);
                    }
                }
            }

            return result;
        }

        public virtual List<BlockConstraint> GetConstraintsByBlockHashs(IEnumerable<string> hashs)
        {
            List<BlockConstraint> result= new List<BlockConstraint>();
            var condition= $"('{string.Join("','", hashs)}')";
            //string SQL_PREV_TRANSACTIONS = $"UPDATE Transactions SET HASH = REPLACE(HASH,' ','') WHERE REPLACE(HASH,' ','') IN {condition};";
            //string SQL_PREV_OUTPUTS = $"UPDATE outputList SET Transactionhash = REPLACE(Transactionhash,' ','') WHERE REPLACE(Transactionhash, ' ', '') IN {condition};";
            string SQL_MAIN = $"select outputList.TransactionHash,Blocks.Height,Transactions.LockTime," +
                                   $"(select count(1) from inputlist where OutputTransactionHash = '0000000000000000000000000000000000000000000000000000000000000000' and outputList.transactionHash = inputlist.transactionHash) as IsCoinBase " +
                                   $"from outputList " +
                                   $"join Transactions on outputList.transactionHash = Transactions.Hash " +
                                   $"join Blocks on outputList.BlockHash = Blocks.Hash and blocks.IsDiscarded = 0 " +
                                   $"where transactionHash in {condition};" ;

            string SQL_STATEMENT = SQL_MAIN;// SQL_PREV_TRANSACTIONS +SQL_PREV_OUTPUTS + SQL_MAIN;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        BlockConstraint constraint = new BlockConstraint();
                        constraint.Height = GetDataValue<long>(dr, "Height");
                        constraint.TransactionHash = GetDataValue<string>(dr, "TransactionHash");
                        constraint.LockTime = GetDataValue<long>(dr, "LockTime");
                        constraint.IsCoinBase = GetDataValue<bool>(dr, "IsCoinBase");

                        result.Add(constraint);
                    }
                }
            }

            return result;
        }
    }
}
