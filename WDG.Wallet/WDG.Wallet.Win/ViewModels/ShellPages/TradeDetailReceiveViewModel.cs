// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Models;

namespace WDG.Wallet.Win.ViewModels.ShellPages
{
    public class TradeDetailReceiveViewModel : PopupShellBase
    {
        protected override string GetPageName()
        {
            return Pages.TradeDetailReceivePage;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            RegeistMessenger<TradeRecordInfo>(GetRequest);
        }

        void GetRequest(TradeRecordInfo tradeRecordInfo)
        {
            TradeRecordInfo = tradeRecordInfo;
        }


        private TradeRecordInfo _tradeRecordInfo;

        public TradeRecordInfo TradeRecordInfo
        {
            get { return _tradeRecordInfo; }
            set
            {
                _tradeRecordInfo = value;
                RaisePropertyChanged("TradeRecordInfo");
            }
        }
    }
}
