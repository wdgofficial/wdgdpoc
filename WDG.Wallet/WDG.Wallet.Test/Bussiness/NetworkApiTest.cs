// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

using WDG.Business;
using WDG.Models;
using WDG.Utility.Api;
using FluentScheduler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WDG.Wallet.Test.Bussiness
{
    [TestClass]
    public class NetworkApiTest: Registry
    {
        [TestMethod]
        public async Task GetNetworkInfo()
        {
            ApiResponse response = await NetworkApi.GetNetworkInfo();
            Assert.IsFalse(response.HasError);
            NetworkInfo result = response.GetResult<NetworkInfo>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetNetTotals()
        {
            ApiResponse response = await NetworkApi.GetNetTotals();
            Assert.IsFalse(response.HasError);
            NetTotals result = response.GetResult<NetTotals>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetConnectionCount()
        {
            ApiResponse response = await NetworkApi.GetConnectionCount();
            Assert.IsFalse(response.HasError);
            long result = response.GetResult<long>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetPeerInfo()
        {
            ApiResponse response = await NetworkApi.GetPeerInfo();
            Assert.IsFalse(response.HasError);
            List<PeerInfo> result = response.GetResult<List<PeerInfo>>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task AddNode()
        {
            ApiResponse response = await NetworkApi.AddNode("123.123.123.123:4321");
            Assert.IsFalse(response.HasError);
        }

        [TestMethod]
        public async Task GetAddedNodeInfo()
        {
            ApiResponse response = await NetworkApi.GetAddedNodeInfo("192.168.1.177:4321");
            Assert.IsFalse(response.HasError);
            AddNodeInfo result = response.GetResult<AddNodeInfo>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task DisconnectNode()
        {
            ApiResponse response = await NetworkApi.DisconnectNode("123.123.123.123:4321");
            Assert.IsFalse(response.HasError);
            bool result = response.GetResult<bool>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task SetBan()
        {
            ApiResponse response = await NetworkApi.SetBan("123.123.123.123:4321", "add");
            Assert.IsFalse(response.HasError);
        }

        [TestMethod]
        public async Task ListBanned()
        {
            ApiResponse response = await NetworkApi.ListBanned();
            Assert.IsFalse(response.HasError);
            List<BannedInfo> result = response.GetResult<List<BannedInfo>>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task ClearBanned()
        {
            ApiResponse response = await NetworkApi.ClearBanned();
            Assert.IsFalse(response.HasError);
        }

        [TestMethod]
        public async Task SetNetworkActive()
        {
            ApiResponse response = await NetworkApi.SetNetworkActive(true);
            Assert.IsFalse(response.HasError);
        }

        [TestMethod]
        public async Task GetBlockChainInfo()
        {
            ApiResponse response = await NetworkApi.GetBlockChainInfo();
            Assert.IsFalse(response.HasError);
            BlockChainInfo result = response.GetResult<BlockChainInfo>();
            Assert.IsNotNull(result);
        }

        public BlockSyncInfo SyncBlock()
        {
            /* 页面开始的区块同步
             * 1、剩余区块数                                      1466  ↓
             * 2、最后区块时间                                     周三 7月4 01:55:39:2018 ↑
             * 3、进度                                             99.93% ↑
             * 4、每小时增长速度                                    1.00%   速率(可大可小)
             * 5、剩余同步时间                                     4 minute(s)  ↓
             * 
             * 讲解
             * 已知变量：
             * 远端区块高度：远端的区块高度，总的区块高度，每5分钟打包一次，5分钟变化一次
             * 本地区块高度：本地的区块高度，会有延迟
             * 时间偏移量：时间戳，需要和当前时间比较，两者的差即为区块落后时间，注意单位变换
             * 
             * 刚开始会从那边接口获取三个数据字段，一个是当前区块高度，另一个是总的区块高度，先用一个全局变量存储总的区块高度，然后总的区块高度和当前区块高度两个的差就是未同步的区块高度即剩余区块数，
             * 循环任务
             * 剩余区块数：获取到的remoteLastBlockHeight和全局变量存储的区块高度对比，如果大于存储的就更新， remoteLastBlockHeight-localLastBlockHeight
             * 最后区块时间：暂无
             * 进度：(localLastBlockHeight/总的区块高度)*100%
             * 每小时增长速度：((进度2-进度1)/ 时间2-时间1) *3600=(((localLastBlockHeight2/总的区块高度)-(localLastBlockHeight1/总的区块高度))/ 1s) *3600=((localLastBlockHeight2-localLastBlockHeight1)/总的区块高度)*3600
             * 剩余同步时间：剩余区块数/增长速度
             * 同步区块个数除以总区块个数就是进度
             * 剩余同步时间就是进度除以每小时增长速度
             */
            long AllBlockHeight = 0;
            long beforeTimeOffset = 0;
            long beforeLocalLastBlockHeight = 0;
            BlockSyncInfo block = new BlockSyncInfo();
            Schedule(async () =>
            {
                ApiResponse response = await NetworkApi.GetBlockChainInfo();
                if (!response.HasError)
                {
                    BlockChainInfo info = response.GetResult<BlockChainInfo>();
                    if (info.IsRunning)
                    {
                        if (info.RemoteLatestBlockHeight > info.LocalLastBlockHeight + 2)
                        {
                            if (AllBlockHeight == 0)
                            {
                                //第一次获取数据
                                AllBlockHeight = info.RemoteLatestBlockHeight;
                                beforeTimeOffset = info.TimeOffset;
                                beforeLocalLastBlockHeight = info.LocalLastBlockHeight;
                            }
                            else
                            {
                                //不是第一次，开始组织数据
                                if (info.RemoteLatestBlockHeight > AllBlockHeight)
                                {
                                    AllBlockHeight = info.RemoteLatestBlockHeight;
                                }
                                //剩余区块数：总区块高度-本地最后区块高度
                                block.BlockLeft = AllBlockHeight - info.LocalLastBlockHeight;
                                //最后区块时间
                                block.LastBlockTime = null;
                                //区块同步进度：已同步的区块数目 / 总区块数目
                                block.Progress = (1 - (block.BlockLeft / AllBlockHeight)) * 100;        
                                //区块每小时增加进度：
                                block.ProgressSpeed = ((info.LocalLastBlockHeight - beforeLocalLastBlockHeight) / AllBlockHeight) * 3600;
                                //剩余时间
                                block.TimeLeft = (block.Progress / block.ProgressSpeed).ToString();
                                block.BehindTime = (double)info.TimeOffset - (DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds;

                                //
                                beforeLocalLastBlockHeight = info.LocalLastBlockHeight;
                                beforeTimeOffset = info.TimeOffset;
                            }
                        }
                    }
                }
            }).ToRunNow().AndEvery(1).Seconds();
            return block;
        }
    }

    public class BlockSyncInfo
    {
        /// <summary>
        /// 剩余区块数量
        /// </summary>
        public long BlockLeft { get; set; }

        /// <summary>
        /// 最后区块时间
        /// </summary>
        public string LastBlockTime { get; set; }

        /// <summary>
        /// 同步进度
        /// </summary>
        public double Progress { get; set; }

        /// <summary>
        /// 同步速度
        /// </summary>
        public double ProgressSpeed { get; set; }

        /// <summary>
        /// 剩余时间
        /// </summary>
        public string TimeLeft { get; set; }

        /// <summary>
        /// 落后时间
        /// </summary>
        public double BehindTime { get; set; }
    }
}
