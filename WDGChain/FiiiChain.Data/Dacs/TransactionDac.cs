// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.Entities;
using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Linq;

namespace FiiiChain.Data
{
    public class TransactionDac : DataAccessComponent<TransactionDac>
    {
        public virtual long Insert(Transaction transaction)
        {
            const string SQL_STATEMENT =
                "INSERT INTO Transactions " +
                "(Hash, BlockHash, Version, Timestamp, LockTime, TotalInput, TotalOutput, Fee, Size, IsDiscarded) " +
                "VALUES (@Hash, @BlockHash, @Version, @Timestamp, @LockTime, @TotalInput, @TotalOutput, @Fee, @Size, @IsDiscarded);" +
                "SELECT LAST_INSERT_ROWID() newid;";

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Hash", transaction.Hash);
                cmd.Parameters.AddWithValue("@BlockHash", transaction.BlockHash);
                cmd.Parameters.AddWithValue("@Version", transaction.Version);
                cmd.Parameters.AddWithValue("@Timestamp", transaction.Timestamp);
                cmd.Parameters.AddWithValue("@LockTime", transaction.LockTime);
                cmd.Parameters.AddWithValue("@TotalInput", transaction.TotalInput);
                cmd.Parameters.AddWithValue("@TotalOutput", transaction.TotalOutput);
                cmd.Parameters.AddWithValue("@Fee", transaction.Fee);
                cmd.Parameters.AddWithValue("@Size", transaction.Size);
                cmd.Parameters.AddWithValue("@IsDiscarded", transaction.IsDiscarded);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                return Convert.ToInt64(cmd.ExecuteScalar());
            }
        }

        public virtual List<Transaction> SelectByBlockHash(string blockHash)
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM Transactions " +
                "WHERE BlockHash = @BlockHash " +
                "AND IsDiscarded = 0 ";

            List<Transaction> result = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@BlockHash", blockHash);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    result = new List<Transaction>();

