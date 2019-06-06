// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDG.Wallet.Win
{
    public enum NetworkType
    {
        Mainnet,
        TestNet
    }
    
    public class FiiiCoinSetting
    {
        private static NetworkType _currentNetworkType = NetworkType.TestNet;

        public static NetworkType CurrentNetworkType
        {
            get
            {
                return _currentNetworkType;
            }
            set
            {
                _currentNetworkType = value;
                WalletNetwork.SetNetWork(_currentNetworkType == NetworkType.TestNet);
            }
        }

        private const int NodeAPIPort_Test = 20022;
        private const int NodeAPIPort_Main = 20021;

        private const int NodePort_Test = 20312;
        private const int NodePort_Main = 20111;

        public static int NodeAPIPort
        {
            get
            {
                var port = CurrentNetworkType == NetworkType.TestNet ? NodeAPIPort_Test : NodeAPIPort_Main;
                return port;
            }
        }
        
        public static int NodePort
        {
            get
            {
                var port = CurrentNetworkType == NetworkType.TestNet ? NodePort_Test : NodePort_Main;
                return port;
            }
        }

        public static string NodeApiUrl
        {
            get
            {
                var apiUrl = string.Format("http://localhost:{0}", NodeAPIPort);
                return apiUrl;
            }
        }

        public static string NodeRunParams
        {
            get
            {
                var result = "FiiiChain.Node.dll";
                switch (CurrentNetworkType)
                {
                    case NetworkType.Mainnet:
                        result += "";
                        break;
                    case NetworkType.TestNet:
                        result += " -testnet";
                        break;
                    default:
                        break;
                }
                return result;
            }
        }

        public static string NodeTypeStr
        {
            get
            {
                var result = "";
                switch (CurrentNetworkType)
                {
                    case NetworkType.Mainnet:
                        result = "mainnet";
                        break;
                    case NetworkType.TestNet:
                        result = "testnet";
                        break;
                    default:
                        break;
                }
                return result;
            }
        }
    }
}