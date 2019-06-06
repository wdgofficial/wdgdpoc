// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDG.DTO
{
    public class PageAccountSimpleOM : OMBase
    {
        public int Count { get; set; }
        public List<AccountSimple> Accounts { get; set; }
    }

    public class PageAccountDetailOM : OMBase
    {
        public int Count { get; set; }
        public List<AccountDetailOM> Accounts { get; set; }
    }

    public class OMBase
    {

    }

    public class AccountSimple
    {
        public string Id { get; set; }
        public string Tag { get; set; }
    }
}
