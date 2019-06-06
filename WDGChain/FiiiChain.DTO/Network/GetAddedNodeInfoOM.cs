// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.DTO
{
    public class GetAddedNodeInfoOM
    {
        public string address { get; set; }
        public string connected { get; set; }
        public long connectedTime { get; set; }
    }
}
