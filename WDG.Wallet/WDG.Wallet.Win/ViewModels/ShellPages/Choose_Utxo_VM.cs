// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

using WDG.Models;
using WDG.Wallet.Win.Biz.Services;
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Models;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WDG.Wallet.Win.ViewModels.ShellPages
{
    public class Choose_Utxo_VM : PopupShellBase
    {
        protected override string GetPageName()
        {
            return Pages.Choose_Utxo_Page;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            ChooseCommand = new RelayCommand<PageUnspent>(Choose);
            SearchCommand = new RelayCommand<List<DependencyObject>>(OnSearch);

            RegeistMessenger<SendMsgData<PageUnspent>>(OnGetRequest);
        }

        private int _currentPage;
        private int _pageCount;
        private double _minAmount = 0;
        private double _maxAmount = 10000;


        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                if (_currentPage == value)
                    return;
                _currentPage = value;
                InitData();
                RaisePropertyChanged("CurrentPage");
            }
        }

        public int PageCount
        {
            get { return _pageCount; }
            set
            {
                if (value <= 0)
                    return;
                _pageCount = value;
                RaisePropertyChanged("PageCount");
            }
        }

        public double MinAmount
        {
            get
            {
                return _minAmount;
            }
            set
            {
                _minAmount = value;
                RaisePropertyChanged("MinAmount");
            }
        }

        public double MaxAmount
        {
            get
            {
                return _maxAmount;
            }
            set
            {
                _maxAmount = value;
                RaisePropertyChanged("MaxAmount");
            }
        }


        private ObservableCollection<PageUnspent> _utxos;

        public ObservableCollection<PageUnspent> Utxos
        {
            get
            {
                if (_utxos == null)
                    _utxos = new ObservableCollection<PageUnspent>();
                return _utxos;
            }
            set
            {
                Utxos = value;
                RaisePropertyChanged("Utxos");
            }
        }


        public ICommand ChooseCommand { get; private set; }
        public ICommand SearchCommand { get; private set; }


        const int PageSize = 20;
        SendMsgData<PageUnspent> _msgData = null;
        Task runningTask = null;

        

        void OnGetRequest(SendMsgData<PageUnspent> msgData)
        {
            _msgData = msgData;
            CurrentPage = 1;
            InitData();
        }

        const double pow = 100000000;

        SendSettingViewModel settingModel = null;

        void InitData()
        {
            if (runningTask != null)
            {
                return;
            }
            if (settingModel == null)
                settingModel = BootStrapService.Default.GetPage(Pages.SendSettingPage).DataContext as SendSettingViewModel;
            StaticViewModel.GlobalViewModel.IsLoading = true;
            Task task = new Task(() =>
            {
                var min = Convert.ToInt64(MinAmount * pow);
                var max = Convert.ToInt64(MaxAmount * pow);
                var listUnspentResult = UtxoService.Default.ListPageUnspent(7, CurrentPage, PageSize, 9999999, min, max, true);
                if (!listUnspentResult.IsFail)
                {
                    var itemCount = Convert.ToInt32(listUnspentResult.Value.Count);
                    var pageCount = itemCount / PageSize;
                    if (itemCount % PageSize > 0)
                        pageCount++;
                    PageCount = pageCount;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Utxos.Clear();
                        var items = listUnspentResult.Value.UnspentList;
                        var utxos = settingModel.Setting.UTXO;
                        if (utxos != null)
                            items = items.Where(x => !utxos.Any(u => u.Txid == x.Txid && u.Vout == x.Vout)).ToList();
                        items.ForEach(x => Utxos.Add(x) );
                        runningTask = null;
                    });
                }
            });

            task.ContinueWith(t =>
            {
                StaticViewModel.GlobalViewModel.IsLoading = false;
                runningTask = null;
            });

            task.Start();
            runningTask = task;
        }

        public void Choose(PageUnspent output)
        {
            if (output == null)
                return;
            
            _msgData.CallBackParams = output;
            _msgData.CallBack();
        }

        private void OnSearch(List<DependencyObject> dependencyObjects)
        {
            var flag = true;
            if (dependencyObjects != null)
            {
                foreach (var item in dependencyObjects)
                {
                    if (Validation.GetHasError(item))
                    {
                        flag = false;
                        break;
                    }
                }
            }

            if (!flag)
                return;

            InitData();
        }

        public override void OnClosePopup()
        {
            _msgData.CallBack();
        }
    }
}