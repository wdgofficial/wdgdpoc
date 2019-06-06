// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Models;
using WDG.Wallet.Win.ValidationRules;
using System;

namespace WDG.Wallet.Win.ViewModels.ShellPages
{
    public class OpenUrlViewModel : PopupShellBase
    {
        public OpenUrlViewModel()
        {

        }

        private string _url;
        private bool _isUrlError = false;

        public string Url
        {
            get { return _url; }
            set { _url = value; RaisePropertyChanged("Url"); }
        }

        public bool IsUrlError
        {
            get { return _isUrlError; }
            set { _isUrlError = value; RaisePropertyChanged("IsUrlError"); }
        }

        protected override string GetPageName()
        {
            return Pages.OpenUrlPage;
        }

        public override void OnOkClick()
        {
            if (string.IsNullOrEmpty(Url))
                return;

            var @params = Url.Replace("wdgcoin:", "").Replace("amount=", "").Replace("label=", "").Replace("message=", "").Replace("?", "&").Split('&');

            if (@params.Length != 4)
            {
                ShowMessage(LanguageService.Default.GetLanguageValue(MessageKeys.Error_Uri));
                return;
            }
            long longAmount;
            if (!long.TryParse(@params[1], out longAmount))
            {
                ShowMessage(LanguageService.Default.GetLanguageValue(ValidationType.Error_Amount.ToString()));
                return;
            }

            SendMsgData<SendItemInfo> data = new SendMsgData<SendItemInfo>();
            data.Token = new SendItemInfo();
            data.Token.Address = @params[0];
            data.Token.Tag = @params[2];
            data.Token.PayAmountStr = (longAmount / Math.Pow(10, 8)).ToString("0.00000000");
            SendMessenger(Pages.SendPage, data);
            UpdatePage(Pages.SendPage, PageModel.TabPage);
            base.OnOkClick();
            Url = string.Empty;
        }
    }
}
