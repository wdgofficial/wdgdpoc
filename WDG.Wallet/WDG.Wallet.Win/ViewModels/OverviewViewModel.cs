// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Biz.Monitor;
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace WDG.Wallet.Win.ViewModels
{
    public class OverviewViewModel : PopupShellBase
    {
        protected override string GetPageName()
        {
            return Pages.OverviewPage;
        }

        public OverviewViewModel()
        {
            if (IsInDesignMode)
            {

            }
            else
            {
                Init();
            }
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            AmountMonitor.Default.MonitorCallBack += walletAmountData =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    CanUseMoney = walletAmountData.CanUseAmount;

                    WaitMoney = walletAmountData.WaitAmount;

                    TotalMoney = walletAmountData.TotalAmount;
                });

                if (!AmountMonitor.Default.IsSyncComplete)
                {
                    AmountMonitor.Default.Stop();
                }
            };

            TradeRecodesMonitor.Default.MonitorCallBack += tradeRecords =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (tradeRecords.Count <= 5)
                        TradeRecords = tradeRecords;
                    else
                    {
                        TradeRecords.Clear();
                        tradeRecords.Take(5).ToList().ForEach(x => TradeRecords.Add(x));
                    }
                });
            };
        }

        private long _canUseMoney;
        private long _totalMoney;
        private long _waitMoney;
        public ObservableCollection<TradeRecordInfo> _tradeRecords;
        

        public long WaitMoney
        {
            get { return _waitMoney; }
            set
            {
                _waitMoney = value;
                RaisePropertyChanged("WaitMoney");
            }
        }

        public long CanUseMoney
        {
            get { return _canUseMoney; }
            set
            {
                _canUseMoney = value;
                RaisePropertyChanged("CanUseMoney");
            }
        }

        public long TotalMoney
        {
            get { return _totalMoney; }
            set
            {
                _totalMoney = value;
                RaisePropertyChanged("TotalMoney");
            }
        }

        public ObservableCollection<TradeRecordInfo> TradeRecords
        {
            get
            {
                if (_tradeRecords == null)
                    _tradeRecords = new ObservableCollection<TradeRecordInfo>();
                return _tradeRecords;
            }
            set
            {
                _tradeRecords = value;
                RaisePropertyChanged("TradeRecords");
            }
        }
    }
}
