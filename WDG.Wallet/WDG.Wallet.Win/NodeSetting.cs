// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using FiiiCoin.Wallet.Win.Biz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiCoin.Wallet.Win
{
    public class NodeSetting
    {
        public static NetworkType CurrentNetworkType = NetworkType.TestNetPort;

        private const int NodeAPIPort_Test = 5006;
        private const int NodeAPIPort_Main = 5007;

        public static int NodeAPIPort
        {
            get
            {
                var port = CurrentNetworkType == NetworkType.TestNetPort ? NodeAPIPort_Test : NodeAPIPort_Main;
                return port;
            }
        }

        private const int NodePort_Test = 53111;
        private const int NodePort_Main = 53222;
        public static int NodePort
        {
            get
            {
                var port = CurrentNetworkType == NetworkType.TestNetPort ? NodePort_Test : NodePort_Main;
                return port;
            }
        }
    }
}
