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
    public class PaymentRequestDac : DataAccessComponent<PaymentRequestDac>
    {
        public virtual long Insert(PaymentRequest request)
        {
            const string SQL_STATEMENT =
                "INSERT INTO PaymentRequests " +
                "(AccountId, Tag, Comment, Amount, Timestamp) " +
                "VALUES (@AccountId, @Tag, @Comment, @Amount, @Timestamp);" +
                "SELECT LAST_INSERT_ROWID() newid;";

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@AccountId", request.AccountId);
                cmd.Parameters.AddWithValue("@Timestamp", request.Timestamp);
                cmd.Parameters.AddWithValue("@Amount", request.Amount);

                if (string.IsNullOrWhiteSpace(request.Tag))
                {
                    cmd.Parameters.AddWithValue("@Tag", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Tag", request.Tag);
                }

                if (string.IsNullOrWhiteSpace(request.Comment))
                {
                    cmd.Parameters.AddWithValue("@Comment", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Comment", request.Comment);
                }

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                return Convert.ToInt64(cmd.ExecuteScalar());
            }

        }

        public virtual void DeleteByIds(long[] ids)
        {
            string sql =
                "DELETE FROM PaymentRequests " +
                "WHERE Id In ({0});";

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(string.Format(sql, string.Join(",", ids)), con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                cmd.ExecuteNonQuery();
            }
        }

        public virtual List<PaymentRequest> SelectAll()
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM PaymentRequests " +
                "ORDER BY Timestamp DESC;";

            List<PaymentRequest> result = new List<PaymentRequest>();

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var pr = new PaymentRequest();

                        pr.Id = GetDataValue<long>(dr, "Id");
                        pr.AccountId = GetDataValue<string>(dr, "AccountId");
                        pr.Tag = GetDataValue<string>(dr, "Tag");
                        pr.Comment = GetDataValue<string>(dr, "Comment");
                        pr.Amount = GetDataValue<long>(dr, "Amount");
                        pr.Timestamp = GetDataValue<long>(dr, "Timestamp");

                        result.Add(pr);
                    }
                }

                return result;
            }
        }
        public virtual PaymentRequest SelectById(long id)
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM PaymentRequests " +
                "WHERE Id = @Id;";

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
                        var pr = new PaymentRequest();

                        pr.Id = GetDataValue<long>(dr, "Id");
                        pr.AccountId = GetDataValue<string>(dr, "AccountId");
                        pr.Tag = GetDataValue<string>(dr, "Tag");
                        pr.Comment = GetDataValue<string>(dr, "Comment");
                        pr.Amount = GetDataValue<long>(dr, "Amount");
                        pr.Timestamp = GetDataValue<long>(dr, "Timestamp");

                        return pr;
                    }
                }
            }

            return null;
        }
    }
}
