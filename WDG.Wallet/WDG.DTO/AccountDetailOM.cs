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
    public class AccountDetailOM
    {
        public string Id { get; set; }

        public string PrivateKey { get; set; }

        public string PublicKey { get; set; }

        public long Balance { get; set; }

        public bool IsDefault { get; set; }

        public bool WatchOnly { get; set; }

        public long Timestamp { get; set; }

        public string Tag { get; set; }
    }
}
