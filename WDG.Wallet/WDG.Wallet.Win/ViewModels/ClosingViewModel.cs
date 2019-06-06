// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.ServiceAgent;
using WDG.Utility;
using WDG.Wallet.Win.Biz.Monitor;
using WDG.Wallet.Win.Biz.Services;
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Common.Utils;
using WDG.Wallet.Win.Models;
using System;
using System.Diagnostics;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WDG.Wallet.Win.ViewModels
{
    public class ClosingViewModel : VmBase
    {
        protected override void OnLoaded()
        {
            base.OnLoaded();
            RegeistMessenger<Window>(OnClosing);
            RegeistMessenger<SendMsgData<Window>>(OnRestart);
        }

        protected override string GetPageName()
        {
            return Pages.ClosingPage;
        }

        void OnClosing(Window window)
        {
            Close(); ;

            Logger.Singleton.Debug("Node正常关闭，强杀FiiiCoin");
            Environment.Exit(0);
        }

        void OnRestart(SendMsgData<Window> data)
        {
            Close();

            App.CloseMutex();

            if (FiiiCoinSetting.CurrentNetworkType == NetworkType.Mainnet)
                Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);
            else
                Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location, " -testnet");
            Task.Delay(50);
            Logger.Singleton.Debug("Node 重启");
            Environment.Exit(0);
        }

        void Close()
        {
            StartChangeText();
            try
            {
                ServiceMonitor.StopAll();
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
            }
            finally
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                Logger.Singleton.Debug("开始计时");
                Task task = new Task(() =>
                {
                    while (true)
                    {
                        //10秒后不管有没有关闭端口都要关闭程序
                        if (stopwatch.ElapsedMilliseconds >= 10000)
                        {
                            Logger.Singleton.Debug("计时 大于10秒，关闭计时");
                            KillProcess();
                            break;
                        }

                        try
                        {
                            if (NodeMonitor.Default.NodeIsRunning())
                            {
                                Task.Run(() =>
                                {
                                    var result = EngineService.Default.AppClosed();
                                });
                                Task.Delay(1000).Wait();
                            }
                            else
                            {
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Singleton.Debug(ex.ToString());
                            KillProcess();
                            break;
                        }
                    }
                });
                task.Start();
                Task.WaitAll(task);
            }
        }

        void KillProcess()
        {
            Logger.Singleton.Debug("获取 Node 进程");
            var ids = ProcessUtil.Find("dotnet.exe", FiiiCoinSetting.NodeRunParams);
            Logger.Singleton.Debug(string.Format("强杀Node 进程，进程ID {0}", string.Join(",", ids)));
            ProcessUtil.Kill(ids);
        }

        void StartChangeText()
        {
            var text = LanguageService.Default.GetLanguageValue("Closing_caption1");
            int count = 0;
            Task task = new Task(() =>
            {
                while (true)
                {
                    count++;
                    CloseText = text + GetString(count, '.');
                    if (count == 6)
                        count = 0;
                    Task.Delay(300).Wait();
                }
            });
            task.Start();
        }

        private string GetString(int count,char c)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                stringBuilder.Append(c);
            }
            return stringBuilder.ToString();
        }

        private string _closeText;

        public string CloseText
        {
            get { return _closeText; }
            set { _closeText = value; RaisePropertyChanged("CloseText"); }
        }


    }
}
