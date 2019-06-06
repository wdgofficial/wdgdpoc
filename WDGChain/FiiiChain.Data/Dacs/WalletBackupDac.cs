// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.Entities;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace FiiiChain.Data
{
    public class WalletBackupDac : DataAccessComponent<WalletBackupDac>
    {
        public virtual int Restore(WalletBackup entity)
        {
            int rows = 0;

            rows += RestoreSettings(entity);
            rows += RestoreTransactionCommentList(entity);
            rows += RestoreAddressBook(entity);
            rows += RestoreAccountList(entity);

            return rows;
        }

        public virtual int RestoreAddressBook(WalletBackup entity)
        {
            int rows = 0;
            var addressList = entity.AddressBookItemList;

            if (addressList == null || addressList.Count == 0)
                return rows;
            RestoreAddressBooks(addressList);

            return rows;
        }

        private int RestoreAddressBooks(IEnumerable<AddressBookItem> AddressBookItemList)
        {
            int rows = 0;

            var SQL = "REPLACE INTO AddressBook (Address, Tag, Timestamp) VALUES (@Address,@Tag,@Timestamp);";
            List<SqliteParameter[]> parametersList = new List<SqliteParameter[]>();
            foreach (var item in AddressBookItemList)
            {
                SqliteParameter[] parameters = new SqliteParameter[3];
                parameters[0] = new SqliteParameter("@Address", item.Address);
                parameters[1] = new SqliteParameter("@Tag", item.Tag);
                parameters[2] = new SqliteParameter("@Timestamp", item.Timestamp);

                parametersList.Add(parameters);
            }
            rows = SqliteHelper.ExecuteNonQuery(SQL, parametersList.ToArray());
            return rows;
        }

        public virtual int RestoreAccountList(WalletBackup entity)
        {
            int rows = 0;
            var accountList = entity.AccountList;

            if (accountList == null || accountList.Count == 0)
                return rows;

            rows = RestoreAccounts(accountList);

            return rows;
        }

        private int RestoreAccounts(IEnumerable<Account> AccountList)
        {
            int rows = 0;

            var SQL = "REPLACE INTO Accounts VALUES (@Id,@PrivateKey,@PublicKey,@Balance,@IsDefault,@WatchedOnly,@Timestamp,@Tag);";
            List<SqliteParameter[]> parametersList = new List<SqliteParameter[]>();
            foreach (var item in AccountList)
            {
                SqliteParameter[] parameters = new SqliteParameter[8];
                parameters[0] = new SqliteParameter("@Id", item.Id);
                parameters[1] = new SqliteParameter("@PrivateKey", item.PrivateKey);
                parameters[2] = new SqliteParameter("@PublicKey", item.PublicKey);
                parameters[3] = new SqliteParameter("@Balance", item.Balance);
                parameters[4] = new SqliteParameter("@IsDefault", Convert.ToInt32(item.IsDefault));
                parameters[5] = new SqliteParameter("@WatchedOnly", Convert.ToInt32(item.WatchedOnly));
                parameters[6] = new SqliteParameter("@Timestamp", item.Timestamp);
                parameters[7] = new SqliteParameter("@Tag", string.IsNullOrEmpty(item.Tag) ? DBNull.Value.ToString() : item.Tag);

                parametersList.Add(parameters);
            }
            rows = SqliteHelper.ExecuteNonQuery(SQL, parametersList.ToArray());
            return rows;
        }

        private int RestoreSettings(WalletBackup entity)
        {
            int rows = 0;
            StringBuilder SQL_STATEMENT = new StringBuilder("BEGIN TRANSACTION;");
            SQL_STATEMENT.Append("DELETE FROM Settings;");
            foreach (var item in entity.SettingList)
            {
                SQL_STATEMENT.Append($"INSERT INTO Settings (Confirmations, FeePerKB, Encrypt, PassCiphertext) VALUES({item.Confirmations},{item.FeePerKB}, {Convert.ToInt32(item.Encrypt)}, '{item.PassCiphertext}');");
            }
            SQL_STATEMENT.Append("END TRANSACTION;");
            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT.ToString(), con))
            {

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                rows = cmd.ExecuteNonQuery();
            }
            return rows;
        }

        private int RestoreTransactionCommentList(WalletBackup entity)
        {
            int rows = 0;
            StringBuilder SQL_STATEMENT = new StringBuilder("BEGIN TRANSACTION;");
            SQL_STATEMENT.Append("DELETE FROM TransactionComments;");
            foreach (var item in entity.TransactionCommentList)
            {
                SQL_STATEMENT.Append($"INSERT INTO TransactionComments (TransactionHash, OutputIndex, Comment, Timestamp) VALUES('{item.TransactionHash}',{item.OutputIndex},'{item.Comment}',{item.Timestamp});");
            }
            SQL_STATEMENT.Append("END TRANSACTION;");
            using (SqliteConnection con = new SqliteConnection(base.CacheConnectionString))
            using (SqliteCommand cmd = new SqliteCommand(SQL_STATEMENT.ToString(), con))
            {

                cmd.Connection.Open();
                //con.SynchronousNORMAL();
                rows = cmd.ExecuteNonQuery();
            }
            return rows;
        }
    }
}