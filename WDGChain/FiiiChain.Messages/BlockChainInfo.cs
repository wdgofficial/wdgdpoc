// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.Messages
{
    public class BlockChainInfo
    {
        public bool IsP2PRunning { get; set; }
        public int ConnectionCount { get; set; }
        public long LastBlockHeightInCurrentNode { get; set; }
        public long LastBlockTimeInCurrentNode { get; set; }
        public long LatestBlockHeightInNetwork { get; set; }
        public long LatestBlockTimeInNetwork { get; set; }
        public int TempBlockCount { get; set; }
        public string TempBlockHeights { get; set; }

        public List<SyncTaskItem> SyncTasks { get; set; }
    }
    public class SyncTaskItem
    {
        public string IP { get; set; }
        public int Port { get; set; }
        public long StartTime { get; set; }
        public String Status { get; set; }
        public string Heights { get; set; }
    }
}
