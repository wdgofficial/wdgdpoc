// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.Entities
{
    public class Input
    {
        public long Id { get; set; }

        public string TransactionHash { get; set; }

        public string OutputTransactionHash { get; set; }

        public int OutputIndex { get; set; }

        public int Size { get; set; }

        public long Amount { get; set; }

        public string UnlockScript { get; set; }

        public string AccountId { get; set; }

        public bool IsDiscarded { get; set; }

        public string BlockHash { get; set; }
    }
}
