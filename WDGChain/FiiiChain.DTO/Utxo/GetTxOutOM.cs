// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.DTO
{
    public class GetTxOutOM
    {
        public string bestblock { get; set; }
        public long confirmations { get; set; }
        public long value { get; set; }
        public string scriptPubKey { get; set; }
        public int version { get; set; }
        public bool coinbase { get; set; }
    }
}
