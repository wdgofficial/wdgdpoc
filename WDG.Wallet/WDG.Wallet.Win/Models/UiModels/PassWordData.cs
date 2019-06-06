// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common;

namespace WDG.Wallet.Win.Models.UiModels
{
    public class PassWordData : NotifyBase
    {
        private string _pageTitle;
        private string _newPassWord1;
        private string _newPassWord2;
        private string _passWord;
        private PwdPageType _pwdPageType;

        public string NewPassWord1 { get => _newPassWord1; set { _newPassWord1 = value; RaisePropertyChanged("NewPassWord1"); } }
        public string NewPassWord2 { get => _newPassWord2; set { _newPassWord2 = value; RaisePropertyChanged("NewPassWord2"); } }
        public string PassWord { get => _passWord; set { _passWord = value; RaisePropertyChanged("PassWord"); } }
        public PwdPageType PwdPageType { get => _pwdPageType; set { _pwdPageType = value; RaisePropertyChanged("PwdPageType"); } }
        public string PageTitle { get => _pageTitle; set { _pageTitle = value; RaisePropertyChanged("PageTitle"); } }

        public void Reset()
        {
            NewPassWord1 = string.Empty;
            NewPassWord2 = string.Empty;
            PassWord = string.Empty;
        }
    }

    public enum PwdPageType
    {
        ChangePWD = 0,
        EncryptedWallet =1,
    }
}
