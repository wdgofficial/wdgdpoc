// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Models;
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Converters;
using WDG.Wallet.Win.Models;
using System;

namespace WDG.Wallet.Win.ViewModels.ShellPages
{
    public class TradeDetailSendViewModel : PopupShellBase
    {
        protected override string GetPageName()
        {
            return Pages.TradeDetailSendPage;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            RegeistMessenger<TradeRecordInfo>(GetRequest);
        }

        void GetRequest(TradeRecordInfo tradeRecordInfo)
        {
            //TradeRecordInfo = tradeRecordInfo;
            TradeRecordDetail = new TradeRecordDetail(tradeRecordInfo.Payment);
        }

        private TradeRecordDetail _tradeRecordDetail;

        public TradeRecordDetail TradeRecordDetail
        {
            get { return _tradeRecordDetail; }
            set { _tradeRecordDetail = value; RaisePropertyChanged("TradeRecordDetail"); }
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


    public class TradeRecordDetail : Payment
    {
        public TradeRecordDetail(Payment payment)
        {
            if (payment == null)
                return;

            TradeFee = payment.Fee;
            base.Confirmations = payment.Confirmations;
            base.Time = payment.Time;
            base.Fee = payment.Fee;
            base.TxId = payment.TxId;
            base.TotalInput = payment.TotalInput;
            base.TotalOutput = payment.TotalOutput;
            base.Category = payment.Category;
            base.Address = payment.Address;

            var categoryType = (PaymentCategoryType)Enum.Parse(typeof(PaymentCategoryType), payment.Category);
            var converter = new PaymentToMarkConverter();
            switch (categoryType)
            {
                case PaymentCategoryType.generate:
                    break;
                case PaymentCategoryType.receive:
                    break;
                case PaymentCategoryType.send:
                    var mark = converter.Convert(payment, typeof(object), null, new System.Globalization.CultureInfo(1033));
                    if (mark != null)
                        To = mark.ToString();
                    TradeAmount = payment.Amount;
                    ArrivalAmount = payment.TotalOutput;
                    break;
                case PaymentCategoryType.self:
                    To = LanguageService.Default.GetLanguageValue("converter_disabled");
                    TradeAmount = payment.TotalOutput;
                    ArrivalAmount = payment.TotalOutput;
                    Address = LanguageService.Default.GetLanguageValue("self");
                    break;
                default:
                    break;
            }
        }


        private string _to;

        public string To
        {
            get { return _to; }
            set { _to = value; OnChanged("To"); }
        }

        private long _tradeAmount;

        public long TradeAmount
        {
            get { return _tradeAmount; }
            set { _tradeAmount = value; OnChanged("TradeAmount"); }
        }

        private long _tradeFee;

        public long TradeFee
        {
            get { return _tradeFee; }
            set { _tradeFee = value; OnChanged("TradeFee"); }
        }

        private long _arrivalAmount;

        public long ArrivalAmount
        {
            get { return _arrivalAmount; }
            set { _arrivalAmount = value; OnChanged("ArrivalAmount"); }
        }
    }

}
