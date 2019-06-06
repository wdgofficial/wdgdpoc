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
    public class NetworkTest
    {
        [TestMethod]
        public async Task GetNetworkInfo()
        {
            Network network = new Network();
            NetworkInfoOM result = await network.GetNetworkInfo();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetNetTotals()
        {
            Network network = new Network();
            NetTotalsOM result = await network.GetNetTotals();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetConnectionCount()
        {
            Network network = new Network();
            long result = await network.GetConnectionCount();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetPeerInfo()
        {
            Network network = new Network();
            PeerInfoOM[] result = await network.GetPeerInfo();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task AddNode()
        {
            Network network = new Network();
            await network.AddNode("123.123.123.123:4321");
        }

        [TestMethod]
        public async Task GetAddedNodeInfo()
        {
            Network network = new Network();
            AddNodeInfoOM result = await network.GetAddedNodeInfo("192.168.1.177:4321");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task DisconnectNode()
        {
            Network network = new Network();
            bool result = await network.DisconnectNode("192.168.1.177:4321");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task SetBan()
        {
            Network network = new Network();
            await network.SetBan("123.123.123.123:4321", "add");
        }

        [TestMethod]
        public async Task ListBanned()
        {
            Network network = new Network();
            BannedInfoOM[] result = await network.ListBanned();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task ClearBanned()
        {
            Network network = new Network();
            await network.ClearBanned();
        }

        [TestMethod]
        public async Task SetNetworkActive()
        {
            Network network = new Network();
            await network.SetNetworkActive(false);
        }

        [TestMethod]
        public async Task GetBlockChainInfo()
        {
            Network network = new Network();
            BlockChainInfoOM result = await network.GetBlockChainInfo();
            Assert.IsNotNull(result);
        }
    }
}
