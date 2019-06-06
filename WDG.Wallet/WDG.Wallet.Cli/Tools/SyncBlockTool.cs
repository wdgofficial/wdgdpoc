// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

using WDG.Business;
using WDG.Models;
using WDG.Utility.Api;
using System;
using System.Threading.Tasks;

namespace WDG.Wallet.Cli.Tools
{
    public class SyncBlockTool
    {
        public static async Task SyncBlock(Func<int, int> dispalyProgress = null)
        {
            //获取当前区块高度和远端区块高度
            
            ApiResponse response = await NetworkApi.GetBlockChainInfo();
            if (!response.HasError)
            {
                BlockChainInfo info = response.GetResult<BlockChainInfo>();
                if (info.IsRunning)
                {
                    if (info.RemoteLatestBlockHeight > info.LocalLastBlockHeight)
                    {
                        //开始同步
                        long localBlockHeight = info.LocalLastBlockHeight;
                        long remoteBlockHeight = info.RemoteLatestBlockHeight;
                        while (remoteBlockHeight > localBlockHeight)
                        {
                            dispalyProgress?.Invoke(Convert.ToInt32(((double)localBlockHeight / (double)remoteBlockHeight) * 100));
                            System.Threading.Thread.Sleep(3000);
                            ApiResponse newResponse = await NetworkApi.GetBlockChainInfo();
                            if (!newResponse.HasError)
                            {
                                BlockChainInfo newBlockInfo = newResponse.GetResult<BlockChainInfo>();
                                localBlockHeight = newBlockInfo.LocalLastBlockHeight;
                                remoteBlockHeight = newBlockInfo.RemoteLatestBlockHeight;
                                if (remoteBlockHeight <= localBlockHeight)
                                {
                                    dispalyProgress?.Invoke(100);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static async Task<bool> IsNeedSyncBlock()
        {
            ApiResponse response = await NetworkApi.GetBlockChainInfo();
            if (!response.HasError)
            {
                BlockChainInfo info = response.GetResult<BlockChainInfo>();
                if (info.IsRunning)
                {
                    if (info.RemoteLatestBlockHeight > info.LocalLastBlockHeight)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
