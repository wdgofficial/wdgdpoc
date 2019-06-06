// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Models;
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Converters;
using GalaSoft.MvvmLight;
using System;
using System.Text;

namespace WDG.Wallet.Win.Models
{
    public class TradeRecordInfo : ViewModelBase
    {
        public TradeRecordInfo(Payment payment)
        {
            this.TradeTime = GetTradeTime(payment.Time);
            Payment = payment;
        }

        private Payment _payment;

        public Payment Payment
        {
            get { return _payment; }
            set { _payment = value; RaisePropertyChanged("Payment"); }
        }
        

        protected DateTime GetTradeTime(long time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            DateTime dt = startTime.AddMilliseconds(time);
            return dt;
        }
        private DateTime _tradeTime;

        public DateTime TradeTime
        {
            get { return _tradeTime; }
            set
            {
                _tradeTime = value;
                RaisePropertyChanged("TradeTime");
                RaisePropertyChanged("TradeTimeStr");
            }
        }
    }



    public static class TradeRecordInfoExtensions
    {
        public static string GetCsvContent(this System.Collections.Generic.IEnumerable<TradeRecordInfo> tradeRecordInfos)
        {
            var state = LanguageService.Default.GetLanguageValue("page_tradeRecord_state");
            var time = LanguageService.Default.GetLanguageValue("page_tradeRecord_time");
            var type = LanguageService.Default.GetLanguageValue("page_tradeRecord_type");
            var tag = LanguageService.Default.GetLanguageValue("page_tradeRecord_tag");
            var amount = LanguageService.Default.GetLanguageValue("page_tradeRecord_amount");

            string header = string.Format("{0},{1},{2},{3},{4}", state, time, type, tag, amount);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(header);

            var stateConverter = new ConfirmationToStatusDetail();
            var timeConverter = new TimestampToDateTimeConverter();
            var typeConverter = new CategoryToStringConverter();
            var markConverter = new PaymentToMarkConverter();
            var amountConverter = new PaymentToTradeAmountConverter();

            var cultureInfo = new System.Globalization.CultureInfo(1033);
            foreach (var item in tradeRecordInfos)
            {
                var txt_state = stateConverter.Convert(item.Payment, typeof(object), null, cultureInfo);
                var txt_time = timeConverter.Convert(item.Payment.Time, typeof(object), "yyyy-MM-dd HH:mm:ss", cultureInfo);
                var txt_type = typeConverter.Convert(item.Payment.Category, typeof(object), null, cultureInfo);
                var txt_mark = markConverter.Convert(item.Payment, typeof(object), null, cultureInfo);
                var txt_amount = amountConverter.Convert(item.Payment, typeof(object), null, cultureInfo);

                string line = string.Format("{0},{1},{2},{3},{4}", txt_state, txt_time, txt_type, txt_mark, txt_amount);
                stringBuilder.AppendLine(line);
            }

            return stringBuilder.ToString();
        }
    }

}