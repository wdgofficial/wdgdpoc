// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.Entities;
using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Text;
using FiiiChain.Entities.ExtensionModels;
using System.Linq;
using FiiiChain.Framework;

namespace FiiiChain.Data
{
    public class OutputDac : DataAccessComponent<OutputDac>
    {
        public virtual long Insert(Output output)
        {
            const string SQL_STATEMENT =
                "INSERT INTO OutputList " +
                "([Index], TransactionHash, ReceiverId, Amount, Size, LockScript, Spent, IsDiscarded, BlockHash) " +
                "VALUES (@Index, @TransactionHash, @ReceiverId, @Amount, @Size, @LockScript, @Spent, @IsDiscarded, @BlockHash); " +
                "SELECT LAST_INSERT_ROWID() newid;";

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Index", output.Index);
                cmd.Parameters.AddWithValue("@TransactionHash", output.TransactionHash);
                cmd.Parameters.AddWithValue("@ReceiverId", output.ReceiverId);
                cmd.Parameters.AddWithValue("@Amount", output.Amount);
                cmd.Parameters.AddWithValue("@Size", output.Size);
                cmd.Parameters.AddWithValue("@LockScript", output.LockScript);
                cmd.Parameters.AddWithValue("@Spent", output.Spent);
                cmd.Parameters.AddWithValue("@IsDiscarded", output.IsDiscarded);
                cmd.Parameters.AddWithValue("@BlockHash", output.BlockHash);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                return Convert.ToInt64(cmd.ExecuteScalar());
            }
        }

