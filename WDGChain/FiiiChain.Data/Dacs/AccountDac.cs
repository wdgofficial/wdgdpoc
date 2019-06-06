// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.Entities;
using FiiiChain.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Text;

namespace FiiiChain.Data
{
    public class AccountDac : DataAccessComponent<AccountDac>
    {
        public static Action<string> InsertAccountEvent;

        public virtual void Insert(Account account)
        {
            const string SQL_STATEMENT =
                "INSERT INTO Accounts " +
                "(Id, PrivateKey, PublicKey, Balance, IsDefault, WatchedOnly, Timestamp, Tag) " +
                "VALUES (@Id, @PrivateKey, @PublicKey, @Balance, @IsDefault, @WatchedOnly, @Timestamp, @Tag);";

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Id", account.Id);
                cmd.Parameters.AddWithValue("@PrivateKey", account.PrivateKey);
                cmd.Parameters.AddWithValue("@PublicKey", account.PublicKey);
                cmd.Parameters.AddWithValue("@Balance", account.Balance);
                cmd.Parameters.AddWithValue("@IsDefault", account.IsDefault);
                cmd.Parameters.AddWithValue("@WatchedOnly", account.WatchedOnly);
                cmd.Parameters.AddWithValue("@Timestamp", Time.EpochTime);

                if (account.Tag == null)
                {
                    cmd.Parameters.AddWithValue("@Tag", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Tag", account.Tag);
                }

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                cmd.ExecuteNonQuery();
            }

            InsertAccountEvent?.Invoke(account.Id);
        }

        public virtual void UpdateBalance(string id, long amount)
        {
            const string SQL_STATEMENT =
                "UPDATE Accounts " +
                "SET Balance = (Balance +  @Amount) " +
                "WHERE Id = @Id;";

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                cmd.ExecuteNonQuery();
            }
        }

        public virtual void UpdateTag(string id, string tag)
        {
            const string SQL_STATEMENT =
                "UPDATE Accounts " +
                "SET Tag = @Tag " +
                "WHERE Id = @Id;";

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Id", id);

                if(string.IsNullOrWhiteSpace(tag))
                {
                    cmd.Parameters.AddWithValue("@Tag", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Tag", tag);
                }

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                cmd.ExecuteNonQuery();
            }
        }

