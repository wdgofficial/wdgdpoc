// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.Framework;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;

namespace FiiiChain.Data
{
    public class SqliteHelper
    {
        public static string SqliteConnString
        {
            get
            {
                return GlobalParameters.IsTestnet ? Resource.TestnetConnectionString : Resource.MainnetConnectionString;
            }
        }

        private static readonly object obj = new object();

        public static int ExecuteNonQuery(string commandText, params SqliteParameter[][] parametersList)
        {
            int result = 0;
            if (commandText == null || commandText.Length == 0)
                throw new ArgumentNullException("Sql Command Text is Null");

            using (SqliteConnection con = new SqliteConnection(SqliteConnString))
            {
                con.Open();

                //con.SynchronousNORMAL();

                var cmd = con.CreateCommand();
                cmd.Connection = con;
                cmd.CommandText = commandText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(cmd.CreateParameter());

                var trans = con.BeginTransaction();

                try
                {
                    foreach (var item in parametersList)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddRange(item);
                        cmd.Transaction = trans;
                        result += cmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
            return result;
        }
    }
}