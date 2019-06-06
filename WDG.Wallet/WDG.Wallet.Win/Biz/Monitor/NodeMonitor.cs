// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.ServiceAgent;
using WDG.Wallet.Win.Biz.Services;
using WDG.Wallet.Win.Common.Utils;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace WDG.Wallet.Win.Biz.Monitor
{
    public class NodeMonitor : ServiceMonitorBase<bool?>
    {
        private static NodeMonitor _default;

        public static NodeMonitor Default
        {
            get
            {
                if (_default == null)
                    _default = new NodeMonitor();
                return _default;
            }
        }
        
        public bool Set_NetIsActive = true;

        protected override bool? ExecTaskAndGetResult()
        {
            if (!Set_NetIsActive)
                return null;
            return NodeIsRunning();
        }

        public bool NodeIsRunning()
        {
            var ls = ProcessUtil.Find("dotnet.exe", FiiiCoinSetting.NodeRunParams);
            return ls.Any();
        }

        public void ActiveNode()
        {
            var tcpPorts = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections().Select(x => x.LocalEndPoint);
            var isActiveNode = tcpPorts.Any(x => x.Port == FiiiCoinSetting.NodeAPIPort);
            if (!isActiveNode)
                NetWorkService.Default.SetNetworkActive(true);
        }
    }
}
