// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

using WDG.Business;
using WDG.DTO;
using WDG.Models;
using WDG.Utility;
using WDG.Utility.Api;
using WDG.Utility.Helper;
using FluentScheduler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WDG.Wallet.Test.Bussiness
{
    [TestClass]
    public class BlockChainEngineApiTest : Registry
    {
        [TestMethod]
        public async Task GetBlockChainStatus()
        {
            ApiResponse response = await BlockChainEngineApi.GetBlockChainStatus();
            Assert.IsFalse(response.HasError);
            BlockChainStatus result = response.GetResult<BlockChainStatus>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task StopEngine()
        {
            ApiResponse response = await BlockChainEngineApi.StopEngine();
            Assert.IsFalse(response.HasError);
        }

        [TestMethod]
        public async Task ApplicationClosed()
        {
            /* 当应用程序关闭的时候不能直接关闭，因为某些程序可能还在运行，如果立即结束，有可能造成数据丢失
             * 1、当应用程序关闭的时候需要即时判断BlockChainStatus的状态，如果是Stoped就可以结束了，如果是Stopping或者Running不能结束
             * 需要用try catch捕获异常来判断是否关闭
             */
            //应用程序关闭的时候先调用StopEngine的接口
            await BlockChainEngineApi.StopEngine();
            //即时判断BlockChainStatus
            try
            {
                Schedule(async () =>
                {
                    ApiResponse response = await BlockChainEngineApi.GetBlockChainStatus();
                    if (!response.HasError)
                    {
                        BlockChainStatus result = response.GetResult<BlockChainStatus>();
                        if (result.RpcService == "Stopped")
                        {
                            //这个其实什么都不用写，因为接口服务关闭后，你只能获取到错误
                            //Application.Exit();
                        }
                    }
                }).ToRunNow().AndEvery(1).Seconds();
            }
            catch
            {
                //在这里捕获错误，然后关闭整个Application
                //Application.Exit();
            }
        }

        [TestMethod]
        public async Task GetBlockCount()
        {
            while (true)
            {
                try
                {
                    ApiResponse response = await BlockChainEngineApi.GetBlockCount();
                    if (response.HasError)
                    {
                        throw new Exception();
                    }
                    else
                    {
                        long result = response.GetResult<long>();
                        Assert.IsNotNull(result);
                    }
                    Thread.Sleep(8000);
                }
                catch (Exception ex)
                {
                    Assert.Fail();
                }
            }
        }

        [TestMethod]
        public async Task GetBlockHash()
        {
            ApiResponse response = await BlockChainEngineApi.GetBlockHash(29);
            Assert.IsFalse(response.HasError);
            string result = response.GetResult<string>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetBlock()
        {
            ApiResponse response = await BlockChainEngineApi.GetBlock("B6D9FD10775254A86E836F211205D8A21D0F3FF3821C9E6F1F99377BFB4DADFA", 1);
            Assert.IsFalse(response.HasError);
            if (response.Result != null)
            {
                BlockInfoOM result = response.GetResult<BlockInfoOM>();
                Assert.IsNotNull(result);
            }
            else
            {
                Assert.IsNull(response.Result);
            }
            
        }

        [TestMethod]
        public async Task GetBlockHeader()
        {
            ApiResponse response = await BlockChainEngineApi.GetBlockHeader("B6D9FD10775254A86E836F211205D8A21D0F3FF3821C9E6F1F99377BFB4DADFA", 1);
            Assert.IsFalse(response.HasError);
            if (response.Result != null)
            {
                BlockHeaderOM result = response.GetResult<BlockHeaderOM>();
                Assert.IsNotNull(result);
            }
            else
            {
                Assert.IsNull(response.Result);
            }
            
        }

        [TestMethod]
        public async Task GetChainTips()
        {
            ApiResponse response = await BlockChainEngineApi.GetChainTips();
            Assert.IsFalse(response.HasError);
            if (response.Result != null)
            {
                ChainTipsOM[] result = response.GetResult<ChainTipsOM[]>();
                Assert.IsNotNull(result);
            }
            else
            {
                Assert.IsNull(response.Result);
            }
        }

        [TestMethod]
        public async Task GetDifficulty()
        {
            ApiResponse response = await BlockChainEngineApi.GetDifficulty();
            Assert.IsFalse(response.HasError);
            BlockDifficultyOM result = response.GetResult<BlockDifficultyOM>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GenerateNewBlock()
        {
            ApiResponse response = await BlockChainEngineApi.GenerateNewBlock("Zhangsan", "", 1);
            Assert.IsFalse(response.HasError);
            BlockInfoOM result = response.GetResult<BlockInfoOM>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task SubmitBlock()
        {
            ApiResponse response = await BlockChainEngineApi.SubmitBlock("1234567890ABCDEF");
            Assert.IsFalse(response.HasError);
        }

        /* 挖矿步骤
         * 1、获取区块（初始区块没有nonce、没有hash、没有timestamp），注意判断目前区块的高度
         * 2、取出所有Tx（bytes）
         * 3、随机取一个nonce
         * 4、复杂度校验（逐个判断长度64的字符，是不是小于，大于失败，小于成功）
         * 5、校验不成功再取nonce，继续校验，成功就把nonce加入到Header中
         * 6、nonce加上timestamp生成hash
         * 7、提交block
         * 接口
         * GetBlockCount                获取最新的区块高度
         * GetBlockHash                 根据高度取出区块Hash
         * GetBlock                     根据Hash取出区块数据
         * GetBlockHeader               根据Hash取出区块头数据
         * GetChainTips                 获取区块链中分叉的区块端点
         * GetDifficulty                获取下个区块的难度
         * GenerateNewBlock             生成一个新的区块
         * SubmitBlock                  提交区块
         * 思路
         * 1、先调用接口GenerateNewBlock返回一个BlockInfoOM
         * 2、遍历BlockInfoOM的Transactions返回一个List<bytes>
         * 3、开启多线程循环从0L到Int64.Max随机取数，转化为byte[]，添加到BlockInfoOM转化的List<byte>后面，先Hash然后Base16编码
         * 4、验证复杂度（调用获取下个区块的难度接口）
         * 5、验证成功把nonce和时间戳加入到Header中（调用GetBlockHash），不成功的话就继续下个随机数
         * 6、提交block（调用SubmitBlock）
         * 
         * 
         * 
         */
        [TestMethod]
        public async Task BlockMining()
        {
            bool isStop = false;
            ApiResponse response = await BlockChainEngineApi.GenerateNewBlock("Zhangsan", "", 1);
            if (!response.HasError)
            {
                
                BlockInfoOM block = response.GetResult<BlockInfoOM>();

                List<byte> blockData = new List<byte>();

                foreach (BlockTransactionsOM tx in block.Transactions)
                {
                    blockData.AddRange(tx.Serialize());
                }
                string strDifficulty = string.Empty;
                //long blockHeight = 0;
                ApiResponse difficultyResponse = await BlockChainEngineApi.GetDifficulty();
                if (!difficultyResponse.HasError)
                {
                    BlockDifficultyOM blockDifficulty = difficultyResponse.GetResult<BlockDifficultyOM>();
                    strDifficulty = blockDifficulty.HashTarget;
                }
                //ApiResponse heightResponse = await BlockChainEngineApi.GetBlockCount();
                //if (!heightResponse.HasError)
                //{
                //    blockHeight = heightResponse.GetResult<long>();
                //}
                var cts = new CancellationTokenSource();
                var ct = cts.Token;
                //开启新线程
                Task task1 = Task.Run(async () =>
                {
                    Console.WriteLine("New Task Start");
                    while(!isStop)
                    {
                        Console.WriteLine("Loop is start");
                        ApiResponse tempResponse = await BlockChainEngineApi.GetBlockCount();
                        if (!tempResponse.HasError)
                        {
                            long height = tempResponse.GetResult<long>();
                            if (height >= block.Header.Height)
                            {
                                isStop = true;
                                cts.Cancel();
                            }
                        }
                        Console.WriteLine("I will sleep 5000 millseconds");
                        Thread.Sleep(5000);
                    }
                }, ct);
                
                Parallel.For(0L, Int64.MaxValue, new ParallelOptions { MaxDegreeOfParallelism = 2 }, async (i, loopState) =>
                {
                    Console.WriteLine("Parallel start");

                    //根据BlockData初始化一个List<byte>
                    List<byte> newBuffer = new List<byte>(blockData.ToArray());
                    //获取随机的64位数的字节
                    byte[] nonceBytes = BitConverter.GetBytes(i);
                    //存储在此计算机体系结构中的字节顺序
                    if (BitConverter.IsLittleEndian)
                    {
                        //翻转整个byte[]顺序
                        Array.Reverse(nonceBytes);
                    }
                    //随机数添加到List<byte>的末尾
                    newBuffer.AddRange(nonceBytes);
                    //List<byte>转化为byte[]数组先Hash后Base16编码
                    string result = Base16.Encode(
                        HashHelper.Hash(
                            newBuffer.ToArray()
                        ));
                    Console.WriteLine("begin verify data");
                    //block头验证,校验不成功，继续循环
                    if (BlockInfoOM.Verify(strDifficulty, result))
                    {
                        Console.WriteLine("Verify success");
                        //区块头的时间戳
                        block.Header.Timestamp = TimeHelper.EpochTime;
                        //区块头的随机数
                        block.Header.Nonce = i;
                        //区块头的hash
                        block.Header.Hash = block.Header.GetHash();
                        //提交区块
                        ApiResponse submitResponse = await BlockChainEngineApi.SubmitBlock(Base16.Encode(block.Serialize()));
                        if (!submitResponse.HasError)
                        {
                            //停止循环
                            loopState.Stop();
                            Logger.Singleton.Debug("A New Block " + block.Header.Height + "has been created, the correct nonce is " + i);
                        }
                        
                    }
                    if (isStop)
                    {
                        ct.ThrowIfCancellationRequested();
                        cts.Cancel();
                        await BlockMining();
                    }
                });
            }
        }
    }
}
