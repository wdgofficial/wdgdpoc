// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Biz;
using WDG.Wallet.Win.Biz.Services;
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Models;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WDG.Wallet.Win.ViewModels
{
    public class ReceiveViewModel : VmBase
    {
        protected override string GetPageName()
        {
            return Pages.ReceivePage;
        }

        private ReceiveInfo _receiveData;

        public ReceiveInfo ReceiveData
        {
            get
            {
                if (_receiveData == null)
                    _receiveData = new ReceiveInfo();
                return _receiveData;
            }
            set { _receiveData = value; RaisePropertyChanged("ReceiveData"); }
        }

        public ICommand RequestPayCommand { get; private set; }

        public ICommand ClearCommand { get; private set; }

        public ReceiveViewModel()
        {
            RequestPayCommand = new RelayCommand<List<DependencyObject>>(OnRequestPay);
            ClearCommand = new RelayCommand<ReceiveClearType>(OnClear);
        }


        private void OnRequestPay(List<DependencyObject> dependencyObjects)
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

            if (ReceiveData.Amount <= 0)
            {
                ShowMessage(LanguageService.Default.GetLanguageValue("Error_Amount"));
                return;
            }

            if (flag)
                RequestPay();
        }
        

        void RequestPay()
        {
            if (Initializer.Default.DefaultAccount == null)
                return;

            var txsetting = FiiiCoinService.Default.GetTxSettings();
            if (txsetting.IsFail)
                return;

            if (txsetting.Value.Encrypt)
            {
                SendMsgData<InputWalletPwdPageTopic> sendMsgData = new SendMsgData<InputWalletPwdPageTopic>();
                sendMsgData.Token = InputWalletPwdPageTopic.UnLockWallet;
                sendMsgData.SetCallBack(Request);
                SendMessenger(Pages.InputWalletPwdPage, sendMsgData);
                UpdatePage(Pages.InputWalletPwdPage);
            }
            else
            {
                Request();
            }
            SendMessenger(Pages.ReceiveAddressPage, SendMessageTopic.Refresh);
        }
        
        void Request()
        {
            ReceiveData.Account = Initializer.Default.DefaultAccount.Address;
            var result = FiiiCoinService.Default.CreateNewPaymentRequest(ReceiveData);
            if (!result.IsFail)
            {
                SendMessenger(Pages.ReceiveAddressPage, SendMessageTopic.Refresh);
                SendMessenger(Pages.RequestPayPage, result.Value);
                UpdatePage(Pages.RequestPayPage);
            }
            else
            {
                ShowMessage(result.GetErrorMsg());
            }
        }



        void OnClear(ReceiveClearType receiveClearType)
        {
            switch (receiveClearType)
            {
                case ReceiveClearType.All:
                    ReceiveData.Amount_Str = "";
                    ReceiveData.Tag = string.Empty;
                    ReceiveData.Comment = string.Empty;
                    break;
                case ReceiveClearType.Tag:
                    ReceiveData.Tag = string.Empty;
                    break;
                case ReceiveClearType.Comment:
                    ReceiveData.Comment = string.Empty;
                    break;
                default:
                    break;
            }
        }
    }
}