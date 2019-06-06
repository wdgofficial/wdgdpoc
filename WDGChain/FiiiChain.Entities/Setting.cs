// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.Entities
{
    public class Setting
    {
        public long Confirmations { get; set; }
        public long FeePerKB { get; set; }
        public bool Encrypt { get; set; }
        public string PassCiphertext { get; set; }
    }
}
