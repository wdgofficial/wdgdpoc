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
    public class MemoryPoolTest
    {
        [TestMethod]
        public async Task GetAllTxInMemPool()
        {
            MemoryPool pool = new MemoryPool();
            string[] result = await pool.GetAllTxInMemPool();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetPaymentInfoInMemPool()
        {
            MemoryPool pool = new MemoryPool();
            PaymentOM[] result = await pool.GetPaymentInfoInMemPool("1D6B63D8294162000D7B4FB2035053482A3D14E228E8543ED4CDE39A93FA1561");
            Assert.IsNotNull(result);
        }
    }
}
