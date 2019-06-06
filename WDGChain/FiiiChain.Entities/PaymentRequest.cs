// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.Entities
{
    public class PaymentRequest
    {
        public long Id { get; set; }
        public string AccountId { get; set; }
        public string Tag { get; set; }
        public string Comment { get; set; }
        public long Amount { get; set; }
        public long Timestamp { get; set; }
    }
}
