// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Models;
using WDG.Wallet.Win.Models.UiModels;

namespace WDG.Wallet.Win.ViewModels.ShellPages
{
    public class ConfirmSendViewModel : PopupShellBase
    {
        protected override string GetPageName()
        {
            return Pages.ConfirmSendPage;
        }
        
        public override void OnOkClick()
        {
            msgData.CallBack();
            base.OnOkClick();
        }

        private ConfirmSendData confirmSendData;

        public ConfirmSendData ConfirmSendData
        {
            get { return confirmSendData; }
            set { confirmSendData = value; RaisePropertyChanged("ConfirmSendData"); }
        }


        protected override void OnLoaded()
        {
            base.OnLoaded();
            RegeistMessenger<SendMsgData<ConfirmSendData>>(OnGetResponse);
        }
        private SendMsgData<ConfirmSendData> msgData;
        void OnGetResponse(SendMsgData<ConfirmSendData> data)
        {
            msgData = data;
            ConfirmSendData = data.Token;
        }
    }
}