// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.Entities;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiiiChain.Data.Dacs
{
    public class UserSettingDac : DataAccessComponent<UserSettingDac>
    {
        public virtual List<UserSetting> SelectAll()
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM UserSetting ";

            List<UserSetting> result = null;

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    result = new List<UserSetting>();

                    while (dr.Read())
                    {
                        var userSetting = new UserSetting();

                        userSetting.Id = GetDataValue<int>(dr, "Id");
                        userSetting.Type = (UserSettingType)GetDataValue<int>(dr, "Type");
                        userSetting.Value = GetDataValue<string>(dr, "Value");
                        result.Add(userSetting);
                    }
                }

                return result;
            }
        }

        public virtual UserSetting SelectByType(UserSettingType settingType)
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM UserSetting Where Type = '@Type'";

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Type", (int)settingType);
                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        var result = new UserSetting();
                        result.Id = GetDataValue<int>(dr, "Id");
                        result.Type = (UserSettingType)GetDataValue<int>(dr, "Type");
                        result.Value = GetDataValue<string>(dr, "Value");
                        return result;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public virtual void Upsert(UserSetting userSetting)
        {
            var settings = SelectAll();

            if (settings.Any(x => x.Type == userSetting.Type))
            {
                userSetting.Id = settings.FirstOrDefault(x => x.Type == userSetting.Type).Id;
                Update(userSetting);
            }
            else
            {
                Insert(userSetting);
            }
        }

        private void Insert(UserSetting userSetting)
        {
            const string SQL_STATEMENT =
                "INSERT INTO UserSetting " +
                "([Type], [Value]) " +
                "VALUES (@Type, @Value); ";

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Type", (int)userSetting.Type);
                cmd.Parameters.AddWithValue("@Value", userSetting.Value);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                cmd.ExecuteNonQuery();
            }
        }

        private void Update(UserSetting userSetting)
        {
            const string SQL_STATEMENT =
                "UPDATE [UserSetting] " +
                "Set Value = @Value " +
                "Where [Id] = @Id; ";

            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT, con))
            {
                cmd.Parameters.AddWithValue("@Id", userSetting.Id);
                cmd.Parameters.AddWithValue("@Value", userSetting.Value);

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
