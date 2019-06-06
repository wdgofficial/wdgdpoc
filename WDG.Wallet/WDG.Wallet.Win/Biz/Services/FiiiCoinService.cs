// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Business;
using WDG.DTO;
using WDG.Models;
using WDG.Utility.Api;
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace WDG.Wallet.Win.Biz.Services
{
    public class FiiiCoinService : ServiceBase<FiiiCoinService>
    {
        public Result<string> SendMany(string fromAccount, IEnumerable<SendItemInfo> sendManyModels)
        {
            var subtractFeeFromAmounts = new List<string>();
            sendManyModels.ToList().ForEach(x =>
            {
                if (x.IsContainFee)
                    subtractFeeFromAmounts.Add(x.Address);
            });
            ApiResponse response = TransactionApi.SendMany(fromAccount, sendManyModels.ToArray(), subtractFeeFromAmounts.ToArray()).Result;

            return base.GetResult<string>(response); ;
        }

        public Result<string> SendRawTransactions(List<SendRawTransactionInputsIM> senders, IEnumerable<SendItemInfo> sendManyModels, string changeAddress, long lockTime, long feeRate)
        {
            if (senders == null || !senders.Any())
                return null;

            List<SendRawTransactionOutputsIM> receivers = new List<SendRawTransactionOutputsIM>();
            sendManyModels.ToList().ForEach(x =>
            {
                SendRawTransactionOutputsIM receiver = new SendRawTransactionOutputsIM
                {
                    Address = x.Address,
                    Amount = x.Amount
                };
                receivers.Add(receiver);
            });

            ApiResponse response = TransactionApi.SendRawTransaction(senders.ToArray(), receivers.ToArray(), changeAddress, lockTime, feeRate).Result;

            return base.GetResult<string>(response); ;
        }

        public Result<ObservableCollection<TradeRecordInfo>> GetListTransactions(string account = "*", int skip = 0, bool includeWatchOnly = true, long count = 5)
        {
            var result = new Result<ObservableCollection<TradeRecordInfo>>();

            var p = Math.Pow(10, 8);
            ApiResponse response = TransactionApi.ListTransactions(account, count, skip, includeWatchOnly).Result;
            result.IsFail = response.HasError;
            if (result.IsFail)
                return result;

            var payments = response.GetResult<List<Payment>>();

            result.Value = new ObservableCollection<TradeRecordInfo>();
            payments.ForEach(x =>
            {
                var item = new TradeRecordInfo(x);
                result.Value.Add(item);
            });

            return result;
        }

        public Result<ObservableCollection<TradeRecordInfo>> ListFilterTrans(FilterIM filer, int skip = 0, bool includeWatchOnly = true, int count = 5)
        {
            var result = new Result<ObservableCollection<TradeRecordInfo>>();

            var p = Math.Pow(10, 8);
            ApiResponse response = TransactionApi.ListFilterTrans(filer, count, skip, includeWatchOnly).Result;
            result.IsFail = response.HasError;
            if (result.IsFail)
                return result;

            var payments = response.GetResult<List<Payment>>();

            result.Value = new ObservableCollection<TradeRecordInfo>();
            payments.ForEach(x =>
            {
                var item = new TradeRecordInfo(x);
                result.Value.Add(item);
            });

            return result;
        }

        public Result SetTxFee(double fee)
        {
            var feeLong = Convert.ToInt64(fee * Math.Pow(10, 8));
            ApiResponse response = TransactionApi.SetTxFee(feeLong).Result;
            return base.GetResult(response);
        }

        public Result SetConfirmations(double fee)
        {
            var feeLong = Convert.ToInt64(fee * Math.Pow(10, 8));
            ApiResponse response = TransactionApi.SetConfirmations(feeLong).Result;
            return base.GetResult(response);
        }

        public Result<TransactionFeeSetting> GetTxSettings()
        {
            ApiResponse response = GetResponseResult(TransactionApi.GetTxSettings());
            return base.GetResult<TransactionFeeSetting>(response);
        }

        public Result<PayRequest> CreateNewPaymentRequest(ReceiveInfo receiveInfo)
        {
            ApiResponse response = PaymentRequestApi.CreateNewPaymentRequest(receiveInfo.Tag, receiveInfo.Amount, receiveInfo.Comment).Result;
            return GetResult<PayRequest>(response);
        }

        public Result<TxFeeForSend> EstimateTxFeeForSendToAddress(string address, long amount, string commentTo)
        {
            ApiResponse response = TransactionApi.EstimateTxFeeForSendToAddress(address, amount, "", commentTo, false).Result;
            return GetResult<TxFeeForSend>(response);
        }

        public Result<TxFeeForSend> EstimateTxFeeForSendMany(string fromAccount, IEnumerable<SendItemInfo> sendManyModels)
        {
            var subtractFeeFromAmounts = sendManyModels.Where(x => x.IsContainFee).Select(x => x.Address).ToArray();
            ApiResponse response = TransactionApi.EstimateTxFeeForSendMany(fromAccount, sendManyModels.ToArray(), subtractFeeFromAmounts).Result;
            return base.GetResult<TxFeeForSend>(response);
        }

        public Result<TxFeeForSend> EstimateRawTransaction(List<SendRawTransactionInputsIM> senders, IEnumerable<SendItemInfo> sendManyModels, string changeAddress, long feeRate)
        {
            List<SendRawTransactionOutputsIM> receivers = new List<SendRawTransactionOutputsIM>();
            sendManyModels.ToList().ForEach(x =>
            {
                SendRawTransactionOutputsIM receiver = new SendRawTransactionOutputsIM
                {
                    Address = x.Address,
                    Amount = x.Amount
                };
                receivers.Add(receiver);
            });

            ApiResponse response = TransactionApi.EstimateRawTransaction(senders.ToArray(), receivers.ToArray(), changeAddress, feeRate).Result;
            return base.GetResult<TxFeeForSend>(response);
        }
    }
}