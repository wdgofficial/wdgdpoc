// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

using WDG.ServiceAgent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace WDG.Wallet.Test.ServiceAgent
{
    [TestClass]
    public class WalletMangementTest
    {
        [TestMethod]
        public async Task BackupWallet()
        {
            WalletManagement management = new WalletManagement();
            await management.BackupWallet("D:\\wallet.dat");
        }

        [TestMethod]
        public async Task RestoreWalletBackup()
        {
            WalletManagement management = new WalletManagement();
            await management.RestoreWalletBackup("D:\\wallet.dat");
        }

        [TestMethod]
        public async Task EncryptWallet()
        {
            WalletManagement management = new WalletManagement();
            await management.EncryptWallet("P@ssw0rd");
        }

        [TestMethod]
        public async Task WalletPassphrase()
        {
            WalletManagement management = new WalletManagement();
            await management.WalletPassphrase("P@ssw0rd");
        }

        [TestMethod]
        public async Task WalletLock()
        {
            WalletManagement management = new WalletManagement();
            await management.WalletLock();
        }

        [TestMethod]
        public async Task WalletPassphraseChange()
        {
            WalletManagement management = new WalletManagement();
            await management.WalletPassphraseChange("P@ssw0rd", "NewP@ssw0rd");
        }
    }
}
