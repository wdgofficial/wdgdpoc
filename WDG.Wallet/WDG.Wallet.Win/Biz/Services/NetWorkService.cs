// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Business;
using WDG.Models;
using WDG.Utility;
using WDG.Utility.Api;
using WDG.Wallet.Win.Biz.Monitor;
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Common.Utils;
using WDG.Wallet.Win.Models;
using System;
using System.Collections.Generic;

namespace WDG.Wallet.Win.Biz.Services
{
    public class NetWorkService : ServiceBase<NetWorkService>
    {
        public string behindformat {
            get
            {
                return LanguageService.Default.GetLanguageValue("behind");
            }
        }
        public string yearsformat
        {
            get
            {
                return LanguageService.Default.GetLanguageValue("yearsBehind");
            }
        }
        public string monthsformat
        {
            get
            {
                return LanguageService.Default.GetLanguageValue("monthsBehind");
            }
        }
        public string weekformat
        {
            get
            {
                return LanguageService.Default.GetLanguageValue("weeksBehind");
            }
        }
        public string dayformat
        {
            get
            {
                return LanguageService.Default.GetLanguageValue("daysBehind");
            }
        }
        public string hoursformat
        {
            get
            {
                return LanguageService.Default.GetLanguageValue("hoursBehind");
            }
        }
        public string minutesformat
        {
            get
            {
                return LanguageService.Default.GetLanguageValue("minutesBehind");
            }
        }
        public string secondsformat
        {
            get
            {
                return LanguageService.Default.GetLanguageValue("secondsBehind");
            }
        }


        public Result<BlockSyncInfo> GetBlockChainInfoSync(BlockSyncInfo block)
        {
            Result<BlockSyncInfo> result = new Result<BlockSyncInfo>();

            ApiResponse response = NetworkApi.GetBlockChainInfo().Result;
            result.IsFail = response.HasError;
            result.Value = block;
            result.ApiResponse = response;
            if (!response.HasError)
            {
                BlockChainInfo info = response.GetResult<BlockChainInfo>();
                if (!info.IsRunning)
                {
                    return result;
                }

                block.ConnectCount = info.Connections;

                //当前区块高度+缓存的区块高度
                var localBlockHeight = info.LocalLastBlockHeight;

                //剩余的区块高度
                block.BlockLeft = info.RemoteLatestBlockHeight - localBlockHeight;
                if (block.BlockLeft < 0)
                    block.BlockLeft = 0;

                if (localBlockHeight < 0 || info.RemoteLatestBlockHeight < 0 || info.Connections <2)
                    return result;
                
                if (!block.IsStartSync && block.BlockLeft > 0)
                {
                    block.IsStartSync = true;
                    block.beforeLocalLastBlockHeight = localBlockHeight;
                    block.StartTimeOffset = DateTimeUtil.GetDateTimeStamp(DateTime.Now);
                }

                //BehindTime
                if (info.LocalLastBlockTime > 0 && info.LocalLastBlockHeight < info.RemoteLatestBlockHeight)
                {
                    var content = GetTimeBehindContent(info.LocalLastBlockTime);
                    if (!string.IsNullOrEmpty(content))
                        block.BehindTime = content;
                }
                else
                {
                    block.BehindTime = "";
                }

                //已经更新的区块高度
                var syncedHeight = localBlockHeight - block.beforeLocalLastBlockHeight;
                //更新区块花了多长时间
                var syncedTime = DateTimeUtil.GetDateTimeStamp(DateTime.Now) - block.StartTimeOffset;
                //当前更新进度
                if ((info.RemoteLatestBlockHeight - block.beforeLocalLastBlockHeight) != 0)
                {
                    var progress = (Convert.ToDouble(syncedHeight) / (info.RemoteLatestBlockHeight - block.beforeLocalLastBlockHeight)) * 100;
                    block.Progress = progress;
                }
                //剩余更新时间
                if (syncedHeight > 0)
                    block.TimeLeft = (syncedTime / syncedHeight) * block.BlockLeft;
            }
            return result;
        }

        public Result<List<PeerInfo>> GetPeerInfo()
        {
            ApiResponse response =  NetworkApi.GetPeerInfo().Result;
            return GetResult<List<PeerInfo>>(response);
        }

        public Result SetNetworkActive(bool isActive)
        {
            ApiResponse response =  NetworkApi.SetNetworkActive(isActive).Result;
            var result = GetResult(response);
            if (result.IsFail)
                return result;
            if (isActive)
            {
                UpdateBlocksMonitor.Default.Reset();
                UpdateBlocksMonitor.Default.Start(3000);
            }
            else
            {
                UpdateBlocksMonitor.Default.Stop();
            }
            NodeMonitor.Default.Set_NetIsActive = isActive;
            return result;
        }

        public Result<long> GetConnectionCount()
        {
            ApiResponse response =  NetworkApi.GetConnectionCount().Result;
            return GetResult<long>(response);
        }

        public Result<NetworkInfo> GetNetworkInfo()
        {
            ApiResponse response =  NetworkApi.GetNetworkInfo().Result;
            return GetResult<NetworkInfo>(response);
        }

        /// <summary>
        /// 获取区块落后时间, UTC时间
        /// </summary>
        /// <param name="lastBlockTime"></param>
        /// <returns></returns>
        public string GetTimeBehindContent(long lastBlockTime)
        {
            string result = null;
            var behindTimeSpan = DateTime.Now - DateTimeUtil.GetDateTime(lastBlockTime);
            var days = behindTimeSpan.Days;
            const int yearCount = 30 * 12;
            string content = "";
            if (days > yearCount)
            {
                var year = days / yearCount;
                content += string.Format(yearsformat, year);
                int month = 0;
                if (days % yearCount > 30)
                {
                    month = (days % yearCount) / month;
                    content += " ";
                    content += string.Format(monthsformat, month);
                }
                result = string.Format(behindformat, content);
            }
            else if (days > 30)
            {
                var month = days / 30;
                if (days % 30 > 0)
                {
                    content = string.Format(monthsformat, month);
                    if (days % 30 > 1)
                    {
                        int dayss = (days % 30);
                        content += " ";
                        content += string.Format(dayformat, dayss);
                    }
                }
                result = string.Format(behindformat, content);
            }
            else if (days > 0)
            {
                content = string.Format(dayformat, days);
                if (behindTimeSpan.Hours > 0)
                {
                    content += " ";
                    content += string.Format(hoursformat, behindTimeSpan.Hours);
                }
                if (behindTimeSpan.Milliseconds > 0)
                {
                    content += " ";
                    content += string.Format(minutesformat, behindTimeSpan.Minutes);
                }

                result = string.Format(behindformat, content);
            }
            else
            {
                if (behindTimeSpan.Hours > 0)
                {
                    content += " ";
                    content += string.Format(hoursformat, behindTimeSpan.Hours);
                }
                if (behindTimeSpan.Minutes > 0)
                {
                    content += " ";
                    content += string.Format(minutesformat, behindTimeSpan.Minutes);
                }
                if (behindTimeSpan.Seconds > 0)
                {
                    content += " ";
                    content += string.Format(secondsformat, behindTimeSpan.Seconds);
                }

                if (string.IsNullOrEmpty(content.Trim()))
                    result = "";
                else
                    result = string.Format(behindformat, content);
            }
            return result;
        }
    }
}
