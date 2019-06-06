// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.DTO
{
    public class AccountOM
    {
        public string Address { get; set; }

        public string PublicKey { get; set; }

        public long Balance { get; set; }

        public bool IsDefault { get; set; }

        public bool WatchOnly { get; set; }

        public string Tag { get; set; }
    }
}
