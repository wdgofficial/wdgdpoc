// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Biz.Services;
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Models;
using WDG.Wallet.Win.Models.UiModels;
using WDG.Wallet.Win.ValidationRules;
using GalaSoft.MvvmLight.Messaging;
using System;

namespace WDG.Wallet.Win.ViewModels.ShellPages
{
    public class PasswordSettingViewModel : PopupShellBase
    {
        protected override string GetPageName()
        {
            return Pages.PasswordSettingPage;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            PassWordData = new PassWordData();

            RegeistMessenger<string>(OnGetRequest);
        }

        private PassWordData _passWordData;

        public PassWordData PassWordData
        {
            get { return _passWordData; }
            set { _passWordData = value; RaisePropertyChanged("PassWordData"); }
        }
        
        private int _displayControlNum ;

        public int DisplayControlNum
        {
            get { return _displayControlNum ; }
            set { _displayControlNum  = value; RaisePropertyChanged("DisplayControlNum"); }
        }


        void OnGetRequest(string @params)
        {
            if (!Enum.IsDefined(typeof(PwdPageType), @params))
                return;
            var pwdPageType = (PwdPageType)Enum.Parse(typeof(PwdPageType), @params);
            PassWordData.PwdPageType = pwdPageType;
            DisplayControlNum = (int)pwdPageType;
            switch (pwdPageType)
            {
                case PwdPageType.ChangePWD:
                    PassWordData.PageTitle = LanguageService.Default.GetLanguageValue("page_PwdSetting");
                    break;
                case PwdPageType.EncryptedWallet:
                    PassWordData.PageTitle = LanguageService.Default.GetLanguageValue("page_PwdSetting_EncryptedWallet");
                    break;
                default:
                    break;
            }
        }

        protected override void Refresh()
        {
            PassWordData = new PassWordData();
            base.Refresh();
        }

        public override void OnOkClick()
        {
            if (PassWordData.PwdPageType == PwdPageType.ChangePWD &&  PassWordData.PassWord == PassWordData.NewPassWord2)
            {
                ShowMessage(LanguageService.Default.GetLanguageValue(ValidationType.Error_NewPasswordDifferent.ToString()));
                return;
            }

            if (PassWordData.NewPassWord1 != PassWordData.NewPassWord2)
            {
                ShowMessage(LanguageService.Default.GetLanguageValue(ValidationType.Error_PasswordDifferent.ToString()));
                return;
            }

            var messageData = new MessagePageData();
            messageData.MsgType = MsgType.Warwarning;
            messageData.PageTitle = LanguageService.Default.GetLanguageValue("messagePage_title_confirmPwd");
            messageData.MsgItems.Add(new MsgData(LanguageService.Default.GetLanguageValue("messagePage_title_confirmPwd_cation"), "#F65952"));
            messageData.MsgItems.Add(new MsgData(LanguageService.Default.GetLanguageValue("messagePage_title_confirmPwd_cation2"), "#333333"));
            switch (PassWordData.PwdPageType)
            {
                case PwdPageType.ChangePWD:
                    var unlockreuslt = CheckOldPassword(PassWordData.PassWord);
                    if (!unlockreuslt)
                    {
                        ShowMessage(LanguageService.Default.GetLanguageValue("Error_enterOldPwd"));
                        return;
                    }
                    messageData.SetOkCallBack(ShowResult);
                    break;
                case PwdPageType.EncryptedWallet:
                    messageData.SetOkCallBack(EncryptWallet);
                    messageData.SetCancelCallBack(() => { PassWordData.Reset(); });
                    break;
                default:
                    break;
            }
            
            
            SendMessenger(Pages.MessagePage,messageData);
            UpdatePage(Pages.MessagePage);
        }


        bool CheckOldPassword(string password)
        {
            var result = WalletService.Default.UnLockWallet(password);
            if (!result.IsFail && result.Value)
            {
                WalletService.Default.LockWallet();
                return true;
            }
            return false;
        }

        public override void OnClosePopup()
        {
            base.OnClosePopup();
            PassWordData.Reset();
        }

        

        void ShowResult()
        {
            var result = WalletService.Default.ChangeWalletPassword(PassWordData.PassWord, PassWordData.NewPassWord1);
            if (!result.IsFail)
            {
                ShowMessage(LanguageService.Default.GetLanguageValue("page_changePwd_okCaption"));
                OnClosePopup();
            }
            else
            {
                ShowMessage(LanguageService.Default.GetLanguageValue("Error_enterOldPwd"));
                PassWordData.Reset();
            }
        }

        public override void OnCancelClick()
        {
            base.OnCancelClick();
        }

        void EncryptWallet()
        {
            var result = WalletService.Default.EncryptWallet(PassWordData.NewPassWord1);
            if (!result.IsFail)
            {
                var Msg = LanguageService.Default.GetLanguageValue("encryptSucceed");
                ShowMessage(Msg);
                Messenger.Default.Send(CommonTopic.UpdateWalletStatus);
                PassWordData.Reset();
            }
            else
            {
                PassWordData.Reset();
                var errorMsg = result.GetErrorMsg();
                if (string.IsNullOrEmpty(errorMsg))
                    errorMsg = LanguageService.Default.GetLanguageValue("encryptFail");
                ShowMessage(errorMsg);
            }
        }
    }
}