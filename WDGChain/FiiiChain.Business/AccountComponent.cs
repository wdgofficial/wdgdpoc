// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.Consensus;
using FiiiChain.Data;
using FiiiChain.Data.Accesses;
using FiiiChain.DataAgent;
using FiiiChain.Entities;
using FiiiChain.Framework;
using FiiiChain.IModules;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FiiiChain.Business
{
    public class AccountComponent
    {
        const string encryptExtensionName = ".fcx";
        const string noEncryptExtensionName = ".fct";
        public Account GenerateNewAccount(bool isAddtoCache = true)
        {
            var dac = AccountDac.Default;

            byte[] privateKey;
            byte[] publicKey;
            using (var dsa = ECDsa.GenerateNewKeyPair())
            {
                privateKey = dsa.PrivateKey;
                publicKey = dsa.PublicKey;
            }

            var id = AccountIdHelper.CreateAccountAddress(publicKey);
            if (dac.IsExisted(id))
            {
                throw new Exception("Account id is existed");
            }

            Account account = new Account();
            account.Id = id;
            account.PrivateKey = Base16.Encode(privateKey);
            account.PublicKey = Base16.Encode(publicKey);
            account.Balance = 0;
            account.IsDefault = false;
            account.WatchedOnly = false;

            AccountAccess.Current.Insert(account, null);
            if(isAddtoCache)
                CacheManager.Default.Put(DataCatelog.Accounts, account.Id, account);
            if (UtxoSet.Instance != null)
            {
                UtxoSet.Instance.AddAccountId(account.Id);
            }

            return account;
        }

        public Account ImportAccount(string privateKeyText)
        {
            var dac = AccountDac.Default;

            byte[] privateKey = Base16.Decode(privateKeyText);
            byte[] publicKey;
            using (var dsa = ECDsa.ImportPrivateKey(privateKey))
            {
                publicKey = dsa.PublicKey;
            }

            var id = AccountIdHelper.CreateAccountAddress(publicKey);
            Account account = dac.SelectById(id);

            if (account == null)
            {
                account = new Account();
                account.Id = AccountIdHelper.CreateAccountAddress(publicKey);
                account.PrivateKey = Base16.Encode(privateKey);
                account.PublicKey = Base16.Encode(publicKey);
                account.Balance = 0;
                account.IsDefault = false;
                account.WatchedOnly = false;

                AccountAccess.Current.Insert(account);
                UtxoSet.Instance.AddAccountId(account.Id);
            }

            return account;
        }

        public Account ImportObservedAccount(string publicKeyText)
        {
            var dac = AccountDac.Default;

            var publicKey = Base16.Decode(publicKeyText);
            var id = AccountIdHelper.CreateAccountAddress(publicKey);

            Account account = dac.SelectById(id);

            if (account == null)
            {
                account = new Account();
                account.Id = AccountIdHelper.CreateAccountAddress(publicKey);
                account.PrivateKey = "";
                account.PublicKey = Base16.Encode(publicKey);
                account.Balance = 0;
                account.IsDefault = false;
                account.WatchedOnly = true;

                AccountAccess.Current.Insert(account);
                UtxoSet.Instance.AddAccountId(account.Id);
            }

            return account;
        }

        public Account GenerateWatchOnlyAddress(string publickeyText)
        {
            var dac = AccountDac.Default;
            byte[] publickey = Base16.Decode(publickeyText);
            string id = AccountIdHelper.CreateAccountAddress(publickey);

            Account account = dac.SelectById(id);

            if (account == null)
            {
                account = new Account();
                account.Id = id;
                account.PrivateKey = "";
                account.PublicKey = publickeyText;
                account.Balance = 0;
                account.IsDefault = false;
                account.WatchedOnly = true;

                AccountAccess.Current.Insert(account);
                //UtxoSet.Instance.AddAccountId(account.Id);
            }

            return account;
        }

        public List<Account> GetAllAccountsInDb()
        {
            var dac = AccountDac.Default;
            return dac.SelectAll();
        }

        public List<Account> GetAllAccounts()
        {
            return CacheManager.Default.Get<Account>(DataCatelog.Accounts);
        }

        public Account GetAccountById(string id)
        {
            var dac = AccountDac.Default;
            return dac.SelectById(id);
        }

        public List<Account> GetAccountCategory(int category)
        {
            return AccountDac.Default.GetAccountCategory(category);
        }

        public List<Account> GetAccountsByTag(string tag)
        {
            return AccountDac.Default.SelectByTag(tag);
        }

        public Account GetDefaultAccount()
        {
            var dac = AccountDac.Default;
            return dac.SelectFirstDefaultAccount();
        }

        public void SetDefaultAccount(string id)
        {
            var dac = AccountDac.Default;
            dac.SetDefaultAccount(id);
        }

        public void UpdateBalance(string id, long amount)
        {
            AccountAccess.Current.UpdateBalance(id, amount);
        }

        public void UpdatePrivateKeyAr(Account account)
        {
            AccountAccess.Current.UpdatePrivateKeyAr(new List<Account>(new Account[]{ account }));
        }

        public void DeleteAccount(string id)
        {
            var dac = AccountDac.Default;
            dac.Delete(id);
            UtxoSet.Instance.RemoveAccountId(id);
        }

        public void UpdateTag(string id, string tag)
        {
            var task = new Action(() => {
                var dac = AccountDac.Default;
                var account = dac.SelectById(id);
                CacheManager.Default.Put(DataCatelog.Accounts, account.Id, account);
            });
            AccountAccess.Current.UpdateTag(id, tag, task);
        }

        public void ExportPublicKeyAndAddress(string address, string filePath, string salt)
        {
            string extensionName = Path.GetExtension(filePath).ToLower();
            if (string.IsNullOrWhiteSpace(salt))
            {
                filePath = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + noEncryptExtensionName);
            }
            else
            {
                filePath = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + encryptExtensionName);
            }
            try
            {
                Account account = AccountDac.Default.SelectById(address);
                WatchAccountBackup backup = new WatchAccountBackup() { Address = account.Id, PublicKey = account.PublicKey };
                if (backup != null)
                {
                    if (extensionName == noEncryptExtensionName)
                    {
                        SaveFile(backup, filePath, null);
                    }
                    else
                    {
                        SaveFile(backup, filePath, salt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CommonException(ErrorCode.Engine.Wallet.DATA_SAVE_TO_FILE_ERROR, ex);
            }
        }

        private static void SaveFile<T>(T obj, string filePath, string salt)
        {
            string jsonString = JsonConvert.SerializeObject(obj);
            string encryptString = jsonString;

            if (!string.IsNullOrEmpty(salt))
            {
                encryptString = AES128.Encrypt(jsonString, salt);
            }
            FileHelper.StringSaveFile(encryptString, filePath);
        }

        public Account ImportPublicKeyAndAddress(string filePath, string salt)
        {
            Account result = null;

            string extensionName = Path.GetExtension(filePath).ToLower();
            if (extensionName != encryptExtensionName && extensionName != noEncryptExtensionName)
            {
                throw new CommonException(ErrorCode.Engine.Wallet.IO.EXTENSION_NAME_NOT_SUPPORT);
            }
            WatchAccountBackup backup = null;
            try
            { 
                if (extensionName == noEncryptExtensionName)
                {
                    backup = LoadFile<WatchAccountBackup>(filePath, null);
                }
                else
                {
                    backup = LoadFile<WatchAccountBackup>(filePath, salt);
                }
                if (backup != null)
                {
                    AccountDac dac = AccountDac.Default;
                    dac.Insert(new Account { Balance = 0, Id = backup.Address, IsDefault = false, PrivateKey = null, PublicKey = backup.PublicKey, Tag = "", Timestamp = Time.EpochTime, WatchedOnly = true});
                    result = dac.SelectById(backup.Address);
                }
            }
            catch (Exception ex)
            {
                throw new CommonException(ErrorCode.Engine.Wallet.DB.EXECUTE_SQL_ERROR, ex);
            }
            return result;
        }

        private T LoadFile<T>(string filePath, string salt)
        {
            if (!File.Exists(filePath))
            {
                throw new CommonException(ErrorCode.Engine.Wallet.IO.FILE_NOT_FOUND);
            }
            string fileString = string.Empty;
            try
            {
                fileString = FileHelper.LoadFileString(filePath);
            }
            catch (Exception ex)
            {
                throw new CommonException(ErrorCode.Engine.Wallet.IO.FILE_DATA_INVALID, ex);
            }

            try
            {
                if (!string.IsNullOrEmpty(salt))
                {
                    fileString = AES128.Decrypt(fileString, salt);
                }
            }
            catch (Exception ex)
            {
                throw new CommonException(ErrorCode.Engine.Wallet.DECRYPT_DATA_ERROR, ex);
            }
            return JsonConvert.DeserializeObject<T>(fileString);
        }
    }
}
