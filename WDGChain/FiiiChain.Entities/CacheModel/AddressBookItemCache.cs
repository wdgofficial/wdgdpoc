// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.Entities.CacheModel
{
    public class AddressBookItemCache
    {
        public int Id { get; set; }
        public long AddressId { get; set; }
        public string Address { get; set; }
        public string Tag { get; set; }
        public long Timestamp { get; set; }

        public override string ToString()
        {
            return string.Format("{0}", Address);
        }
    }
}
