// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.Entities;
using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Text;

namespace FiiiChain.Data
{
    public class InputDac : DataAccessComponent<InputDac>
    {
        public virtual long Insert(Input input)
        {
            const string SQL_STATEMENT =
                "INSERT INTO InputList " +
                "(TransactionHash, OutputTransactionHash, OutputIndex, Size, Amount, UnlockScript, AccountId, IsDiscarded， BlockHash) " +
                "VALUES (@TransactionHash, @OutputTransactionHash, @OutputIndex, @Size, @Amount, @UnlockScript, @AccountId, @IsDiscarded, @BlockHash);" +
                "SELECT LAST_INSERT_ROWID() newid;";

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@TransactionHash", input.TransactionHash);
                cmd.Parameters.AddWithValue("@OutputTransactionHash", input.OutputTransactionHash);
                cmd.Parameters.AddWithValue("@OutputIndex", input.OutputIndex);
                cmd.Parameters.AddWithValue("@Size", input.Size);
                cmd.Parameters.AddWithValue("@Amount", input.Amount);
                cmd.Parameters.AddWithValue("@UnlockScript", input.UnlockScript);

                if (string.IsNullOrWhiteSpace(input.AccountId))
                {
                    cmd.Parameters.AddWithValue("@AccountId", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@AccountId", input.AccountId);
                }

                cmd.Parameters.AddWithValue("@IsDiscarded", input.IsDiscarded);
                cmd.Parameters.AddWithValue("@BlockHash", input.BlockHash);
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                return Convert.ToInt64(cmd.ExecuteScalar());
            }

        }

        public virtual void UpdateBlockAndDiscardStatus(long id, string blockHash, bool isDiscarded)
        {
            const string SQL_STATEMENT =
                "UPDATE InputList " +
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

        public virtual List<Input> SelectByTransactionHash(string txHash)
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM InputList " +
                "WHERE TransactionHash = @TransactionHash " +
                "AND IsDiscarded = 0 ";

            List<Input> result = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@TransactionHash", txHash);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    result = new List<Input>();

                    while (dr.Read())
                    {
                        Input input = new Input();

                        input.Id = GetDataValue<long>(dr, "Id");
                        input.TransactionHash = GetDataValue<string>(dr, "TransactionHash");
                        input.OutputTransactionHash = GetDataValue<string>(dr, "OutputTransactionHash");
                        input.OutputIndex = GetDataValue<int>(dr, "OutputIndex");
                        input.Size = GetDataValue<int>(dr, "Size");
                        input.Amount = GetDataValue<long>(dr, "Amount");
                        input.UnlockScript = GetDataValue<string>(dr, "UnlockScript");
                        input.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");
                        input.AccountId = GetDataValue<string>(dr, "AccountId");
                        input.BlockHash = GetDataValue<string>(dr, "BlockHash");

                        result.Add(input);
                    }
                }
            }

