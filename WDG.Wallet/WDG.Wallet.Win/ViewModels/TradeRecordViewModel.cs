// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.DTO;
using WDG.Utility.Helper;
using WDG.Wallet.Win.Biz.Monitor;
using WDG.Wallet.Win.Biz.Services;
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Common.Utils;
using WDG.Wallet.Win.Converters;
using WDG.Wallet.Win.Models;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WDG.Wallet.Win.ViewModels
{
    public class TradeRecordViewModel : VmBase
    {
        private int _timeSelectIndex = 0;
        private int _tradeSelectIndex = 0;
        private string _searchText;
        private string _searchAmount;
        private double _searchAmountValue;
        private bool _isShowTimeRange = false;
        private ObservableCollection<TradeRecordInfo> _tradeRecords;
        private string _refreshTitle;
        private int _timerCount = 10;

        public int TimeSelectIndex
        {
            get { return _timeSelectIndex; }
            set
            {
                _timeSelectIndex = value;
                OnTimeSelectIndexChanged(value);
                RaisePropertyChanged("TimeSelectIndex");
            }
        }

        public int TradeSelectIndex
        {
            get { return _tradeSelectIndex; }
            set
            {
                _tradeSelectIndex = value;
                RaisePropertyChanged("TradeSelectIndex");
            }
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                RaisePropertyChanged("SearchText");
            }
        }

        public string SearchAmount
        {
            get { return _searchAmount; }
            set
            {
                if (_searchAmount == value)
                {
                    RaisePropertyChanged("SearchAmount");
                    return;
                }
                _searchAmount = value;
                if (!string.IsNullOrEmpty(value))
                    _searchAmountValue = double.Parse(value);
                else
                    _searchAmountValue = 0;

                RaisePropertyChanged("SearchAmount");
            }
        }

        public bool IsShowTimeRange
        {
            get { return _isShowTimeRange; }
            set
            {
                _isShowTimeRange = value;
                RaisePropertyChanged("IsShowTimeRange");
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


        public ICommand MouseDubleClickCommand { get; private set; }
        public ICommand ExportCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }
        public ICommand CopyUriCommand { get; private set; }
        public ICommand ScrollChangedCommand { get; private set; }

        private DateTime _startDate;

        public DateTime StartDate
        {
            get { return _startDate; }
            set { _startDate = value; RaisePropertyChanged("StartDate"); }
        }

        private DateTime _endDate;

        public DateTime EndDate
        {
            get { return _endDate; }
            set { _endDate = value; RaisePropertyChanged("EndDate"); }
        }

        public string RefreshTitle
        {
            get
            {
                return _refreshTitle;
            }
            set
            {
                _refreshTitle = value;
                RaisePropertyChanged("RefreshTitle");
            }
        }

        private TradeRecordInfo _selectedItem;

        public TradeRecordInfo SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged("SelectedItem");
            }
        }

        private void OnTimeSelectIndexChanged(int index)
        {
            if (index == 6)
                IsShowTimeRange = true;
            else
                IsShowTimeRange = false;
        }

        private void OnMouseDubleClick(TradeRecordInfo tradeRecordInfo)
        {
            if (tradeRecordInfo == null)
                return;
            switch (tradeRecordInfo.Payment.Category)
            {
                case "self":
                case "send":
                    SendMessenger(Pages.TradeDetailSendPage, tradeRecordInfo);
                    UpdatePage(Pages.TradeDetailSendPage);
                    break;
                case "receive":
                    SendMessenger(Pages.TradeDetailReceivePage, tradeRecordInfo);
                    UpdatePage(Pages.TradeDetailReceivePage);
                    break;

                case "generate":
                    SendMessenger(Pages.TradeDetailMiningPage, tradeRecordInfo);
                    UpdatePage(Pages.TradeDetailMiningPage);
                    break;
                default:
                    break;
            }
        }

        Timer refreshTimer = null;

        protected override void OnLoaded()
        {
            TradeRecords = new ObservableCollection<TradeRecordInfo>();
            StartDate = DateTime.Now.AddYears(-10);
            EndDate = DateTime.Now;
            base.OnLoaded();

            MouseDubleClickCommand = new RelayCommand<TradeRecordInfo>(OnMouseDubleClick);
            ExportCommand = new RelayCommand<DataGrid>(OnExport);
            RefreshCommand = new RelayCommand(OnRefresh);
            CopyUriCommand = new RelayCommand<string>(OnCopyUri);

            RegeistMessenger<bool>(OnScrollChanged);
            refreshTimer = new Timer();
            refreshTimer.Interval = 1000;
            refreshTimer.Elapsed += (s, e) => {
                if (_timerCount <= 0)
                    return;
                _timerCount--;
                if (_timerCount == 0)
                {
                    refreshTimer.Stop();
                    RefreshTitle = LanguageService.Default.GetLanguageValue("page_tradeRecord_refresh");
                    RefreshRecords(false);
                }
                else
                {
                    RefreshTitle = string.Format(Formats.RefreshFormat, _timerCount);
                }
            };
            refreshTimer.Start();
        }

        protected void OnCopyUri(string id)
        {
            if (string.IsNullOrEmpty(id) || SelectedItem == null)
                return;

            if (id == "address")
                ClipboardUtil.SetText(SelectedItem.Payment.Address);
            else if (id == "amount")
                ClipboardUtil.SetText((SelectedItem.Payment.Amount / Math.Pow(10, 8)).ToString("0.00000000"));
            else if (id == "label")
            {
                PaymentToMarkConverter converter = new PaymentToMarkConverter();
                var mark = converter.Convert(SelectedItem.Payment, typeof(object), null, new System.Globalization.CultureInfo(1033));
                if (mark != null)
                    ClipboardUtil.SetText(mark.ToString());
            }
            else if (id == "txid")
                ClipboardUtil.SetText(SelectedItem.Payment.TxId);
        }

        protected override string GetPageName()
        {
            return Pages.TradeRecordPage;
        }

        protected override void Refresh()
        {
            base.Refresh();

            TimeSelectIndex = 0;
            TradeSelectIndex = 0;
            SearchText = null;
            SearchAmount = "";
        }

        void OnExport(DataGrid dataGrid1)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV（*.csv）|*.csv";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;
            var result = saveFileDialog.ShowDialog(BootStrapService.Default.Shell.GetWindow());
            if (result.HasValue && result.Value)
            {
                var content = TradeRecords.GetCsvContent();
                var file = saveFileDialog.FileName;
                try
                {
                    using (Stream stream = File.Create(file))
                    {
                        using (var writer = new StreamWriter(stream, System.Text.Encoding.Unicode))
                        {
                            var data = content.Replace(",", "\t");
                            writer.Write(data);
                            writer.Close();
                        }
                        stream.Close();
                    }
                    ShowMessage(LanguageService.Default.GetLanguageValue(MessageKeys.Export_Sucesses));
                }
                catch (Exception ex)
                {
                    ShowMessage(LanguageService.Default.GetLanguageValue(MessageKeys.Export_Fail));
                }
                
            }
        }

        void OnRefresh()
        {
            RefreshRecords(false);
        }

        void RefreshRecords(bool showLoading = true)
        {
            refreshTimer.Stop();
            var count = TradeRecords.Count;
            if (count < 20)
                count = 20;
            if (showLoading)
                StaticViewModel.GlobalViewModel.IsLoading = true;
            Task task = new Task(() =>
            {
                var filterData = GetFilterCondition();
                var tradeRecordsResult = FiiiCoinService.Default.ListFilterTrans(filterData, 0, true, count);
                if (!tradeRecordsResult.IsFail)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        TradeRecords.Clear();
                        tradeRecordsResult.Value.ToList().ForEach(x => TradeRecords.Add(x));
                    });
                }
            });
            task.ContinueWith(t =>
            {
                if (showLoading)
                    StaticViewModel.GlobalViewModel.IsLoading = false;
                _timerCount = 10;
                refreshTimer.Start();
            });
            task.Start();
        }

        void OnScrollChanged(bool isdd)
        {
            StaticViewModel.GlobalViewModel.IsLoading = true;
            Task task = new Task(() =>
            {
                var filterData = GetFilterCondition();
                var tradeRecordsResult = FiiiCoinService.Default.ListFilterTrans(filterData, TradeRecords.Count, true, 20);
                if (!tradeRecordsResult.IsFail)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        tradeRecordsResult.Value.ToList().ForEach(x => TradeRecords.Add(x));
                    });
                }
            });
            task.ContinueWith(t => {
                StaticViewModel.GlobalViewModel.IsLoading = false;
            });
            task.Start();
        }

        private FilterIM GetFilterCondition()
        {
            FilterIM filterData = new FilterIM { StartTime = -1, EndTime = -1, TradeType = 0 };
            var date = DateTime.Now;
            switch (TimeSelectIndex)
            {
                case 1://today
                    {
                        var startdate = date;
                        filterData.StartTime = Time.GetEpochTime(startdate.Year, startdate.Month, startdate.Day, 0, 0, 0);
                        var endDate = date.AddDays(1);
                        filterData.EndTime = Time.GetEpochTime(endDate.Year, endDate.Month, endDate.Day, 0, 0, 0);
                    }
                    break;
                case 2://thisWeek
                    {
                        var startdate = date.AddDays(-(int)date.DayOfWeek);
                        filterData.StartTime = Time.GetEpochTime(startdate.Year, startdate.Month, startdate.Day, 0, 0, 0);
                        var endDate = date.AddDays(-(int)date.DayOfWeek + 7);
                        filterData.EndTime = Time.GetEpochTime(endDate.Year, endDate.Month, endDate.Day, 0, 0, 0);
                    }
                    break;
                case 3://thismonth
                    {
                        var startdate = date.AddDays(-date.Day +1);
                        filterData.StartTime = Time.GetEpochTime(startdate.Year, startdate.Month, startdate.Day, 0, 0, 0);
                        var endDate = date.AddDays(-(int)date.Day + 1).AddMonths(1);
                        filterData.EndTime = Time.GetEpochTime(endDate.Year, endDate.Month, endDate.Day, 0, 0, 0);
                    }
                    break;
                case 4://prevMon
                    {
                        var startdate = date.AddDays(-date.Day +1).AddMonths(-1);
                        filterData.StartTime = Time.GetEpochTime(startdate.Year, startdate.Month, startdate.Day, 0, 0, 0);
                        var endDate = startdate.AddMonths(1);
                        filterData.EndTime = Time.GetEpochTime(endDate.Year, endDate.Month, endDate.Day, 0, 0, 0);
                    }
                    break;
                case 5://thisYear
                    {
                        var startdate = date;
                        filterData.StartTime = Time.GetEpochTime(startdate.Year, 1, 1, 0, 0, 0);
                        var endDate = startdate.AddYears(1);
                        filterData.EndTime = Time.GetEpochTime(endDate.Year, 1, 1, 0, 0, 0);
                    }
                    break;
                case 6://other
                    {
                        var startdate = StartDate;
                        filterData.StartTime = Time.GetEpochTime(startdate.Year, startdate.Month, startdate.Day, 0, 0, 0);
                        var endDate = EndDate;
                        filterData.EndTime = Time.GetEpochTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);
                    }
                    break;
                default:
                    break;
            }

            switch (TradeSelectIndex)
            {
                case 1: filterData.TradeType = 1; break;
                case 2: filterData.TradeType = 2; break;
                case 3: filterData.TradeType = 3; break;
                case 4: filterData.TradeType = 4; break;
                default: break;
            }

            if (SearchText!=null && !string.IsNullOrEmpty(SearchText.Trim()) )
            {
                filterData.Account = SearchText.Trim();
            }

            if (SearchAmount != null && !string.IsNullOrEmpty(SearchAmount.Trim()))
            {
                double amount = 0;
                if (double.TryParse(SearchAmount, out amount))
                {
                    filterData.Amount = Convert.ToInt64(amount * Math.Pow(10, 8));
                }
            }

            return filterData;
        }
    }
}
