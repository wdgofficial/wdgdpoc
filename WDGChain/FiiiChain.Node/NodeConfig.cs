// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.Node
{
    public class NodeConfig
    {
        public string Ip { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public string Port { get; set; }

        public string WalletNotify { get; set; }
    }
}
