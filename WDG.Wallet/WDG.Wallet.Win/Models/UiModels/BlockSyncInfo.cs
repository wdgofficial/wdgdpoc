// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Utility;
using WDG.Wallet.Win.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDG.Wallet.Win.Models
{
    public class BlockSyncInfo : NotifyBase
    {
        private long _blockLeft;
        private double _progress;
        private long _timeLeft;
        private long _startTimeOffset = 0;
        private string _behindTime;

        public long ConnectCount { get; set; }
        
        public long BlockLeft
        {
            get
            {
                return _blockLeft;
            }
            set
            {
                _blockLeft = value;
                RaisePropertyChanged("BlockLeft");
            }
        }
        
        public double Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                _progress = value;
                RaisePropertyChanged("Progress");
            }
        }
        
        public long TimeLeft
        {
            get
            {
                return _timeLeft;
            }
            set
            {
                _timeLeft = value;
                RaisePropertyChanged("TimeLeft");
            }
        }
        
        public long StartTimeOffset
        {
            get { return _startTimeOffset; }
            set { _startTimeOffset = value; RaisePropertyChanged("StartTimeOffset"); }
        }
        /// <summary>
        /// 同步之前的最大区块高度
        /// </summary>
        public long beforeLocalLastBlockHeight = 0;
        /// <summary>
        /// 是否开始同步
        /// </summary>
        public bool IsStartSync = false;

        public string BehindTime
        {
            get
            {
                return _behindTime;
            }
            set
            {
                _behindTime = value;
                RaisePropertyChanged("BehindTime");
            }
        }

        public bool IsSyncComplete()
        {
            var x = this;
            var result = x.ConnectCount >=2 && 
                            x.BlockLeft == 0 && 
                            x.StartTimeOffset >= 0 && 
                            x.beforeLocalLastBlockHeight >= 0 && 
                            (x.Progress >= 100 || x.Progress == 0);
            return result;
        }

        public bool IsSyncStart()
        {
            var blockSyncInfo = this;
            //开始同步区块
            if (blockSyncInfo.ConnectCount >= 1 || blockSyncInfo.IsStartSync || blockSyncInfo.BlockLeft > 0)
                return true;
            //区块已经到最新高度
            return (blockSyncInfo.BlockLeft == 0 && !blockSyncInfo.IsStartSync && blockSyncInfo.Progress == 100 && blockSyncInfo.StartTimeOffset >= 0);
        }

        public override string ToString()
        {
            var str = "ConnectCount = {0} " +
                "BlockLeft = {1} " +
                "Progress = {2} " +
                "StartTimeOffset = {3} " +
                "beforeLocalLastBlockHeight = {4}";

            return string.Format(str, ConnectCount, BlockLeft, Progress, StartTimeOffset, beforeLocalLastBlockHeight);
        }
    }
}
