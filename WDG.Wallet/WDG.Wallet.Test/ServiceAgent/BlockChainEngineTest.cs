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
    public class BlockChainEngineTest
    {
        [TestMethod]
        public async Task GetBlockChainStatus()
        {
            BlockChainEngine engine = new BlockChainEngine();
            BlockChainStatusOM result = await engine.GetBlockChainStatus();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task StopEngine()
        {
            BlockChainEngine engine = new BlockChainEngine();
            await engine.StopEngine();
        }

        [TestMethod]
        public async Task GetBlockCount()
        {
            BlockChainEngine engine = new BlockChainEngine();
            long result = await engine.GetBlockCount();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetBlockHash()
        {
            BlockChainEngine engine = new BlockChainEngine();
            string result = await engine.GetBlockHash(29);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetBlock()
        {
            BlockChainEngine engine = new BlockChainEngine();
            BlockInfoOM result = await engine.GetBlock("B6D9FD10775254A86E836F211205D8A21D0F3FF3821C9E6F1F99377BFB4DADFA", 1);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetBlockHeader()
        {
            BlockChainEngine engine = new BlockChainEngine();
            BlockHeaderOM result = await engine.GetBlockHeader("B6D9FD10775254A86E836F211205D8A21D0F3FF3821C9E6F1F99377BFB4DADFA", 1);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetChainTips()
        {
            BlockChainEngine engine = new BlockChainEngine();
            ChainTipsOM[] result = await engine.GetChainTips();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetDifficulty()
        {
            BlockChainEngine engine = new BlockChainEngine();
            BlockDifficultyOM result = await engine.GetDifficulty();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GenerateNewBlock()
        {
            BlockChainEngine engine = new BlockChainEngine();
            BlockInfoOM result = await engine.GenerateNewBlock("Zhangsan", "", 1);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task SubmitBlock()
        {
            BlockChainEngine engine = new BlockChainEngine();
            await engine.SubmitBlock("1234567890ABCDEF");
        }
    }
}
