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
    public class AddressBookTest
    {
        [TestMethod]
        public async Task AddNewAddressBookItem()
        {
            AddressBook book = new AddressBook();
            await book.AddNewAddressBookItem("fiiitFmH9Cqk5B9gTH3LqZzBtqb8pxgHJ7sVqY", "testnet");
        }

        [TestMethod]
        public async Task GetAddressBook()
        {
            AddressBook book = new AddressBook();
            AddressBookInfoOM[] result = await book.GetAddressBook();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetAddressBookItemByAddress()
        {
            AddressBook book = new AddressBook();
            AddressBookInfoOM result = await book.GetAddressBookItemByAddress("fiiitFmH9Cqk5B9gTH3LqZzBtqb8pxgHJ7sVqY");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetAddressBookByTag()
        {
            AddressBook book = new AddressBook();
            AddressBookInfoOM[] result = await book.GetAddressBookByTag("John");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task DeleteAddressBookByIds()
        {
            AddressBook book = new AddressBook();
            await book.DeleteAddressBookByIds(new long[] { 5 });
        }
    }
}
