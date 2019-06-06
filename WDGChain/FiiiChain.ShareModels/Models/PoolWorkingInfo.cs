// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.ShareModels.Models
{
    public class PoolWorkingInfo
    {
        public long HashRates { get; set; }
        public long PushTime { get; set; }
        public string[] Miners { get; set; }
    }
}