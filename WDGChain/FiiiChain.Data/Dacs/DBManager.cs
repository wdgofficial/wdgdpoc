// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;
using System.Data;
using FiiiChain.Framework;

namespace FiiiChain.Data
{
    public class DBManager
    {
        public static void Initialization()
        {
            var sql = Resource.InitScript;

            using (SqliteConnection con = new SqliteConnection(GlobalParameters.IsTestnet ?
                Resource.TestnetConnectionString : Resource.MainnetConnectionString))
            {
                con.Open();

                #region db patch 1
                bool needPatch1 = false;
                var indexCheckSql = "PRAGMA INDEX_LIST('InputList');";
                using (SqliteCommand cmd = new SqliteCommand(indexCheckSql, con))
                {
                    using (SqliteDataReader dr = cmd.ExecuteReader())
                    {
                        bool dbExisted = false;
                        bool patch1Existed = false;

                        while (dr.Read())
                        {
                            int i = dr.GetOrdinal("name");
                            if (!dr.IsDBNull(i))
                            {
                                if (dr.GetFieldValue<string>(i) == "InputList_BlockHash")
                                {
                                    patch1Existed = true;
                                    break;
                                }
                                else if (dr.GetFieldValue<string>(i) == "InputListAcountId")
                                {
                                    dbExisted = true;
                                }
                            }
                        }

                        if(dbExisted && !patch1Existed)
                        {
                            needPatch1 = true;
                        }
                    }
                }

                if (needPatch1)
                {
                    LogHelper.Info("Start to install patch 1");
                    using (SqliteCommand cmd = new SqliteCommand(Resource.Patch1Script, con))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    LogHelper.Info("Patch 1 installed");
                }
                #endregion


                using (SqliteCommand cmd = new SqliteCommand(sql, con))
                {
                    cmd.ExecuteNonQuery();
                }

                #region db patch 2
                if (!GlobalParameters.IsTestnet)
                {
                    LogHelper.Info("Start to install patch 2");
                    using (SqliteCommand cmd = new SqliteCommand(Resource.Patch2Script, con))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    LogHelper.Info("Patch 2 installed");

                }
                #endregion

                #region db patch 3
                if (!GlobalParameters.IsTestnet)
                {
                    LogHelper.Info("Start to install patch 3");
                    using (SqliteCommand cmd = new SqliteCommand(Resource.Patch3Script, con))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    LogHelper.Info("Patch 3 installed");

                }
                #endregion

                #region db patch 4
                if (!GlobalParameters.IsTestnet)
                {
                    LogHelper.Info("Start to install patch 4");
                    using (SqliteCommand cmd = new SqliteCommand(Resource.Patch4Script, con))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    LogHelper.Info("Patch 4 installed");

                }
                #endregion

                //if(DateTime.Now <= DateTime.Parse("2018/08/30"))
                //{
                //    var sql2 = "DROP INDEX TxHash; " +
                //        "CREATE INDEX TxHash ON Transactions ( " +
                //        "    Hash " +
                //        "); " +
                //        "DROP INDEX InputListUniqueIndex; " +
                //        "CREATE INDEX InputListUniqueIndex ON InputList ( " +
                //        "    TransactionHash " +
                //        "); " +
                //        "DROP INDEX OutputListUniqueIndex; " +
                //        "CREATE INDEX OutputListUniqueIndex ON OutputList ( " +
                //        "    \"Index\", " +
                //        "    TransactionHash " +
                //        ");";

                //    using (SqliteCommand cmd = new SqliteCommand(sql2, con))
                //    {
                //        cmd.ExecuteNonQuery();
                //    }
                //}

                #region db patch 5
                //if (!GlobalParameters.IsTestnet)
                //{
                //    LogHelper.Info("Start to install patch 5");
                //    using (SqliteCommand cmd = new SqliteCommand(Resource.Patch5Script, con))
                //    {
                //        cmd.ExecuteNonQuery();
                //    }

                //    LogHelper.Info("Patch 5 installed");

                //}
                #endregion
            }
        }
    }
}
