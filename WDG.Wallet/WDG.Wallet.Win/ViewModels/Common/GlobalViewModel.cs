// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Models;
using WDG.Wallet.Win.ViewModels.ShellPages;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using System.Windows.Input;

namespace WDG.Wallet.Win.ViewModels
{
    public class GlobalViewModel : VmBase
    {
        public GlobalViewModel()
        {
            SendSettingCommand = new RelayCommand(OnSendSetting);
        }

        private WalletModel _walletModel;

        public WalletModel WalletModel
        {
            get { return _walletModel; }
            set
            {
                _walletModel = value;
                IsProfessional = _walletModel == WalletModel.Professional;
                RaisePropertyChanged("WalletModel");
            }
        }

        private bool _isProfessional;

        public bool IsProfessional
        {
            get { return _isProfessional; }
            set
            {
                _isProfessional = value;
                RaisePropertyChanged("IsProfessional");
            }
        }

        public ProfessionalSetting ProfessionalSetting = new ProfessionalSetting();

        public ICommand SendSettingCommand { get; private set; }

        void OnSendSetting()
        {
            SendMsgData<ProfessionalSetting> sendMsgData = new SendMsgData<ProfessionalSetting>();
            sendMsgData.Token = ProfessionalSetting.Clone();
            sendMsgData.SetCallBack(() =>
            {
                ProfessionalSetting = sendMsgData.Token;
            });
            SendMessenger(Pages.SendSettingPage, sendMsgData);
            UpdatePage(Pages.SendSettingPage);
        }

        public bool IsEnableProfessional
        {
            get
            {
                return IsProfessional && ProfessionalSetting != null && ProfessionalSetting.IsEnable;
            }
        }


        private bool _isLoading = false;

        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; RaisePropertyChanged("IsLoading"); }
        }


        private bool _isEnglish;

        public bool IsEnglish
        {
            get { return _isEnglish; }
            set { _isEnglish = value; RaisePropertyChanged("IsEnglish"); }
        }


        private InitStep _initStep;

        public InitStep InitStep
        {
            get { return _initStep; }
            set { _initStep = value; RaisePropertyChanged("InitStep"); }
        }

        public string VERSION
        {
            get
            {
                return "WDG Wallet Beta 1.0.2";
            }
        }

        public bool IsFirstShowProgress = true;
    }
}