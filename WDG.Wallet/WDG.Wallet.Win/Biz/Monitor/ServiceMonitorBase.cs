// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Utility;
using WDG.Wallet.Win.Common.interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WDG.Wallet.Win.Biz.Monitor
{
    public abstract class ServiceMonitorBase<T> : IServiceMonitor<T>
    {
        public event MonitorCallBackHandle<T> MonitorCallBack;

        bool isStop = true;
        int sleepInterval;
        public void Start(int interval)
        {
            sleepInterval = interval;
            if (!isStop)
            {
                return;
            }
            if (!ServiceMonitor.Monitors.Contains(this))
                ServiceMonitor.Monitors.Add(this);
            isStop = false;
            Task task = new Task(() =>
            {
                CyclicTask();
            });
            task.Start();
        }

        public void Stop()
        {
            isStop = true;
        }

        private void CyclicTask()
        {
            while (!isStop)
            {
                try
                {
                    if (MonitorCallBack == null)
                        continue;

                    var result = ExecTaskAndGetResult();
                    if (result != null)
                    {
                        MonitorCallBack.Invoke(result);
                    }

                    Thread.Sleep(sleepInterval);
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Logger.Singleton.Error(ex.ToString());
                    });
                }
            }
        }

        protected abstract T ExecTaskAndGetResult();

        protected void CallBack(T innerParmas)
        {
            if (MonitorCallBack == null)
                return;

            try
            {
                MonitorCallBack.Invoke(innerParmas);
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Logger.Singleton.Error(ex.ToString());
                });
            }

        }
    }
}
