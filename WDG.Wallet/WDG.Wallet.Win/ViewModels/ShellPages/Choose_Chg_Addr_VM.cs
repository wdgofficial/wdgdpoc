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
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WDG.Wallet.Win.ViewModels.ShellPages
{
    public class Choose_Chg_Addr_VM : PopupShellBase
    {
        protected override string GetPageName()
        {
            return Pages.Choose_Change_Addr_Page;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            ChooseCommand = new RelayCommand<AccountInfo>(Choose);

            RegeistMessenger<SendMsgData<AccountInfo>>(OnGetRequest);
        }

        private string _searchText;

        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; Search(); RaisePropertyChanged("SearchText"); }
        }


        private ObservableCollection<AccountInfo> _accountInfos;

        public ObservableCollection<AccountInfo> AccountInfos
        {
            get
            {
                if (_accountInfos == null)
                    _accountInfos = new ObservableCollection<AccountInfo>();
                return _accountInfos;
            }
            set
            {
                _accountInfos = value;
                RaisePropertyChanged("AccountInfos");
            }
        }

        private ObservableCollection<AccountInfo> _allAccountInfos;

        public ObservableCollection<AccountInfo> AllAccountInfos
        {
            get
            {
                if (_allAccountInfos == null)
                    _allAccountInfos = new ObservableCollection<AccountInfo>();
                return _allAccountInfos;
            }
            set
            {
                _allAccountInfos = value;
                RaisePropertyChanged("AllAccountInfos");
            }
        }


        Task runningTask = null;

        void OnGetRequest(SendMsgData<AccountInfo> msgData)
        {
            _msgData = msgData;
            InitData();
        }

        void InitData()
        {
            if (runningTask != null)
            {
                return;
            }

            Task task = new Task(() =>
            {
                var addresses = AccountsService.Default.GetPageAccountCategory(AccountsService.AccountCategory.ALL);
                if (!addresses.IsFail)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        AllAccountInfos.Clear();
                        addresses.Value.ForEach(x =>
                        {
                            AllAccountInfos.Add(x);
                        });
                    });
                }
            });
            task.ContinueWith(t =>
            {
                StaticViewModel.GlobalViewModel.IsLoading = false;
                runningTask = null;
                Search();
            });
            task.Start();
            runningTask = task;
        }


        public ICommand ChooseCommand { get; private set; }

        SendMsgData<AccountInfo> _msgData = null;

        public void Choose(AccountInfo account)
        {
            if (account == null)
                return;

            _msgData.CallBackParams = account;
            _msgData.CallBack();
        }

        public override void OnClosePopup()
        {
            _msgData.CallBack();
        }

        public void Search()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (string.IsNullOrEmpty(SearchText) || string.IsNullOrEmpty(SearchText.Trim()))
                {
                    AccountInfos.Clear();
                    foreach (var item in AllAccountInfos)
                    {
                        AccountInfos.Add(item);
                    }
                }
                else
                {
                    var searchText = SearchText.Trim();
                    AccountInfos.Clear();
                    foreach (var item in AllAccountInfos)
                    {
                        if (item.Address.Contains(searchText) || (item.Tag != null && item.Tag.Contains(searchText)))
                            AccountInfos.Add(item);
                    }
                }
            });
        }
    }
}
