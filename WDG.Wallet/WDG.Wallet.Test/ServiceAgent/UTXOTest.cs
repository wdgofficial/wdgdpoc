// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

using WDG.DTO;
using WDG.ServiceAgent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WDG.Wallet.Test.ServiceAgent
{
    [TestClass]
    public class UTXOTest
    {
        [TestMethod]
        public async Task GetTxOutSetInfo()
        {
            // Server Test
            UTXO utxo = new UTXO();
            TxOutSetOM om = await utxo.GetTxOutSetInfo();
            Assert.IsNotNull(om);
        }

        [TestMethod]
        public async Task GetListUnspentInfo()
        {
            UTXO utxo = new UTXO();
            List<UnspentOM> list = await utxo.ListUnspent(2, 99999, null);
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public async Task GetUnconfirmedBalance()
        {
            UTXO utxo = new UTXO();
            long balance = await utxo.GetUnconfirmedBalance();
            Assert.IsNotNull(balance);
        }

        [TestMethod]
        public async Task GetTxOut()
        {
            UTXO utxo = new UTXO();
            TxOutOM tx = await utxo.GetTxOut("474DF906F3A53285EC40566D0BA1AFBD4828BDD3789C9599F6EA0489C4333381", 0, false);
            Assert.IsNotNull(tx);
        }

        [TestMethod]
        public async Task ListUnspent()
        {
            UTXO utxo = new UTXO();
            List<UnspentOM> result = await utxo.ListUnspent(1);
            Assert.IsNotNull(result);
        }
    }
}
