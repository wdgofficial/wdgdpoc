// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.Entities;
using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using FiiiChain.Framework;

namespace FiiiChain.Data
{
    public class AddressBookDac : DataAccessComponent<AddressBookDac>
    {
        public virtual void InsertOrUpdate(string address, string tag)
        {
            const string SQL_STATEMENT =
                "DELETE FROM AddressBook " +
                "WHERE Address = @Address;" +
                "INSERT INTO AddressBook " +
                "(Address, Tag, Timestamp) " +
                "VALUES (@Address, @Tag, @Timestamp) ";

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Address", address);

                if(tag == null)
                {
                    cmd.Parameters.AddWithValue("@Tag", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Tag", tag);
                }

                cmd.Parameters.AddWithValue("@Timestamp", Time.EpochTime);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                cmd.ExecuteNonQuery();
            }
        }

        public virtual void UpdateTimestamp(string address)
        {
            const string SQL_STATEMENT =
                "UPDATE AddressBook " +
                "SET Timestamp = @Timestamp " +
                "WHERE Address = @Address;";

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Address", address);
                cmd.Parameters.AddWithValue("@Timestamp", Time.EpochTime);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                cmd.ExecuteNonQuery();
            }
        }
        public virtual void Delete(string address)
        {
            const string SQL_STATEMENT =
                "DELETE FROM AddressBook " +
                "WHERE Address = @Address;";

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Address", address);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                cmd.ExecuteNonQuery();
            }
        }

        public virtual void DeleteByTag(string tag)
        {
            const string SQL_STATEMENT =
                "DELETE FROM AddressBook " +
                "WHERE Tag = @Tag;";

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Tag", tag);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                cmd.ExecuteNonQuery();
            }
        }
        public virtual void DeleteByIds(long[] ids)
        {
            string sql =
                "DELETE FROM AddressBook " +
                "WHERE Id In ({0});";

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(string.Format(sql, string.Join(",", ids)), con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                cmd.ExecuteNonQuery();
            }
        }

        public virtual List<AddressBookItem> SelectWholeAddressBook()
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM AddressBook;";

            List<AddressBookItem> result = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    result = new List<AddressBookItem>();

                    while (dr.Read())
                    {
                        AddressBookItem item = new AddressBookItem();
                        item.Id = GetDataValue<long>(dr, "Id");
                        item.Address = GetDataValue<string>(dr, "Address");
                        item.Tag = GetDataValue<string>(dr, "Tag");
                        item.Timestamp = GetDataValue<long>(dr, "Timestamp");

                        result.Add(item);
                    }
                }
            }

            return result;
        }
        public virtual List<AddressBookItem> SelectAddessListByTag(string tag)
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM AddressBook " +
                "WHERE Tag = @Tag;";

            List<AddressBookItem> result = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                if (tag == null)
                {
                    cmd.Parameters.AddWithValue("@Tag", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Tag", tag);
                }

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    result = new List<AddressBookItem>();

                    while (dr.Read())
                    {
                        AddressBookItem item = new AddressBookItem();
                        item.Id = GetDataValue<long>(dr, "Id");
                        item.Address = GetDataValue<string>(dr, "Address");
                        item.Tag = GetDataValue<string>(dr, "Tag");
                        item.Timestamp = GetDataValue<long>(dr, "Timestamp");

                        result.Add(item);
                    }
                }
            }

            return result;
        }

        public virtual AddressBookItem SelectByAddress(string address)
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM AddressBook " +
                "WHERE Address = @Address;";

            AddressBookItem item = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Address", address);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {

                    if (dr.Read())
                    {
                        item = new AddressBookItem();
                        item.Id = GetDataValue<long>(dr, "Id");
                        item.Address = GetDataValue<string>(dr, "Address");
                        item.Tag = GetDataValue<string>(dr, "Tag");
                        item.Timestamp = GetDataValue<long>(dr, "Timestamp");
                    }
                }
            }

            return item;
        }

        public virtual List<AddressBookItem> SelectAll()
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM AddressBook;";

            List<AddressBookItem> result = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    result = new List<AddressBookItem>();

                    while (dr.Read())
                    {
                        AddressBookItem item = new AddressBookItem();
                        item.Id = GetDataValue<long>(dr, "Id");
                        item.Address = GetDataValue<string>(dr, "Address");
                        item.Tag = GetDataValue<string>(dr, "Tag");
                        item.Timestamp = GetDataValue<long>(dr, "Timestamp");

                        result.Add(item);
                    }
                }
            }

            return result;
        }

        public virtual List<AddressBookItem> SelectByIds(long[] ids)
        {
            if (ids == null || ids.Length == 0)
                return null;

            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM AddressBook where id in ({0})";

            var condition = string.Join(",", ids);

            List<AddressBookItem> result = null;

            var sql = string.Format(SQL_STATEMENT, condition);
            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(sql, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    result = new List<AddressBookItem>();

                    while (dr.Read())
                    {
                        AddressBookItem item = new AddressBookItem();
                        item.Id = GetDataValue<long>(dr, "Id");
                        item.Address = GetDataValue<string>(dr, "Address");
                        item.Tag = GetDataValue<string>(dr, "Tag");
                        item.Timestamp = GetDataValue<long>(dr, "Timestamp");

                        result.Add(item);
                    }
                }
            }

            return result;
        }
    }
}
