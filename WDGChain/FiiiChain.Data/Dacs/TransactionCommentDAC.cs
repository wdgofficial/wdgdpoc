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
    public class TransactionCommentDac : DataAccessComponent<TransactionCommentDac>
    {
        public virtual long Insert(TransactionComment comment)
        {
            const string SQL_STATEMENT =
                "INSERT INTO TransactionComments " +
                "(TransactionHash, OutputIndex, Comment, Timestamp) " +
                "VALUES (@TransactionHash, @OutputIndex, @Comment, @Timestamp);" +
                "SELECT LAST_INSERT_ROWID() newid;";

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@TransactionHash", comment.TransactionHash);
                cmd.Parameters.AddWithValue("@OutputIndex", comment.OutputIndex);
                cmd.Parameters.AddWithValue("@Timestamp", comment.Timestamp);

                if (string.IsNullOrWhiteSpace(comment.Comment))
                {
                    cmd.Parameters.AddWithValue("@Comment", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Comment", comment.Comment);
                }

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                return Convert.ToInt64(cmd.ExecuteScalar());
            }

        }

        public virtual TransactionComment SelectByTransactionHashAndIndex(string txHash, int outputIndex)
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM TransactionComments " +
                "WHERE TransactionHash = @TransactionHash AND OutputIndex = @OutputIndex";

            TransactionComment tc = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@TransactionHash", txHash);
                cmd.Parameters.AddWithValue("@OutputIndex", outputIndex);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        tc = new TransactionComment();

                        tc.Id = GetDataValue<long>(dr, "Id");
                        tc.TransactionHash = GetDataValue<string>(dr, "TransactionHash");
                        tc.OutputIndex = GetDataValue<int>(dr, "OutputIndex");
                        tc.Comment = GetDataValue<string>(dr, "Comment");
                        tc.Timestamp = GetDataValue<long>(dr, "Timestamp");
                    }
                }

                return tc;
            }
        }
        public virtual List<TransactionComment> SelectByTransactionHash(string txHash)
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM TransactionComments " +
                "WHERE TransactionHash = @TransactionHash;";


            List<TransactionComment> result = new List<TransactionComment>();
            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@TransactionHash", txHash);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var tc = new TransactionComment();

                        tc.Id = GetDataValue<long>(dr, "Id");
                        tc.TransactionHash = GetDataValue<string>(dr, "TransactionHash");
                        tc.OutputIndex = GetDataValue<int>(dr, "OutputIndex");
                        tc.Comment = GetDataValue<string>(dr, "Comment");
                        tc.Timestamp = GetDataValue<long>(dr, "Timestamp");

                        result.Add(tc);
                    }
                }

                return result;
            }
        }

        public virtual List<TransactionComment> SelectAll()
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM TransactionComments;";

            List<TransactionComment> result = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    result = new List<TransactionComment>();

                    while (dr.Read())
                    {
                        var tc = new TransactionComment();

                        tc.Id = GetDataValue<long>(dr, "Id");
                        tc.TransactionHash = GetDataValue<string>(dr, "TransactionHash");
                        tc.OutputIndex = GetDataValue<int>(dr, "OutputIndex");
                        tc.Comment = GetDataValue<string>(dr, "Comment");
                        tc.Timestamp = GetDataValue<long>(dr, "Timestamp");

                        result.Add(tc);
                    }
                }
            }

            return result;
        }

        public virtual string SelectComment(string txHash, int outputIndex)
        {
            const string SQL_STATEMENT =
                "SELECT Comment " +
                "FROM TransactionComments " +
                "WHERE TransactionHash = @TransactionHash AND OutputIndex = @OutputIndex";

            string comment = "";

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@TransactionHash", txHash);
                cmd.Parameters.AddWithValue("@OutputIndex", outputIndex);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        comment = GetDataValue<string>(dr, "Comment");
                    }
                }

                return comment;
            }
        }
    }
}
