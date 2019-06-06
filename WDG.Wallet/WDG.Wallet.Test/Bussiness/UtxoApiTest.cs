// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

using WDG.Business;
using WDG.Models;
using WDG.Utility.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WDG.Wallet.Test.Bussiness
{
    [TestClass]
    public class UtxoApiTest
    {
        [TestMethod]
        public async Task TestGetTxOutSetInfo()
        {
            //Server Test
            ApiResponse response = await UtxoApi.GetTxOutSetInfo();
            //Client Use
            Assert.IsFalse(response.HasError);
            TxOutSet result = response.GetResult<TxOutSet>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task TestGetListUnspent()
        {
            ApiResponse response = await UtxoApi.ListUnspent(2, 99999, null);
            Assert.IsFalse(response.HasError);
            List<UnspentUtxo> result = response.GetResult<List<UnspentUtxo>>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task TestGetUnconfirmedBalance()
        {
            ApiResponse response = await UtxoApi.GetUnconfirmedBalance();
            Assert.IsFalse(response.HasError);
            long result = response.GetResult<long>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task TestGetTxOut()
        {
            ApiResponse response = await UtxoApi.GetTxOut("474DF906F3A53285EC40566D0BA1AFBD4828BDD3789C9599F6EA0489C4333381", 0, false);
            Assert.IsFalse(response.HasError);
            TxOutModel result = response.GetResult<TxOutModel>();
            Assert.IsNotNull(result);
        }
    }
}