        public virtual void UpdateSpentStatus(string transactionHash, int index)
        {
            const string SQL_STATEMENT =
                "UPDATE OutputList " +
                "SET Spent = @Spent " +
                "WHERE TransactionHash = @TransactionHash AND [Index] = @Index;";

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@TransactionHash", transactionHash);
                cmd.Parameters.AddWithValue("@Index", index);
                cmd.Parameters.AddWithValue("@Spent", true);
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                cmd.ExecuteNonQuery();
            }
        }

        public virtual void UpdateSpentStatuses(IEnumerable<string> hashIndexs)
        {
            if (hashIndexs == null || !hashIndexs.Any())
                return;
            
            var condition = $"('{string.Join("','", hashIndexs)}')" ;
            
            string SQL_STATEMENT =
                "UPDATE OutputList " +
                "SET Spent = @Spent " +
                $"WHERE TransactionHash||[Index] IN {condition};";

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Spent", true);
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                cmd.ExecuteNonQuery();
            }
        }


        public virtual void UpdateSpentStatus(IEnumerable<SimpleUtxo> utxos)
        {
            const string SQL_STATEMENT =
                "UPDATE OutputList " +
                "SET Spent = 1 " +
                "WHERE TransactionHash = @TransactionHash AND [Index] = @Index;";

            var paramsList = new List<SqliteParameter[]>();

            foreach (var item in utxos)
            {
                var parameters = new SqliteParameter[2];
                parameters[0] = new SqliteParameter("@TransactionHash", item.TransactionHash);
                parameters[1] = new SqliteParameter("@Index", item.Index);
                paramsList.Add(parameters);
            }

            SqliteHelper.ExecuteNonQuery(SQL_STATEMENT, paramsList.ToArray());
        }

        public virtual void UpdateBlockAndDiscardStatus(long id, string blockHash, bool isDiscarded)
        {
            const string SQL_STATEMENT =
                "UPDATE OutputList " +
                "SET IsDiscarded = @IsDiscarded, " +
                "BlockHash = @BlockHash " +
                "WHERE Id = @Id;";

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@IsDiscarded", isDiscarded);
                cmd.Parameters.AddWithValue("@BlockHash", blockHash);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                cmd.ExecuteNonQuery();
            }
        }

        public virtual List<Output> SelectByTransactionHash(string transactionHash)
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM OutputList " +
                "WHERE TransactionHash = @TransactionHash " +
                "AND IsDiscarded = 0 ";

            List<Output> result = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@TransactionHash", transactionHash);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    result = new List<Output>();

                    while (dr.Read())
                    {
                        Output output = new Output();

                        output.Id = GetDataValue<long>(dr, "Id");
                        output.Index = GetDataValue<int>(dr, "Index");
                        output.TransactionHash = GetDataValue<string>(dr, "TransactionHash");
                        output.ReceiverId = GetDataValue<string>(dr, "ReceiverId");
                        output.Size = GetDataValue<int>(dr, "Size");
                        output.Amount = GetDataValue<long>(dr, "Amount");
                        output.LockScript = GetDataValue<string>(dr, "LockScript");
                        output.Spent = GetDataValue<bool>(dr, "Spent");
                        output.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");
                        output.BlockHash = GetDataValue<string>(dr, "BlockHash");

                        result.Add(output);
                    }
                }
            }

            return result;
        }

        public virtual List<Output> SelectFirstDiscardedByTransactionHash(string transactionHash)
        {
            const string SQL_STATEMENT =
                "SELECT * FROM OutputList WHERE id IN ( " +
                "SELECT Id FROM OutputList WHERE IsDiscarded = 1 AND TransactionHash = @TransactionHash " +
                "GROUP BY `Index`, TransactionHash " +
                "HAVING COUNT(0) > 1)";

            List<Output> result = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@TransactionHash", transactionHash);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    result = new List<Output>();

                    while (dr.Read())
                    {
                        Output output = new Output();

                        output.Id = GetDataValue<long>(dr, "Id");
                        output.Index = GetDataValue<int>(dr, "Index");
                        output.TransactionHash = GetDataValue<string>(dr, "TransactionHash");
                        output.ReceiverId = GetDataValue<string>(dr, "ReceiverId");
                        output.Size = GetDataValue<int>(dr, "Size");
                        output.Amount = GetDataValue<long>(dr, "Amount");
                        output.LockScript = GetDataValue<string>(dr, "LockScript");
                        output.Spent = GetDataValue<bool>(dr, "Spent");
                        output.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");
                        output.BlockHash = GetDataValue<string>(dr, "BlockHash");

                        result.Add(output);
                    }
                }
            }

            return result;
        }

        public virtual Output SelectById(long id)
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM OutputList " +
                "WHERE Id = @Id " +
                "AND IsDiscarded = 0 ";

            Output output = null;

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
                        output = new Output();

                        output.Id = GetDataValue<long>(dr, "Id");
                        output.Index = GetDataValue<int>(dr, "Index");
                        output.TransactionHash = GetDataValue<string>(dr, "TransactionHash");
                        output.ReceiverId = GetDataValue<string>(dr, "ReceiverId");
                        output.Size = GetDataValue<int>(dr, "Size");
                        output.Amount = GetDataValue<long>(dr, "Amount");
                        output.LockScript = GetDataValue<string>(dr, "LockScript");
                        output.Spent = GetDataValue<bool>(dr, "Spent");
                        output.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");
                        output.BlockHash = GetDataValue<string>(dr, "BlockHash");
                    }
                }

                return output;
            }
        }
        public virtual Output SelectByHashAndIndex(string transactionHash, int index)
        {
            const string SQL_STATEMENT =
                "SELECT Id, [Index], TransactionHash, ReceiverId, Size, Amount, LockScript, Spent, IsDiscarded, BlockHash " +
                "FROM OutputList " +
                "WHERE TransactionHash = @TransactionHash AND [Index] = @Index " +
                "AND IsDiscarded = 0 ";

            Output output = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@TransactionHash", transactionHash);
                cmd.Parameters.AddWithValue("@Index", index);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        output = new Output();
                        /*
                        output.Id = GetDataValue<long>(dr, "Id");
                        output.Index = GetDataValue<int>(dr, "Index");
                        output.TransactionHash = GetDataValue<string>(dr, "TransactionHash");
                        output.ReceiverId = GetDataValue<string>(dr, "ReceiverId");
                        output.Size = GetDataValue<int>(dr, "Size");
                        output.Amount = GetDataValue<long>(dr, "Amount");
                        output.LockScript = GetDataValue<string>(dr, "LockScript");
                        output.Spent = GetDataValue<bool>(dr, "Spent");
                        output.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");
                        output.BlockHash = GetDataValue<string>(dr, "BlockHash");
                        */

                        output.Id = dr.GetInt64(0);
                        output.Index = dr.GetInt32(1);
                        output.TransactionHash = dr.GetString(2);
                        output.ReceiverId = dr.GetString(3);
                        output.Size = dr.GetInt32(4);
                        output.Amount = dr.GetInt64(5);
                        output.LockScript = dr.GetString(6);
                        output.Spent = dr.GetBoolean(7);
                        output.IsDiscarded = dr.GetBoolean(8);
                        output.BlockHash = dr.GetString(9);
                    }
                }

                return output;
            }
        }

        public virtual List<Output> SelectByHashAndIndexs(IEnumerable<string> hashIndexs)
        {
            if (hashIndexs == null || !hashIndexs.Any())
                return new List<Output>();

            var condition = $"('{string.Join("','", hashIndexs)}')";
            string SQL_STATEMENT =
                "SELECT Id, [Index], TransactionHash, ReceiverId, Size, Amount, LockScript, Spent, IsDiscarded, BlockHash " +
                "FROM OutputList " +
                "WHERE TransactionHash||[Index] IN " +
                condition +
                " AND IsDiscarded = 0 ";

            List<Output> outputs = new List<Output>();

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var output = new Output();

                        output.Id = dr.GetInt64(0);
                        output.Index = dr.GetInt32(1);
                        output.TransactionHash = dr.GetString(2);
                        output.ReceiverId = dr.GetString(3);
                        output.Size = dr.GetInt32(4);
                        output.Amount = dr.GetInt64(5);
                        output.LockScript = dr.GetString(6);
                        output.Spent = dr.GetBoolean(7);
                        output.IsDiscarded = dr.GetBoolean(8);
                        output.BlockHash = dr.GetString(9);
                        outputs.Add(output);
                    }
                }

                return outputs;
            }
        }

        public virtual List<Output> SelectUnspentByHashAndIndexs(IEnumerable<string> hashIndexs)
        {
            if (hashIndexs == null || !hashIndexs.Any())
                return new List<Output>();

            var condition = $"('{string.Join("','", hashIndexs)}')";
            string SQL_STATEMENT =
                "SELECT Id, [Index], TransactionHash, ReceiverId, Size, Amount, LockScript, " +
                "(select count(1) from inputlist WHERE OutputTransactionHash = outputList.TransactionHash and inputlist.OutputIndex = outputList.[Index]) AS Spent, " +
                "IsDiscarded, BlockHash " +
                "FROM OutputList " +
                "WHERE TransactionHash||[Index] IN " +
                condition +
                " AND IsDiscarded = 0;";

            List<Output> outputs = new List<Output>();

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var output = new Output();

                        output.Id = dr.GetInt64(0);
                        output.Index = dr.GetInt32(1);
                        output.TransactionHash = dr.GetString(2);
                        output.ReceiverId = dr.GetString(3);
                        output.Size = dr.GetInt32(4);
                        output.Amount = dr.GetInt64(5);
                        output.LockScript = dr.GetString(6);
                        output.Spent = dr.GetBoolean(7);
                        output.IsDiscarded = dr.GetBoolean(8);
                        output.BlockHash = dr.GetString(9);
                        outputs.Add(output);
                    }
                }

                return outputs;
            }
        }

        public virtual List<Output> SelectUnspentByReceiverId(string receiverId)
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM OutputList " +
                "WHERE ReceiverId = @ReceiverId AND Spent = @Spent " +
                "AND IsDiscarded = 0 ";

            List<Output> result = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@ReceiverId", receiverId);
                cmd.Parameters.AddWithValue("@Spent", false);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    result = new List<Output>();

                    while (dr.Read())
                    {
                        var output = new Output();

                        output.Id = GetDataValue<long>(dr, "Id");
                        output.Index = GetDataValue<int>(dr, "Index");
                        output.TransactionHash = GetDataValue<string>(dr, "TransactionHash");
                        output.ReceiverId = GetDataValue<string>(dr, "ReceiverId");
                        output.Size = GetDataValue<int>(dr, "Size");
                        output.Amount = GetDataValue<long>(dr, "Amount");
                        output.LockScript = GetDataValue<string>(dr, "LockScript");
                        output.Spent = GetDataValue<bool>(dr, "Spent");
                        output.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");
                        output.BlockHash = GetDataValue<string>(dr, "BlockHash");
                        result.Add(output);
                    }
                }

                return result;
            }
        }

        public virtual List<Output> SelectAllUnspentOutputs()
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM OutputList " +
                "WHERE Spent = 0 AND ReceiverId IN " +
                "(SELECT Id FROM Accounts WHERE PrivateKey IS NOT NULL) " +
                "AND IsDiscarded = 0";

            List<Output> result = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    result = new List<Output>();

                    while (dr.Read())
                    {
                        var output = new Output();

                        output.Id = GetDataValue<long>(dr, "Id");
                        output.Index = GetDataValue<int>(dr, "Index");
                        output.TransactionHash = GetDataValue<string>(dr, "TransactionHash");
                        output.ReceiverId = GetDataValue<string>(dr, "ReceiverId");
                        output.Size = GetDataValue<int>(dr, "Size");
                        output.Amount = GetDataValue<long>(dr, "Amount");
                        output.LockScript = GetDataValue<string>(dr, "LockScript");
                        output.Spent = GetDataValue<bool>(dr, "Spent");
                        output.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");
                        output.BlockHash = GetDataValue<string>(dr, "BlockHash");
                        result.Add(output);
                    }
                }

                return result;
            }
        }

        public virtual List<Output> SelectAllUnspentOutputs(long start, long limit)
        {
            string SQL_STATEMENT =
                "SELECT * " +
                "FROM OutputList " +
                "WHERE Spent = 0 AND ReceiverId IN " +
                "(SELECT Id FROM Accounts WHERE PrivateKey IS NOT NULL) " +
                $"AND IsDiscarded = 0 limit {start}, {limit}";

            List<Output> result = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    result = new List<Output>();

                    while (dr.Read())
                    {
                        var output = new Output();

                        output.Id = GetDataValue<long>(dr, "Id");
                        output.Index = GetDataValue<int>(dr, "Index");
                        output.TransactionHash = GetDataValue<string>(dr, "TransactionHash");
                        output.ReceiverId = GetDataValue<string>(dr, "ReceiverId");
                        output.Size = GetDataValue<int>(dr, "Size");
                        output.Amount = GetDataValue<long>(dr, "Amount");
                        output.LockScript = GetDataValue<string>(dr, "LockScript");
                        output.Spent = GetDataValue<bool>(dr, "Spent");
                        output.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");
                        output.BlockHash = GetDataValue<string>(dr, "BlockHash");
                        result.Add(output);
                    }
                }

                return result;
            }
        }
        public virtual long CountSelfUnspentOutputs()
        {
            const string SQL_STATEMENT =
                "SELECT COUNT(0) " +
                "FROM OutputList " +
                "WHERE Spent = 0 AND ReceiverId IN " +
                "(SELECT Id FROM Accounts WHERE PrivateKey IS NOT NULL) " +
                "AND IsDiscarded = 0 ";

            long result = 0;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                result = (long)cmd.ExecuteScalar();
            }

            return result;
        }
        public virtual long SumSelfUnspentOutputs()
        {
            const string SQL_STATEMENT =
                "SELECT CASE WHEN SUM(Amount) IS NULL THEN 0 ELSE SUM(Amount) END AS Balance " +
                "FROM OutputList " +
                "WHERE ReceiverId IN " +
                "(SELECT Id FROM Accounts WHERE PrivateKey IS NOT NULL) AND " +
                "outputlist.TransactionHash||outputlist.`Index` in (select OutputTransactionHash||OutputIndex from Inputlist where IsDiscarded = 0) " +
                "AND IsDiscarded = 0 ";

            long result = 0;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                result = (long)cmd.ExecuteScalar();
            }

            return result;
        }

        public virtual List<Output> GetOutputEntitesContainUnspentUTXOByTxHash(long lockTime)
        {
            const string SQL_STATEMENT =
                "SELECT Id, [Index], Amount, LockScript, ReceiverId, Size, Spent, TransactionHash, IsDiscarded, BlockHash FROM OutputList WHERE Spent = 0 AND IsDiscarded = 0 AND ReceiverId IN (SELECT Id FROM Accounts WHERE PrivateKey IS NOT NULL) AND TransactionHash IN (SELECT Hash FROM Transactions WHERE LockTime < @CurrentTime)";

            List<Output> result = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@CurrentTime", lockTime);
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    result = new List<Output>();

                    while (dr.Read())
                    {
                        Output output = new Output();

                        /*
                        output.Id = GetDataValue<long>(dr, "Id");
                        output.Index = GetDataValue<int>(dr, "Index");
                        output.Amount = GetDataValue<long>(dr, "Amount");
                        output.LockScript = GetDataValue<string>(dr, "LockScript");
                        output.ReceiverId = GetDataValue<string>(dr, "ReceiverId");
                        output.Size = GetDataValue<int>(dr, "Size");
                        output.Spent = GetDataValue<bool>(dr, "Spent");
                        output.TransactionHash = GetDataValue<string>(dr, "TransactionHash");
                        output.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");
                        output.BlockHash = GetDataValue<string>(dr, "BlockHash");
                        */
                        output.Id = dr.GetInt64(0);
                        output.Index = dr.GetInt32(1);
                        output.Amount = dr.GetInt64(2);
                        output.LockScript = dr.GetString(3);
                        output.ReceiverId = dr.GetString(4);
                        output.Size = dr.GetInt32(5);
                        output.Spent = dr.GetBoolean(6);
                        output.TransactionHash = dr.GetString(7);
                        output.IsDiscarded = dr.GetBoolean(8);
                        output.BlockHash = dr.GetString(9);

                        result.Add(output);
                    }
                }
            }

            return result;
        }

        public virtual List<OutputConfirmInfo> GetOutputEntitesContainUnspentUTXOByTxHash(long minAmount, long maxAmount, int currentPage, int pageSize, bool isDesc, long lockTime, long minConfirmations, long maxConfirmations, long latestHeight)
        {
            string SQL_STATEMENT =
                $"SELECT({latestHeight} - b.height + 1) as confirmations, a.* from Outputlist a left join blocks b on a.blockhash = b.hash Where a.TransactionHash NOT IN(SELECT Hash FROM Transactions WHERE LockTime > {lockTime}) AND a.Spent = 0 AND a.ReceiverId IN(SELECT Id FROM Accounts WHERE PrivateKey IS NOT NULL) AND a.IsDiscarded = 0 AND ({latestHeight} - b.height + 1) <= {maxConfirmations} and({latestHeight} - b.height + 1) >= {minConfirmations} and a.amount <= {maxAmount} and a.amount >= {minAmount} order by amount {(isDesc ? "DESC" : "ASC")}  LIMIT {(currentPage - 1) * pageSize}, {pageSize};";
            List<OutputConfirmInfo> result = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    result = new List<OutputConfirmInfo>();

                    while (dr.Read())
                    {
                        OutputConfirmInfo output = new OutputConfirmInfo();

                        output.Id = GetDataValue<long>(dr, "Id");
                        output.Index = GetDataValue<int>(dr, "Index");
                        output.Amount = GetDataValue<long>(dr, "Amount");
                        output.LockScript = GetDataValue<string>(dr, "LockScript");
                        output.ReceiverId = GetDataValue<string>(dr, "ReceiverId");
                        output.Size = GetDataValue<int>(dr, "Size");
                        output.Spent = GetDataValue<bool>(dr, "Spent");
                        output.TransactionHash = GetDataValue<string>(dr, "TransactionHash");
                        output.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");
                        output.BlockHash = GetDataValue<string>(dr, "BlockHash");
                        output.Confirmations = GetDataValue<long>(dr, "confirmations");

                        result.Add(output);
                    }
                }
            }
            return result;
        }

        public virtual List<OutputConfirmInfo> GetOutputEntitesContainUnspentUTXOByTxHash(int currentPage, int pageSize, long lockTime, long latestHeight)
        {
            string SQL_STATEMENT =
                $"SELECT({latestHeight} - b.height + 1) as confirmations, a.Id, a.[Index], a.TransactionHash, a.ReceiverId, a.Amount, a.Size, a.LockScript, a.Spent, a.IsDiscarded, a.BlockHash from Outputlist a left join blocks b on a.blockhash = b.hash left join Transactions t ON t.hash = a.TransactionHash Where t.LockTime < {lockTime} AND t.IsDiscarded = 0 AND b.IsVerified = 1 AND((t.TotalInput = 0 AND t.Fee = 0 AND({ latestHeight} - b.height + 1) > 100) OR(t.TotalInput > 0 AND t.Fee > 0)) AND a.Spent = 0 AND a.ReceiverId IN(SELECT Id FROM Accounts WHERE PrivateKey IS NOT NULL) AND a.IsDiscarded = 0 AND({latestHeight} - b.height + 1) <= {Int64.MaxValue} AND ({latestHeight} - b.height + 1) >= 0 AND a.amount <= {Int64.MaxValue} and a.amount >= 0 Order by a.Amount DESC LIMIT {(currentPage - 1) * pageSize}, {pageSize}; ";
            List<OutputConfirmInfo> result = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    result = new List<OutputConfirmInfo>();

                    while (dr.Read())
                    {
                        OutputConfirmInfo output = new OutputConfirmInfo();

                        output.Id = GetDataValue<long>(dr, "Id");
                        output.Index = GetDataValue<int>(dr, "Index");
                        output.Amount = GetDataValue<long>(dr, "Amount");
                        output.LockScript = GetDataValue<string>(dr, "LockScript");
                        output.ReceiverId = GetDataValue<string>(dr, "ReceiverId");
                        output.Size = GetDataValue<int>(dr, "Size");
                        output.Spent = GetDataValue<bool>(dr, "Spent");
                        output.TransactionHash = GetDataValue<string>(dr, "TransactionHash");
                        output.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");
                        output.BlockHash = GetDataValue<string>(dr, "BlockHash");
                        output.Confirmations = GetDataValue<long>(dr, "confirmations");

                        result.Add(output);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// UTXO合并拆分专用
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="lockTime"></param>
        /// <param name="latestHeight"></param>
        /// <returns></returns>
        public virtual List<OutputConfirmInfo> GetOutputEntitesContainUnspentUTXOByTxHash(int currentPage, int pageSize, long lockTime, long latestHeight, long maxAmount, long minAmount)
        {
            string SQL_STATEMENT =
                $"SELECT({latestHeight} - b.height + 1) as confirmations, a.Id, a.[Index], a.TransactionHash, a.ReceiverId, a.Amount, a.Size, a.LockScript, a.Spent, a.IsDiscarded, a.BlockHash from Outputlist a left join blocks b on a.blockhash = b.hash left join Transactions t ON t.hash = a.TransactionHash Where t.LockTime < {lockTime} AND t.IsDiscarded = 0 AND b.IsVerified = 1 AND((t.TotalInput = 0 AND t.Fee = 0 AND({ latestHeight} - b.height + 1) > 100) OR(t.TotalInput > 0 AND t.Fee > 0)) AND a.Spent = 0 AND a.ReceiverId IN(SELECT Id FROM Accounts WHERE PrivateKey IS NOT NULL) AND a.IsDiscarded = 0 AND({latestHeight} - b.height + 1) <= {Int64.MaxValue} AND ({latestHeight} - b.height + 1) >= 0 AND a.amount <= {maxAmount} and a.amount >= {minAmount} Order by a.Amount DESC LIMIT {(currentPage - 1) * pageSize}, {pageSize}; ";
            List<OutputConfirmInfo> result = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    result = new List<OutputConfirmInfo>();

                    while (dr.Read())
                    {
                        OutputConfirmInfo output = new OutputConfirmInfo();

                        output.Id = GetDataValue<long>(dr, "Id");
                        output.Index = GetDataValue<int>(dr, "Index");
                        output.Amount = GetDataValue<long>(dr, "Amount");
                        output.LockScript = GetDataValue<string>(dr, "LockScript");
                        output.ReceiverId = GetDataValue<string>(dr, "ReceiverId");
                        output.Size = GetDataValue<int>(dr, "Size");
                        output.Spent = GetDataValue<bool>(dr, "Spent");
                        output.TransactionHash = GetDataValue<string>(dr, "TransactionHash");
                        output.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");
                        output.BlockHash = GetDataValue<string>(dr, "BlockHash");
                        output.Confirmations = GetDataValue<long>(dr, "confirmations");

                        result.Add(output);
                    }
                }
            }
            return result;
        }

        public virtual long GetOutputEntitesContainUnspentUTXOCount(long minAmount, long maxAmount, long lockTime)
        {
            string SQL_STATEMENT =
                "SELECT COUNT(*) " +
                "FROM OutputList " +
                $"WHERE TransactionHash NOT IN (SELECT Hash FROM Transactions WHERE LockTime > {lockTime}) AND Spent = 0 AND ReceiverId IN " +
                "(SELECT Id FROM Accounts WHERE PrivateKey IS NOT NULL) " +
                $"AND IsDiscarded = 0 AND Amount >= {minAmount} AND Amount <= {maxAmount};";
            //计算总数目
            long result = 0;
            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                result = (long)cmd.ExecuteScalar();

            }
            return result;
        }

        public virtual List<BalanceHelper> SelectAllBalanceHelper()
        {
            const string SQL_STATEMENT =
                "SELECT b.Hash AS TransactionHash, b.BlockHash, b.TotalInput, c.height AS Height, c.IsVerified, b.LockTime, a.Amount, b.IsDiscarded FROM " +
                "OutputList a Inner JOIN Transactions b ON a.TransactionHash = b.Hash " +
                "Inner Join Blocks c ON c.Hash = b.BlockHash " +
                "WHERE a.Spent = 0 AND a.ReceiverId IN " +
                "(SELECT Id FROM Accounts WHERE PrivateKey IS NOT NULL) " +
                "AND a.IsDiscarded = 0  AND c.IsDiscarded = 0 AND a.Amount > 0;";

            List<BalanceHelper> result = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    result = new List<BalanceHelper>();

                    while (dr.Read())
                    {
                        BalanceHelper balance = new BalanceHelper();

                        balance.TransactionHash = dr.GetString(0);
                        balance.BlockHash = dr.GetString(1);
                        balance.TotalInput = dr.GetInt64(2);
                        balance.Height = dr.GetInt64(3);
                        balance.IsVerified = dr.GetBoolean(4);
                        balance.LockTime = dr.GetInt64(5);
                        balance.Amount = dr.GetInt64(6);
                        balance.IsDiscarded = dr.GetBoolean(7);
                        /*
                        balance.TransactionHash = dr.GetFieldValue<string>(0);
                        balance.BlockHash = dr.GetFieldValue<string>(1);
                        balance.TotalInput = dr.GetFieldValue<long>(2);
                        balance.Height = dr.GetFieldValue<long>(3);
                        balance.IsVerified = dr.GetFieldValue<bool>(4);
                        balance.LockTime = dr.GetFieldValue<long>(5);
                        balance.Amount = dr.GetFieldValue<long>(6);
                        balance.IsDiscarded = dr.GetFieldValue<bool>(7);
                        */
                        /*
                        balance.Amount = GetDataValue<long>(dr, "Amount");
                        balance.BlockHash = GetDataValue<string>(dr, "BlockHash");
                        balance.Height = GetDataValue<long>(dr, "Height");
                        balance.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");
                        balance.IsVerified = GetDataValue<bool>(dr, "IsVerified");
                        balance.LockTime = GetDataValue<long>(dr, "LockTime");
                        balance.TotalInput = GetDataValue<long>(dr, "TotalInput");
                        balance.TransactionHash = GetDataValue<string>(dr, "TransactionHash");
                        */

                        result.Add(balance);
                    }
                }
            }

            return result;
        }

        public virtual List<Output> SelectSelfAll(long? minId = null)
        {
            string SQL_STATEMENT = $"SELECT Id, [Index], TransactionHash, ReceiverId, Size, Amount, LockScript, Spent, IsDiscarded, BlockHash " +
                $"FROM OutputList Where IsDiscarded = 0 { (!minId.HasValue ? "" : " And Id >" + minId.Value)} and receiverid in(select id from Accounts) ";

            List<Output> outputs = new List<Output>();

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var output = new Output();

                        output.Id = dr.GetInt64(0);
                        output.Index = dr.GetInt32(1);
                        output.TransactionHash = dr.GetString(2);
                        output.ReceiverId = dr.GetString(3);
                        output.Size = dr.GetInt32(4);
                        output.Amount = dr.GetInt64(5);
                        output.LockScript = dr.GetString(6);
                        output.Spent = dr.GetBoolean(7);
                        output.IsDiscarded = dr.GetBoolean(8);
                        output.BlockHash = dr.GetString(9);
                        outputs.Add(output);
                    }
                }

                return outputs;
            }
        }
        
        public virtual List<Output> SelectAll()
        {
            const string SQL_STATEMENT = "SELECT Id, [Index], TransactionHash, ReceiverId, Size, Amount, LockScript, Spent, IsDiscarded, BlockHash FROM OutputList " +
                "Where IsDiscarded = 0 and receiverid in (select id from accounts)";

            List<Output> outputs = new List<Output>();

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var output = new Output();

                        output.Id = dr.GetInt64(0);
                        output.Index = dr.GetInt32(1);
                        output.TransactionHash = dr.GetString(2);
                        output.ReceiverId = dr.GetString(3);
                        output.Size = dr.GetInt32(4);
                        output.Amount = dr.GetInt64(5);
                        output.LockScript = dr.GetString(6);
                        output.Spent = dr.GetBoolean(7);
                        output.IsDiscarded = dr.GetBoolean(8);
                        output.BlockHash = dr.GetString(9);
                        outputs.Add(output);
                    }
                }

                return outputs;
            }
        }

        public virtual Dictionary<long, string> SelectDropedTxOutputIds()
        {
            Dictionary<long, string> result = new Dictionary<long, string>();
            const string SQL_STATEMENT = "select Id,transactionHash||[index] as outputkey from OutputList where receiverid in (select id from accounts) and spent = 1 and outputkey not in (select outputTransactionHash || outputindex from inputList)";
            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var Id = dr.GetInt64(0);
                        var outputkey = dr.GetString(1);
                        result.Add(Id, outputkey);
                    }
                }

                return result;
            }
        }

        public virtual Dictionary<long, string> SelectSpentedTxNotInOutputIds()
        {
            Dictionary<long, string> result = new Dictionary<long, string>();
            var txHashIndex = "transactionHash||[index]";
            string MY_SQL_STATEMENT =
$"select Id,{txHashIndex} as outputkey from OutputList where receiverid in (select id from accounts) and spent = 0 and {txHashIndex} in (select outputTransactionHash||outputindex from inputList)";
            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(MY_SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var Id = dr.GetInt64(0);
                        var outputkey = dr.GetString(1);
                        result.Add(Id, outputkey);
                    }
                }

                return result;
            }
        }

        public virtual void UpdateOutputSpentState(IEnumerable<long> ids, int state = 0)
        {
            string SQL_STATEMENT = $"update OutputList set spent = {state} WHERE Id IN('{string.Join("','", ids)}') ";
            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                cmd.ExecuteNonQuery();
            }
        }

    }
}
