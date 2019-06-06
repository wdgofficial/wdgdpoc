// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDG.DTO
{
    public class FilterIM
    {
        public long StartTime = -1;
        public long EndTime = -1;
        public int TradeType = 0;
        public string Account = "*";
        public long Amount = -1;
    }
}
