// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Biz.Services;
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Models;

namespace WDG.Wallet.Win.ViewModels.ShellPages
{
    public class InputWalletPwdViewModel : PopupShellBase
    {
        protected override string GetPageName()
        {
            return Pages.InputWalletPwdPage;
        }

        public InputWalletPwdViewModel()
        {
            RegeistMessenger<SendMsgData<InputWalletPwdPageTopic>>(OnGetResponse);
            IsEnableOk = true;
        }
        
        private string _password;

        public string Password
        {
            get { return _password; }
            set { _password = value;
                if (string.IsNullOrEmpty(_password))
                    IsEnableOk = false;
                else
                    IsEnableOk = true;
                RaisePropertyChanged("Password"); }
        }

        private bool _isEnableOk;

        public bool IsEnableOk
        {
            get { return _isEnableOk; }
            set { _isEnableOk = value; RaisePropertyChanged("IsEnableOk"); }
        }


        protected override void Refresh()
        {
            base.Refresh();
            Password = string.Empty;
        }

        public override void OnOkClick()
        {
            switch (_msgData.Token)
            {
                case InputWalletPwdPageTopic.Normal:
                    _msgData.CallBack();
                    break;
                case InputWalletPwdPageTopic.UnLockWallet:
                    var result = UnLockWallet();
                    if (!result)
                        return;
                    _msgData.CallBack();
                    break;
                case InputWalletPwdPageTopic.RequestPassword:
                    var unlockResult = UnLockWallet();
                    if (!unlockResult)
                    {
                        ShowMessage(LanguageService.Default.GetLanguageValue("enterPwd_fail"));
                        return;
                    }
                    _msgData.CallBackParams = Password;
                    _msgData.CallBack();
                    break;
                default:
                    break;
            }
        }

        public override void OnClosePopup()
        {
            base.OnClosePopup();
            Password = string.Empty;
        }


        bool UnLockWallet()
        {
            var result =  WalletService.Default.UnLockWallet(Password);
            if (result.IsFail || !result.Value)
            {
                ShowMessage(LanguageService.Default.GetLanguageValue(MessageKeys.EnterPwdFail));
                return false;
            }
            return true;
        }

        private SendMsgData<InputWalletPwdPageTopic> _msgData;
        void OnGetResponse(SendMsgData<InputWalletPwdPageTopic> msgData)
        {
            this.Password = string.Empty;
            _msgData = msgData;
        }
    }
}
