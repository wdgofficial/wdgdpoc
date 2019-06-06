// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.DTO.Transaction
{
    public class FilterOM
    {
        public long StartTime;
        public long EndTime;
        public int TradeType;
        public string Account;
        public long Amount;
    }
}
