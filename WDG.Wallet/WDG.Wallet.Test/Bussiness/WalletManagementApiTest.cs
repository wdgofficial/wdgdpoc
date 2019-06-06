// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

using WDG.Business;
using WDG.Models;
using WDG.Utility.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WDG.Wallet.Test.Bussiness
{
    [TestClass]
    public class WalletManagementApiTest
    {
        [TestMethod]
        public async Task BackupWallet()
        {
            //钱包备份保存路径（加密文件：.fcdatx，非加密文件：.fcdat）
            ApiResponse response = await WalletManagementApi.BackupWallet("D:\\wallet.fcdat");
            Assert.IsFalse(response.HasError);
        }

        [TestMethod]
        public async Task RestoreWalletBackup()
        {
            //钱包路径（加密文件：.fcdatx，非加密文件：.fcdat）
            //fcdatx需要输入密钥
            ApiResponse response = await WalletManagementApi.BackupWallet("D:\\wallet.fcdat");
            Assert.IsFalse(response.HasError);
        }

        [TestMethod]
        public async Task EncryptWallet()
        {
            string input = "P@ssw0rd$";
            var regex = new Regex(@"
                (?=.*[0-9])                     #必须包含数字
                (?=.*[a-z])                     #必须包含小写字母
                (?=.*[A-Z])                     #必须包含大写字母
                (?=([\x21-\x7e]+)[^a-zA-Z0-9])  #必须包含特殊符号
                .{8,30}                         #至少8个字符，最多30个字符
                ", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            Assert.IsTrue(regex.IsMatch(input));
            
            ApiResponse response = await WalletManagementApi.EncryptWallet("P@ssw0rd$");
            Assert.IsFalse(response.HasError);
        }

        [TestMethod]
        public async Task WalletPassphrase()
        {
            //ApiResponse response = await WalletManagementApi.WalletPassphrase("P@ssw0rd$");
            ApiResponse response = await WalletManagementApi.WalletPassphrase("P@ssw0rd$");
            if (!response.HasError)
            {
                bool result = response.GetResult<bool>();
            }
            Assert.IsFalse(response.HasError);
        }

        [TestMethod]
        public async Task WalletLock()
        {
            ApiResponse response = await WalletManagementApi.WalletLock();
            Assert.IsFalse(response.HasError);
        }

        [TestMethod]
        public async Task WalletPassphraseChange()
        {
            ApiResponse response = await WalletManagementApi.WalletPassphraseChange("P@ssw0rd$", "P@ssw0rd$");
            Assert.IsFalse(response.HasError);
        }

        /* 钱包加密
         * 三处需要锁定，
         * 1、备份前解锁，备份后锁定
         * 2、备份还原后需要锁定
         * 3、转账交易前解锁，交易后锁定
         * 加密前：无限制，修改密码菜单禁用
         * 加密后：加密钱包菜单禁用，转账时，先判断是否加密，如果加密就先输入密码，然后操作，在操作界面等待时间过长就会需要重新解锁
         * 
         */
        
        [TestMethod]
        public async Task Transfer()
        {
            bool isEncrypt = false;
            //先调用接口判断是否加密
            ApiResponse response = await TransactionApi.GetTxSettings();
            if (!response.HasError)
            {
                TransactionFeeSetting setting = response.GetResult<TransactionFeeSetting>();
                isEncrypt = setting.Encrypt;
            }
            if (isEncrypt)
            {
                //先解锁
                string password = "this is a demo";
                ApiResponse unlockResponse = await WalletManagementApi.WalletPassphrase(password);
                if (!unlockResponse.HasError)
                {
                    //write your logic
                   /* 三处需要锁定，
                    *1、备份前解锁，备份后锁定
                    *2、备份还原后需要锁定
                    *3、转账交易前解锁，交易后锁定
                    */
                }
                //锁定
                ApiResponse lockResponse = await WalletManagementApi.WalletLock();
            }
        }
    }
}
