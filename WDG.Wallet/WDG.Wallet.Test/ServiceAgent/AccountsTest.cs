// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

using WDG.DTO;
using WDG.ServiceAgent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace WDG.Wallet.Test.ServiceAgent
{
    [TestClass]
    public class AccountsTest
    {
        [TestMethod]
        public async Task GetAddressesByTag()
        {
            Accounts accounts = new Accounts();
            AccountInfoOM[] result = await accounts.GetAddressesByTag("");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetAccountByAddress()
        {
            Accounts accounts = new Accounts();
            AccountInfoOM result = await accounts.GetAccountByAddress("fiiitFmH9Cqk5B9gTH3LqZzBtqb8pxgHJ7sVqY");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetNewAddress()
        {
            Accounts accounts = new Accounts();
            AccountInfoOM result = await accounts.GetNewAddress("新地址");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetDefaultAccount()
        {
            Accounts accounts = new Accounts();
            AccountInfoOM result = await accounts.GetDefaultAccount();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task SetDefaultAccount()
        {
            Accounts accounts = new Accounts();
            await accounts.SetDefaultAccount("fiiitFmH9Cqk5B9gTH3LqZzBtqb8pxgHJ7sVqY");
        }

        [TestMethod]
        public async Task ValidateAddress()
        {
            Accounts accounts = new Accounts();
            AddressInfoOM result = await accounts.ValidateAddress("fiiitFmH9Cqk5B9gTH3LqZzBtqb8pxgHJ7sVqY");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task SetAccountTag()
        {
            Accounts accounts = new Accounts();
            //first validate address
            string address = "fiiitFmH9Cqk5B9gTH3LqZzBtqb8pxgHJ7sVqY";
            AddressInfoOM result = await accounts.ValidateAddress(address);
            if (result.IsValid)
            {
                await accounts.SetAccountTag(address, "new tag");
            }
        }
    }
}