                    while (dr.Read())
                    {
                        Transaction transaction = new Transaction();

                        transaction.Id = GetDataValue<long>(dr, "Id");
                        transaction.Hash = GetDataValue<string>(dr, "Hash");
                        transaction.BlockHash = GetDataValue<string>(dr, "BlockHash");
                        transaction.Version = GetDataValue<int>(dr, "Version");
                        transaction.Timestamp = GetDataValue<long>(dr, "Timestamp");
                        transaction.LockTime = GetDataValue<long>(dr, "LockTime");
                        transaction.TotalInput = GetDataValue<long>(dr, "TotalInput");
                        transaction.TotalOutput = GetDataValue<long>(dr, "TotalOutput");
                        transaction.Size = GetDataValue<int>(dr, "Size");
                        transaction.Fee = GetDataValue<long>(dr, "Fee");
                        transaction.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");

                        result.Add(transaction);
                    }
                }
            }

            return result;
        }

        public virtual IList<Transaction> Select()
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM Transactions " +
                "WHERE IsDiscarded = 0 ";

            List<Transaction> result = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    result = new List<Transaction>();

                    while (dr.Read())
                    {
                        Transaction transaction = new Transaction();

                        transaction.Id = GetDataValue<long>(dr, "Id");
                        transaction.Hash = GetDataValue<string>(dr, "Hash");
                        transaction.BlockHash = GetDataValue<string>(dr, "BlockHash");
                        transaction.Version = GetDataValue<int>(dr, "Version");
                        transaction.Timestamp = GetDataValue<long>(dr, "Timestamp");
                        transaction.LockTime = GetDataValue<long>(dr, "LockTime");
                        transaction.TotalInput = GetDataValue<long>(dr, "TotalInput");
                        transaction.TotalOutput = GetDataValue<long>(dr, "TotalOutput");
                        transaction.Size = GetDataValue<int>(dr, "Size");
                        transaction.Fee = GetDataValue<long>(dr, "Fee");
                        transaction.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");

                        result.Add(transaction);
                    }
                }
            }

            return result;
        }

        public virtual Transaction SelectById(long id)
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM Transactions " +
                "WHERE Id = @Id " +
                "AND IsDiscarded = 0 ";

            Transaction transaction = null;

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
                        transaction = new Transaction();

                        transaction.Id = GetDataValue<long>(dr, "Id");
                        transaction.Hash = GetDataValue<string>(dr, "Hash");
                        transaction.BlockHash = GetDataValue<string>(dr, "BlockHash");
                        transaction.Version = GetDataValue<int>(dr, "Version");
                        transaction.Timestamp = GetDataValue<long>(dr, "Timestamp");
                        transaction.LockTime = GetDataValue<long>(dr, "LockTime");
                        transaction.TotalInput = GetDataValue<long>(dr, "TotalInput");
                        transaction.TotalOutput = GetDataValue<long>(dr, "TotalOutput");
                        transaction.Size = GetDataValue<int>(dr, "Size");
                        transaction.Fee = GetDataValue<long>(dr, "Fee");
                        transaction.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");
                    }
                }
            }

            return transaction;
        }

        public virtual bool HasTransaction(long transactionId)
        {
            const string SQL_STATEMENT =
                "SELECT 1 " +
                "FROM Transactions " +
                "WHERE IsDiscarded = 0 " +
                "AND Id = @Id LIMIT 1 ";

            bool hasTransaction = false;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Id", transactionId);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    hasTransaction = dr.HasRows;
                }
            }

            return hasTransaction;
        }

        public virtual bool HasTransactionByHash(string hash)
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM Transactions " +
                "WHERE IsDiscarded = 0 " +
                "AND Hash = @Hash " +
                "AND BlockHash IN (SELECT HASH FROM Blocks WHERE IsVerified = 1) " +
                " LIMIT 1 ";

            bool hasTransaction = false;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Hash", hash);
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    hasTransaction = dr.HasRows;
                }
            }

            return hasTransaction;
        }

        public virtual int Count()
        {
            const string SQL_STATEMENT =
                "SELECT COUNT(0) " +
                "FROM Transactions " +
                "WHERE IsDiscarded = 0 ";

            int result = 0;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                result = (int)cmd.ExecuteScalar();
            }

            return result;
        }

        public virtual Transaction SelectByHash(string hash)
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM Transactions " +
                "WHERE Hash = @Hash " +
                "AND IsDiscarded = 0 ";

            Transaction transaction = null;

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
                        transaction = new Transaction();

                        transaction.Id = GetDataValue<long>(dr, "Id");
                        transaction.Hash = GetDataValue<string>(dr, "Hash");
                        transaction.BlockHash = GetDataValue<string>(dr, "BlockHash");
                        transaction.Version = GetDataValue<int>(dr, "Version");
                        transaction.Timestamp = GetDataValue<long>(dr, "Timestamp");
                        transaction.LockTime = GetDataValue<long>(dr, "LockTime");
                        transaction.TotalInput = GetDataValue<long>(dr, "TotalInput");
                        transaction.TotalOutput = GetDataValue<long>(dr, "TotalOutput");
                        transaction.Size = GetDataValue<int>(dr, "Size");
                        transaction.Fee = GetDataValue<long>(dr, "Fee");
                        transaction.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");
                    }
                }
            }

            return transaction;
        }
        
        public virtual bool TransactionHashExist(string hash)
        {
            const string SQL_STATEMENT =
                "SELECT Hash " +
                "FROM Transactions " +
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
        
        public virtual List<Transaction> SelectTransactionsContainUnspentUTXO()
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM Transactions " +
                "WHERE Hash IN ( " +
                "SELECT TransactionHash FROM OutputList " +
                "WHERE Spent = 0 AND ReceiverId IN " +
                "(SELECT Id FROM Accounts WHERE PrivateKey IS NOT NULL)) " +
                "AND IsDiscarded = 0;";
            List<Transaction> result = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            {
                using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
                {
                    cmd.Connection.Open();
                    //con.SynchronousNORMAL();
                    using (SqliteDataReader dr = cmd.ExecuteReader())
                    {
                        result = new List<Transaction>();

                        while (dr.Read())
                        {
                            Transaction transaction = new Transaction();

                            transaction.Id = GetDataValue<long>(dr, "Id");
                            transaction.Hash = GetDataValue<string>(dr, "Hash");
                            transaction.BlockHash = GetDataValue<string>(dr, "BlockHash");
                            transaction.Version = GetDataValue<int>(dr, "Version");
                            transaction.Timestamp = GetDataValue<long>(dr, "Timestamp");
                            transaction.LockTime = GetDataValue<long>(dr, "LockTime");
                            transaction.TotalInput = GetDataValue<long>(dr, "TotalInput");
                            transaction.TotalOutput = GetDataValue<long>(dr, "TotalOutput");
                            transaction.Size = GetDataValue<int>(dr, "Size");
                            transaction.Fee = GetDataValue<long>(dr, "Fee");
                            transaction.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");

                            result.Add(transaction);
                        }
                    }
                }
            }

            return result;
        }

        public virtual long CountSelfUnspentTransactions()
        {
            const string SQL_STATEMENT =
                "SELECT COUNT(0) " +
                "FROM Transactions " +
                "WHERE Hash IN ( " +
                "SELECT TransactionHash FROM OutputList " +
                "WHERE Spent = 0 AND ReceiverId IN " +
                "(SELECT Id FROM Accounts WHERE PrivateKey IS NOT NULL)) " +
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

        public virtual List<Transaction> SelectTransactions(string accountTag, int skip, int takeCount, bool includeWatchOnly)
        {
            string sql =
                "SELECT * FROM Transactions WHERE Hash in " +
                "(SELECT TransactionHash FROM InputList WHERE AccountId in " +
                "(SELECT Id FROM Accounts {0})" +
                "UNION " +
                "SELECT TransactionHash FROM OutputList WHERE ReceiverId in " +
                "(SELECT Id FROM Accounts {0})) " +
                "AND IsDiscarded = 0 " +
                "ORDER BY Timestamp DESC LIMIT {1},{2}";
            string condition = "";
            if (!includeWatchOnly)
            {
                condition = "WHERE PrivateKey IS NOT NULL ";
            }

            if (accountTag != "*")
            {
                if (condition == "")
                {
                    condition += "WHERE ";
                }
                else
                {
                    condition += "AND ";
                }

                if (string.IsNullOrEmpty(accountTag))
                {
                    condition += "(Tag IS NULL OR Tag = '') ";
                }
                else
                {
                    condition += "Tag LIKE @Tag";
                }
            }
            else
            {
                accountTag = "";
            }

            sql = string.Format(sql, condition, skip, takeCount);
            List<Transaction> result = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(sql, con))
            {
                if (!string.IsNullOrWhiteSpace(accountTag))
                {
                    cmd.Parameters.AddWithValue("@Tag", "%" + accountTag + "%");
                }

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    result = new List<Transaction>();

                    while (dr.Read())
                    {
                        Transaction transaction = new Transaction();

                        transaction.Id = GetDataValue<long>(dr, "Id");
                        transaction.Hash = GetDataValue<string>(dr, "Hash");
                        transaction.BlockHash = GetDataValue<string>(dr, "BlockHash");
                        transaction.Version = GetDataValue<int>(dr, "Version");
                        transaction.Timestamp = GetDataValue<long>(dr, "Timestamp");
                        transaction.LockTime = GetDataValue<long>(dr, "LockTime");
                        transaction.TotalInput = GetDataValue<long>(dr, "TotalInput");
                        transaction.TotalOutput = GetDataValue<long>(dr, "TotalOutput");
                        transaction.Size = GetDataValue<int>(dr, "Size");
                        transaction.Fee = GetDataValue<long>(dr, "Fee");
                        transaction.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");

                        result.Add(transaction);
                    }
                }
            }

            return result;
        }

        public virtual List<Transaction> SelectAll()
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM Transactions;";

            List<Transaction> result = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    result = new List<Transaction>();

                    while (dr.Read())
                    {
                        Transaction transaction = new Transaction();

                        transaction.Id = GetDataValue<long>(dr, "Id");
                        transaction.Hash = GetDataValue<string>(dr, "Hash");
                        transaction.BlockHash = GetDataValue<string>(dr, "BlockHash");
                        transaction.Version = GetDataValue<int>(dr, "Version");
                        transaction.Timestamp = GetDataValue<long>(dr, "Timestamp");
                        transaction.LockTime = GetDataValue<long>(dr, "LockTime");
                        transaction.TotalInput = GetDataValue<long>(dr, "TotalInput");
                        transaction.TotalOutput = GetDataValue<long>(dr, "TotalOutput");
                        transaction.Size = GetDataValue<int>(dr, "Size");
                        transaction.Fee = GetDataValue<long>(dr, "Fee");
                        transaction.IsDiscarded = GetDataValue<bool>(dr, "IsDiscarded");

                        result.Add(transaction);
                    }
                }
            }

            return result;
        }

        public virtual bool HasTransaction(string transactionId)
        {
            const string SQL_STATEMENT =
                "SELECT 1 " +
                "FROM Transactions " +
                "WHERE hash = @Hash " +
                " LIMIT 1 ";

            bool hasTransaction = false;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Hash", transactionId);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    hasTransaction = dr.HasRows;
                }
            }

            return hasTransaction;
        }

        public virtual bool HasCost(string transactionHash, int index)
        {
            const string SQL_STATEMENT =
                "SELECT 1 FROM transactions WHERE HASH = " +
                "(select TransactionHash from InputList " +
                "WHERE  " +
                "OutputTransactionHash = @Id and [OutputIndex] = @Index) LIMIT 1 ";

            bool hasTransaction = false;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Id", transactionHash);
                cmd.Parameters.AddWithValue("@Index", index);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    hasTransaction = dr.HasRows;
                }
            }

            return hasTransaction;
        }

        public virtual bool HasAnyCost(IEnumerable<string> hashWithIndex)
        {
            if (hashWithIndex == null || hashWithIndex.Count() == 0)
                return true;

            const string SQL_STATEMENT =
                "SELECT 1 FROM transactions WHERE HASH = " +
                "(select TransactionHash from InputList " +
                "WHERE  " +
                "OutputTransactionHash||OutputIndex = @HashWithIndex) LIMIT 1 ";

            bool hasTransaction = false;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@HashWithIndex", hashWithIndex);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    hasTransaction = dr.HasRows;
                }
            }

            return hasTransaction;
        }
    }
}