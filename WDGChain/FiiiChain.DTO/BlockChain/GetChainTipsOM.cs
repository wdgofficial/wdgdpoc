// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.DTO
{
    public class GetChainTipsOM
    {
        public long height { get; set; }
        public string hash { get; set; }
        public long branchLen { get; set; }
        public string status { get; set; }
    }
}