            return result;
        }

        public virtual List<Input> SelectFirstDiscardedByTransactionHash(string txHash)
        {
            const string SQL_STATEMENT =
                "SELECT * FROM InputList WHERE id IN ( " +
                "SELECT Id FROM InputList WHERE IsDiscarded = 1 AND TransactionHash = @TransactionHash " +
                "GROUP BY OutputIndex, OutputTransactionHash " +
                "HAVING COUNT(0) > 1)";

            List<Input> result = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@TransactionHash", txHash);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    result = new List<Input>();

                    while (dr.Read())
                    {
                        Input input = new Input();

                        input.Id = GetDataValue<long>(dr, "Id");
                        input.TransactionHash = GetDataValue<string>(dr, "TransactionHash");
                        input.OutputTransactionHash = GetDataValue<string>(dr, "OutputTransactionHash");
                        input.OutputIndex = GetDataValue<int>(dr, "OutputIndex");
                        input.Size = GetDataValue<int>(dr, "Size");
                        input.Amount = GetDataValue<long>(dr, "Amount");
                        input.UnlockScript = GetDataValue<string>(dr, "UnlockScript");
                        input.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");
                        input.AccountId = GetDataValue<string>(dr, "AccountId");
                        input.BlockHash = GetDataValue<string>(dr, "BlockHash");

                        result.Add(input);
                    }
                }
            }

            return result;
        }

        public virtual Input SelectById(long id)
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM InputList " +
                "WHERE Id = @Id " +
                "AND IsDiscarded = 0 ";

            Input input = null;

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
                        input = new Input();

                        input.Id = GetDataValue<long>(dr, "Id");
                        input.TransactionHash = GetDataValue<string>(dr, "TransactionHash");
                        input.OutputTransactionHash = GetDataValue<string>(dr, "OutputTransactionHash");
                        input.OutputIndex = GetDataValue<int>(dr, "OutputIndex");
                        input.Size = GetDataValue<int>(dr, "Size");
                        input.Amount = GetDataValue<long>(dr, "Amount");
                        input.UnlockScript = GetDataValue<string>(dr, "UnlockScript");
                        input.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");
                        input.AccountId = GetDataValue<string>(dr, "AccountId");
                        input.BlockHash = GetDataValue<string>(dr, "BlockHash");
                    }
                }

                return input;
            }
        }
        public virtual List<Input> SelectByOutputHash(string outputHash, int outputIndex)
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM InputList " +
                "WHERE OutputTransactionHash = @OutputTransactionHash AND OutputIndex = @OutputIndex " +
                "AND IsDiscarded = 0 ";

            List<Input> items = new List<Input>();

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@OutputTransactionHash", outputHash);
                cmd.Parameters.AddWithValue("@OutputIndex", outputIndex);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        var input = new Input();

                        input.Id = GetDataValue<long>(dr, "Id");
                        input.TransactionHash = GetDataValue<string>(dr, "TransactionHash");
                        input.OutputTransactionHash = GetDataValue<string>(dr, "OutputTransactionHash");
                        input.OutputIndex = GetDataValue<int>(dr, "OutputIndex");
                        input.Size = GetDataValue<int>(dr, "Size");
                        input.Amount = GetDataValue<long>(dr, "Amount");
                        input.UnlockScript = GetDataValue<string>(dr, "UnlockScript");
                        input.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");
                        input.AccountId = GetDataValue<string>(dr, "AccountId");
                        input.BlockHash = GetDataValue<string>(dr, "BlockHash");

                        items.Add(input);
                    }
                }

                return items;
            }
        }

        public virtual List<string> GetCostUtxos()
        {
            const string SQL_STATEMENT = "select inputlist.OutputTransactionHash,inputlist.OutputIndex from inputlist inner join outputlist on inputlist.OutputTransactionHash = outputlist.transactionhash and inputlist.Outputindex = outputlist.[index]";

            List<string> costUtxos = new List<string>();

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var key = GetDataValue<string>(dr, "OutputTransactionHash");
                        var value = GetDataValue<int>(dr, "OutputIndex");

                        costUtxos.Add($"{key }_{value }");
                    }
                }
            }
            return costUtxos;
        }

        public virtual long SelectTotalAmount(IEnumerable<string> hashIndexs)
        {
            var hi = $"('{string.Join("','", hashIndexs)}')";

            var SQL_STATEMENT = "select sum(Amount) as TotalAmount from outputlist where TransactionHash||[Index] in " + hi;
            
            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        if (dr.IsDBNull(0))
                            return 0;
                        else
                            return GetDataValue<long>(dr, "TotalAmount");
                    }
                }
            }
            return 0;
        }

        public virtual int SelectUnSpentCount(IEnumerable<string> hashIndexs)
        {
            var hi = $"('{string.Join("','", hashIndexs)}')";

            var SQL_STATEMENT = "select count(Amount) as TotalCount from outputlist join blocks on outputlist.BlockHash = blocks.Hash and blocks.IsDiscarded=0 "
                              + "where outputlist.TransactionHash || outputlist.[Index] in " + hi +
                                " and (select count(*) from inputList where OutputTransactionHash = TransactionHash and OutputIndex = [index]) =0";

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        if (dr.IsDBNull(0))
                            return 0;
                        else
                            return GetDataValue<int>(dr, "TotalCount");
                    }
                }
            }
            return 0;
        }
    }
}
