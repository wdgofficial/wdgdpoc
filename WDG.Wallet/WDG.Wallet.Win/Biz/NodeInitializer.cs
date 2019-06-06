// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.ServiceAgent;
using WDG.Utility;
using WDG.Wallet.Win.Biz.Monitor;
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Models;
using WDG.Wallet.Win.ViewModels;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;

namespace WDG.Wallet.Win.Biz
{
    public class NodeInitializer : InstanceBase<NodeInitializer>
    {
        private string _targetDir;
        public bool Set_NetIsActive;

        public void StartNode()
        {
            _targetDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Node");
            if (!Directory.Exists(_targetDir))
                Application.Current.MainWindow.Close();
            NodeMonitor.Default.MonitorCallBack += StartNode;
            NodeMonitor.Default.Start(1000);
        }

        void StartNode(bool? portIsUse)
        {
            if (portIsUse.HasValue && portIsUse.Value)
            {
                StaticViewModel.GlobalViewModel.InitStep = InitStep.NetConnectting;
                return;
            }

            if (Set_NetIsActive)
            {
                Thread.Sleep(3000);
                return;
            }
            try
            {
                Process p = new Process();
                p.StartInfo.WorkingDirectory = _targetDir;
                p.StartInfo.FileName = "dotnet.exe";
                p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
                p.StartInfo.CreateNoWindow = true;//不显示程序窗口
                p.StartInfo.Arguments = FiiiCoinSetting.NodeRunParams;
                p.StartInfo.Verb = "RunAs";
                Logger.Singleton.Info(p.StartInfo.Arguments);
                p.Start();//启动程序
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
            }
        }
    }
}