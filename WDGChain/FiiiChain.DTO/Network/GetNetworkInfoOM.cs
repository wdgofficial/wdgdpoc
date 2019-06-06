// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.DTO
{
    public class GetNetworkInfoOM
    {
        public int version { get; set; }
        public int minimumSupportedVersion { get; set; }
        public int protocolVersion { get; set; }
        public bool isRunning { get; set; }
        public int connections { get; set; }

    }
}
