// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.ShareModels.Models
{
    public class PoolInfo
    {
        public string PoolId { get; set; }
        public string PoolAddress { get; set; }
        public int Port { get; set; }
        public long PullTime { get; set; }
        public long MinerCount { get; set; }
    }
}