        public virtual int UpdatePrivateKeyAr(List<Account> aclist)
        {
            StringBuilder SQL_STATEMENT = new StringBuilder("BEGIN TRANSACTION;");
            foreach(var item in aclist)
            {
                SQL_STATEMENT.Append($"UPDATE Accounts SET PrivateKey = '{item.PrivateKey}' WHERE Id = '{item.Id}';");
            }
            SQL_STATEMENT.Append("END TRANSACTION;");
            int rows = -1;
            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT.ToString(), con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                rows = cmd.ExecuteNonQuery();
            }
            return rows;
        }

        public void SetDefaultAccount(string id)
        {
            const string SQL_STATEMENT =
                "UPDATE Accounts " +
                "SET IsDefault = 1 " +
                "WHERE Id = @Id;";

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                cmd.ExecuteNonQuery();
            }
        }

        public virtual void Delete(string id)
        {
            const string SQL_STATEMENT =
                "DELETE FROM Accounts " +
                "WHERE Id = @Id;";

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                cmd.ExecuteNonQuery();
            }
        }

        public virtual List<Account> SelectAll()
        {
            const string SQL_STATEMENT =
                "SELECT Id, PrivateKey, PublicKey, Balance, IsDefault, WatchedOnly, Timestamp, Tag " +
                "FROM Accounts;";

            List<Account> result = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    result = new List<Account>();

                    while (dr.Read())
                    {
                        Account account = new Account();
                        account.Id = dr.GetString(0);
                        account.PrivateKey = dr.GetString(1);
                        account.PublicKey = dr.GetString(2);
                        account.Balance = dr.GetInt64(3);
                        account.IsDefault = dr.GetBoolean(4);
                        account.WatchedOnly = dr.GetBoolean(5);
                        account.Timestamp = dr.GetInt64(6);
                        account.Tag = dr.IsDBNull(7) ? "" : dr.GetString(7);

                        result.Add(account);
                    }
                }
            }

            return result;
        }

        public virtual List<Account> GetAccountCategory(int category)
        {
            string SQL_STATEMENT = "";
            switch (category)
            {
                case 1:
                    SQL_STATEMENT = "SELECT * " +
                        "FROM Accounts " +
                        "WHERE IsDefault = 0 ORDER BY Timestamp;";
                    break;
                case 2:
                    SQL_STATEMENT = "SELECT * " +
                        "FROM Accounts " +
                        "WHERE IsDefault = 1 ORDER BY Timestamp;";
                    break;
                case 3:
                    SQL_STATEMENT = "SELECT * " +
                        "FROM Accounts " +
                        "WHERE PrivateKey IS NULL ORDER BY Timestamp;";
                    break;
                default:
                    SQL_STATEMENT = "SELECT * " +
                        "FROM Accounts " +
                        "ORDER BY Timestamp;";
                    break;
            }
            List<Account> result = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    result = new List<Account>();

                    while (dr.Read())
                    {
                        Account account = new Account();
                        account.Id = GetDataValue<string>(dr, "Id");
                        account.PrivateKey = GetDataValue<string>(dr, "PrivateKey");
                        account.PublicKey = GetDataValue<string>(dr, "PublicKey");
                        account.Balance = GetDataValue<long>(dr, "Balance");
                        account.IsDefault = GetDataValue<bool>(dr, "IsDefault");
                        account.WatchedOnly = GetDataValue<bool>(dr, "WatchedOnly");
                        account.Timestamp = GetDataValue<long>(dr, "Timestamp");
                        account.Tag = GetDataValue<string>(dr, "Tag");

                        result.Add(account);
                    }
                }
            }

            return result;
        }

        public virtual Account SelectById(string id)
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM Accounts " + 
                "WHERE Id = @Id;";

            Account account = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        account = new Account();
                        account.Id = GetDataValue<string>(dr, "Id");
                        account.PrivateKey = GetDataValue<string>(dr, "PrivateKey");
                        account.PublicKey = GetDataValue<string>(dr, "PublicKey");
                        account.Balance = GetDataValue<long>(dr, "Balance");
                        account.IsDefault = GetDataValue<bool>(dr, "IsDefault");
                        account.WatchedOnly = GetDataValue<bool>(dr, "WatchedOnly");
                        account.Timestamp = GetDataValue<long>(dr, "Timestamp");
                        account.Tag = GetDataValue<string>(dr, "Tag");
                    }
                }
            }

            return account;
        }

        public virtual List<Account> SelectByTag(string tag)
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM Accounts " +
                "WHERE IFNULL(Tag,'') LIKE @Tag AND  IsDefault = 1;";

            List<Account> result = new List<Account>();

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                if(tag == null || tag == "*")
                {
                    tag = "";
                }

                tag = "%" + tag + "%";

                cmd.Parameters.AddWithValue("@Tag", tag);
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var account = new Account();
                        account.Id = GetDataValue<string>(dr, "Id");
                        account.PrivateKey = GetDataValue<string>(dr, "PrivateKey");
                        account.PublicKey = GetDataValue<string>(dr, "PublicKey");
                        account.Balance = GetDataValue<long>(dr, "Balance");
                        account.IsDefault = GetDataValue<bool>(dr, "IsDefault");
                        account.WatchedOnly = GetDataValue<bool>(dr, "WatchedOnly");
                        account.Timestamp = GetDataValue<long>(dr, "Timestamp");
                        account.Tag = GetDataValue<string>(dr, "Tag");

                        result.Add(account);
                    }
                }
            }

            return result;
        }

        public virtual Account SelectFirstDefaultAccount()
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM Accounts " +
                "ORDER BY Timestamp LIMIT 1;";

            Account account = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        account = new Account();
                        account.Id = GetDataValue<string>(dr, "Id");
                        account.PrivateKey = GetDataValue<string>(dr, "PrivateKey");
                        account.PublicKey = GetDataValue<string>(dr, "PublicKey");
                        account.Balance = GetDataValue<long>(dr, "Balance");
                        account.IsDefault = GetDataValue<bool>(dr, "IsDefault");
                        account.WatchedOnly = GetDataValue<bool>(dr, "WatchedOnly");
                        account.Timestamp = GetDataValue<long>(dr, "Timestamp");
                        account.Tag = GetDataValue<string>(dr, "Tag");
                    }
                }
            }

            return account;
        }

        public virtual List<Account> SelectAllDefaultAccounts()
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM Accounts " +
                "WHERE IsDefault = 1 ORDER BY Timestamp;";

            List<Account> items = new List<Account>();

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var account = new Account();
                        account.Id = GetDataValue<string>(dr, "Id");
                        account.PrivateKey = GetDataValue<string>(dr, "PrivateKey");
                        account.PublicKey = GetDataValue<string>(dr, "PublicKey");
                        account.Balance = GetDataValue<long>(dr, "Balance");
                        account.IsDefault = GetDataValue<bool>(dr, "IsDefault");
                        account.WatchedOnly = GetDataValue<bool>(dr, "WatchedOnly");
                        account.Timestamp = GetDataValue<long>(dr, "Timestamp");
                        account.Tag = GetDataValue<string>(dr, "Tag");

                        items.Add(account);
                    }
                }
            }

            return items;
        }

        public virtual bool IsExisted(string id)
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM Accounts " +
                "WHERE Id = @Id LIMIT 1;";

            bool hasAccount = false;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Id", id);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    hasAccount = dr.HasRows;
                }
            }

            return hasAccount;
        }
    }
}
