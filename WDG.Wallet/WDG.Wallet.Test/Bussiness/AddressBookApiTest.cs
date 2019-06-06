// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

using WDG.Business;
using WDG.Models;
using WDG.Utility;
using WDG.Utility.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WDG.Wallet.Test.Bussiness
{
    [TestClass]
    public class AddressBookApiTest
    {
        [TestMethod]
        public async Task AddNewAddressBookItem()
        {
            //fiiit6ZgKDM5ZyDYhkSWDsUmRZpkbRQf7NWiKT
            //先根据接口获取网络类型
            ApiResponse blockChainResponse = await BlockChainEngineApi.GetBlockChainStatus();
            if (!blockChainResponse.HasError)
            {
                BlockChainStatus blockChainStatus = blockChainResponse.GetResult<BlockChainStatus>();
                //验证address
                if (AddressTools.AddressVerfy(blockChainStatus.ChainNetwork, "fiiit6ZgKDM5ZyDYhkSWDsUmRZpkbRQf7NWiKT"))
                {
                    ApiResponse addressBookResponse = await AddressBookApi.AddNewAddressBookItem("fiiit6ZgKDM5ZyDYhkSWDsUmRZpkbRQf7NWiKT", "label or comment");
                    if (!addressBookResponse.HasError)
                    {
                        //do something
                    }
                }
            }
        }

        [TestMethod]
        public async Task GetAddressBook()
        {
            ApiResponse response = await AddressBookApi.GetAddressBook();
            Assert.IsFalse(response.HasError);
            List<AddressBookInfo> result = response.GetResult<List<AddressBookInfo>>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetAddressBookItemByAddress()
        {
            ApiResponse response = await AddressBookApi.GetAddressBookItemByAddress("fiiit6ZgKDM5ZyDYhkSWDsUmRZpkbRQf7NWiKT");
            Assert.IsFalse(response.HasError);
            AddressBookInfo result = response.GetResult<AddressBookInfo>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetAddressBookByTag()
        {
            ApiResponse response = await AddressBookApi.GetAddressBookByTag("John");
            Assert.IsFalse(response.HasError);
            List<AddressBookInfo> result = response.GetResult<List<AddressBookInfo>>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task DeleteAddressBookByIds()
        {
            ApiResponse response = await AddressBookApi.DeleteAddressBookByIds(new long[] { 5,6 });
            Assert.IsFalse(response.HasError);
            bool result = response.GetResult<bool>();
            Assert.IsNotNull(result);
        }
    }
}
