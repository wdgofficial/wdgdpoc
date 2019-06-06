// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.DTO
{
    public class ListUnspentOM
    {
        public string txid { get; set; }
        public int vout { get; set; }
        public string address { get; set; }
        public string account { get; set; }
        public string scriptPubKey { get; set; }
        public string redeemScript { get; set; }
        public long amount { get; set; }
        public long confirmations { get; set; }
        public bool spendable { get; set; }
        public bool solvable { get; set; }
    }
}
