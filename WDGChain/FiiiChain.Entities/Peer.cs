﻿// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.Entities
{
    public class Peer
    {
        public long Id { get; set; }

        public string IP { get; set; }

        public int Port { get; set; }

        public long PingTime { get; set; }

        public long Timestamp { get; set; }
    }
}
