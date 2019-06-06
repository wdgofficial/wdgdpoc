// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Models;
using WDG.ServiceAgent;
using WDG.Wallet.Win.Biz.Services;
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Converters;
using WDG.Wallet.Win.Models;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WDG.Wallet.Win.ViewModels.ShellPages
{
    public class SendSettingViewModel : PopupShellBase
    {
        protected override string GetPageName()
        {
            return Pages.SendSettingPage;
        }

        protected override void OnLoaded()
        {
            Setting = new ProfessionalSetting();
            base.OnLoaded();
            RegeistMessenger<SendMsgData<ProfessionalSetting>>(OnGetData);
            ChooseUtxoCommand = new RelayCommand(ChooseUtxo);
            ChooseAddrCommand = new RelayCommand(ChooseAddr);
            RemoveUtxoCommand = new RelayCommand<PageUnspent>(RemoveUtxo);
        }

        private ProfessionalSetting _setting;

        public ProfessionalSetting Setting
        {
            get { return _setting; }
            set { _setting = value; RaisePropertyChanged("Setting"); }
        }

        SendMsgData<ProfessionalSetting> _data = null;

        public void OnGetData(SendMsgData<ProfessionalSetting> data)
        {
            Setting.UTXO = data.Token.UTXO;
            Setting.LockTime = data.Token.LockTime;
            Setting.ChangeAddress = data.Token.ChangeAddress;
            Setting.IsEnable = data.Token.IsEnable;

            _data = data;
        }

        public override void OnOkClick()
        {
            if (_data != null)
            {
                _data.Token.LockTime = Setting.LockTime;
                _data.Token.ChangeAddress = Setting.ChangeAddress;
                _data.Token.UTXO = Setting.UTXO;
                _data.Token.IsEnable = Setting.IsEnable;
                _data.CallBack();
            }

            base.OnOkClick();
        }

        public ICommand ChooseUtxoCommand { get; private set; }
        public ICommand ChooseAddrCommand { get; private set; }
        public ICommand RemoveUtxoCommand { get; private set; }

        void ChooseUtxo()
        {
            SendMsgData<PageUnspent> sendMsgData = new SendMsgData<PageUnspent>();
            sendMsgData.SetCallBack(() => {
                if (sendMsgData.CallBackParams != null && sendMsgData.CallBackParams is PageUnspent)
                {
                    Setting.UTXO.Add((PageUnspent)sendMsgData.CallBackParams);
                }
                UpdatePage(Pages.SendSettingPage);
            });
            UpdatePage(Pages.Choose_Utxo_Page);
            SendMessenger(Pages.Choose_Utxo_Page, sendMsgData);
        }

        void ChooseAddr()
        {
            SendMsgData<AccountInfo> sendMsgData = new SendMsgData<AccountInfo>();
            sendMsgData.SetCallBack(() => {
                if (sendMsgData.CallBackParams != null && sendMsgData.CallBackParams is AccountInfo)
                {
                    Setting.ChangeAddress = (AccountInfo)sendMsgData.CallBackParams;
                }
                UpdatePage(Pages.SendSettingPage);
            });
            UpdatePage(Pages.Choose_Change_Addr_Page);
            SendMessenger(Pages.Choose_Change_Addr_Page, sendMsgData);
        }

        void RemoveUtxo(PageUnspent utxo)
        {
            Setting.UTXO.Remove((PageUnspent)utxo);
        }
    }

    public class ProfessionalSetting : NotifyBase
    {
        public ProfessionalSetting()
        {
            _isEnable = false;
            LockTime = DateTime.UtcNow;
        }

        private ObservableCollection< PageUnspent> _uTXO;
        private DateTime _lockTime;
        private AccountInfo _changeAddress;
        private bool _isEnable;

        public ObservableCollection<PageUnspent> UTXO
        {
            get
            {
                if (_uTXO == null)
                    _uTXO = new ObservableCollection<PageUnspent>();
                return _uTXO;
            }
            set
            {
                _uTXO = value;
                RaisePropertyChanged("UTXO");
            }
        }
        public DateTime LockTime
        {
            get
            {
                return _lockTime;
            }
            set
            {
                _lockTime = value;
                RaisePropertyChanged("LockTime");
            }
        }
        public AccountInfo ChangeAddress
        {
            get
            {
                return _changeAddress;
            }
            set
            {
                _changeAddress = value;
                RaisePropertyChanged("ChangeAddress");
            }
        }
        public bool IsEnable
        {
            get
            {
                return _isEnable;
            }
            set
            {
                if (UTXO == null || !UTXO.Any() || ChangeAddress == null)
                {
                    _isEnable = false;
                }
                else
                {
                    _isEnable = value;
                }
                RaisePropertyChanged("IsEnable");
            }
        }

        public ProfessionalSetting Clone()
        {
            ProfessionalSetting setting = new ProfessionalSetting();
            setting.LockTime = this.LockTime;
            if (UTXO == null)
                setting.UTXO = null;
            else
            {
                setting.UTXO.Clear();
                UTXO.ToList().ForEach(utxo =>
                setting.UTXO.Add(new PageUnspent() { Amount = utxo.Amount, Account = utxo.Account, Vout = utxo.Vout, Txid = utxo.Txid, Address = utxo.Address }));
            }

            if (ChangeAddress == null)
                setting.ChangeAddress = null;
            else
                setting.ChangeAddress = new AccountInfo()
                {
                    Address = ChangeAddress.Address,
                    Balance = ChangeAddress.Balance,
                    Tag = ChangeAddress.Tag,
                };
            setting.IsEnable = this.IsEnable;
            return setting;
        }
    }
}

