using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace FiiiChain.Data 
{
    public static class PragmaExtension
    {
        public static void SynchronousNORMAL(this SqliteConnection con)
        {            
            //PRAGMA schema.synchronous = 0 | OFF | 1 | NORMAL | 2 | FULL | 3 | EXTRA;
            //PRAGMA schema.cache_size = pages;
            var sql = $@"pragma synchronous =1;";
            using (SqliteCommand cmdAtt = new SqliteCommand(sql, con))
            {
                if (con.State != ConnectionState.Open)
                {
                    cmdAtt.Connection.Open();
                }
                cmdAtt.ExecuteNonQuery();
            }
        }
    }
}
