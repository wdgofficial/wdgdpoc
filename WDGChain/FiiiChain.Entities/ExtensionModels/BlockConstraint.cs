// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.Entities.ExtensionModels
{
    public class BlockConstraint
    {
        public string TransactionHash;
        public long Height;
        public long LockTime;
        public bool IsCoinBase;
    }
}
