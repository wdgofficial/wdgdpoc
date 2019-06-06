﻿// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.DTO
{
    public class GetBlockChainInfoOM
    {
        public bool isRunning { get; set; }
        public int connections { get; set; }
        public long localLastBlockHeight { get; set; }
        public long localLastBlockTime { get; set; }
        public int tempBlockCount { get; set; }
        public string TempBlockHeights { get; set; }
        public long remoteLatestBlockHeight { get; set; }
        public long timeOffset { get; set; }

        public List<SyncTaskItem> SyncTasks { get; set; }

    }
    public class SyncTaskItem
    {
        public string IP { get; set; }
        public int Port { get; set; }
        public DateTime StartTime { get; set; }
        public String Status { get; set; }
        public string Heights { get; set; }
    }

}
