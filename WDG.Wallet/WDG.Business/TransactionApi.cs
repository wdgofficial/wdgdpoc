// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

using WDG.DTO;
using WDG.Models;
using WDG.ServiceAgent;
using WDG.Utility;
using WDG.Utility.Api;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WDG.Business
{
    public static class TransactionApi
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromAccount"></param>
        /// <param name="many"></param>
        /// <param name="subtractFeeFromAmount"></param>
        /// <returns></returns>
        public static async Task<ApiResponse> SendMany(string fromAccount, SendManyModel[] many, string[] subtractFeeFromAmount)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Transaction trans = new Transaction();
                List<SendManyOM> omList = new List<SendManyOM>();
                for (int i = 0; i < many.Length; i++)
                {
                    omList.Add(new SendManyOM { Address = many[i].Address, Amount = many[i].Amount, Tag = many[i].Tag });
                }
                SendManyOM[] om = omList.ToArray();

                string result = await trans.SendMany(fromAccount, om, subtractFeeFromAmount);
                if (result != null)
                {
                    response.Result = Newtonsoft.Json.Linq.JToken.FromObject(result);
                }
                else
                {
                    response.Result = null;
                }
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toAddress"></param>
        /// <param name="amount"></param>
        /// <param name="comment"></param>
        /// <param name="commentTo"></param>
        /// <param name="subtractFeeFromAmount"></param>
        /// <returns></returns>
        public static async Task<ApiResponse> SendToAddress(string toAddress, long amount, string comment = "", string commentTo = "", bool subtractFeeFromAmount = false)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Transaction trans = new Transaction();

                string result = await trans.SendToAddress(toAddress, amount, comment, commentTo, subtractFeeFromAmount);
                if (result != null)
                {
                    response.Result = Newtonsoft.Json.Linq.JToken.FromObject(result);
                }
                else
                {
                    response.Result = null;
                }
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transactionFeePerKilobyte"></param>
        /// <returns></returns>
        public static async Task<ApiResponse> SetTxFee(long transactionFeePerKilobyte)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Transaction trans = new Transaction();
                await trans.SetTxFee(transactionFeePerKilobyte);
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="confirmations"></param>
        /// <returns></returns>
        public static async Task<ApiResponse> SetConfirmations(long confirmations)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Transaction trans = new Transaction();
                await trans.SetConfirmations(confirmations);
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static async Task<ApiResponse> GetTxSettings()
        {
            ApiResponse response = new ApiResponse();
            try
            {
                TransactionFeeSetting setting = new TransactionFeeSetting();
                Transaction trans = new Transaction();

                TransactionFeeSettingOM result = await trans.GetTxSettings();
                if (result != null)
                {
                    setting.Confirmations = result.Confirmations;
                    setting.FeePerKB = result.FeePerKB;
                    setting.Encrypt = result.Encrypt;
                    response.Result = Newtonsoft.Json.Linq.JToken.FromObject(setting);
                }
                else
                {
                    response.Result = null;
                }
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }

        public static async Task<ApiResponse> EstimateTxFeeForSendToAddress(string toAddress, long amount, string comment = "", string commentTo = "", bool subtractFeeFromAmount = false)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                TxFeeForSend fee = new TxFeeForSend();
                Transaction trans = new Transaction();

                TxFeeForSendOM result = await trans.EstimateTxFeeForSendToAddress(toAddress, amount, comment, commentTo, subtractFeeFromAmount);
                if (result != null)
                {
                    fee.TotalSize = result.TotalSize;
                    fee.TotalFee = result.TotalFee;
                    response.Result = Newtonsoft.Json.Linq.JToken.FromObject(fee);
                }
                else
                {
                    response.Result = null;
                }
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromAccount"></param>
        /// <param name="many"></param>
        /// <param name="subtractFeeFromAmount"></param>
        /// <returns></returns>
        public static async Task<ApiResponse> EstimateTxFeeForSendMany(string fromAccount, SendManyModel[] many, string[] subtractFeeFromAmount)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Transaction trans = new Transaction();
                TxFeeForSend fee = new TxFeeForSend();
                List<SendManyOM> omList = new List<SendManyOM>();
                for (int i = 0; i < many.Length; i++)
                {
                    omList.Add(new SendManyOM { Address = many[i].Address, Amount = many[i].Amount, Tag = many[i].Tag });
                }
                SendManyOM[] om = omList.ToArray();

                TxFeeForSendOM result = await trans.EstimateTxFeeForSendMany(fromAccount, om, subtractFeeFromAmount);
                if (result != null)
                {
                    fee.TotalFee = result.TotalFee;
                    fee.TotalSize = result.TotalSize;
                    response.Result = Newtonsoft.Json.Linq.JToken.FromObject(fee);
                }
                else
                {
                    response.Result = null;
                }
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }

        /// <summary>
        /// get transaction records
        /// </summary>
        /// <param name="account"></param>
        /// <param name="count"></param>
        /// <param name="skip"></param>
        /// <param name="includeWatchOnly"></param>
        /// <returns></returns>
        public static async Task<ApiResponse> ListTransactions(string account = "*", long count = 10, int skip = 0, bool includeWatchOnly = true)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Transaction trans = new Transaction();
                List<Payment> list = new List<Payment>();
                PaymentOM[] result = await trans.ListTransactions(account, count, skip, includeWatchOnly);
                if (result != null)
                {
                    for (int i = 0; i < result.Length; i++)
                    {
                        list.Add(new Payment()
                        {
                            Account = result[i].Account,
                            Address = result[i].Address,
                            Amount = result[i].Amount,
                            BlockHash = result[i].BlockHash,
                            BlockIndex = result[i].BlockIndex,
                            BlockTime = result[i].BlockTime,
                            Category = result[i].Category,
                            Comment = result[i].Comment,
                            Confirmations = result[i].Confirmations,
                            Fee = result[i].Fee,
                            Size = result[i].Size,
                            Time = result[i].Time,
                            TotalInput = result[i].TotalInput,
                            TotalOutput = result[i].TotalOutput,
                            TxId = result[i].TxId,
                            Vout = result[i].Vout
                        });
                    }
                    response.Result = Newtonsoft.Json.Linq.JToken.FromObject(list);
                }
                else
                {
                    response.Result = null;
                }
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }


        public static async Task<ApiResponse> ListFilterTrans(FilterIM filter, int count, int skip = 0, bool includeWatchOnly = true)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Transaction trans = new Transaction();
                List<Payment> list = new List<Payment>();
                PaymentOM[] result = await trans.ListFilterTrans(filter, count, skip, includeWatchOnly);
                if (result != null)
                {
                    for (int i = 0; i < result.Length; i++)
                    {
                        list.Add(new Payment()
                        {
                            Account = result[i].Account,
                            Address = result[i].Address,
                            Amount = result[i].Amount,
                            BlockHash = result[i].BlockHash,
                            BlockIndex = result[i].BlockIndex,
                            BlockTime = result[i].BlockTime,
                            Category = result[i].Category,
                            Comment = result[i].Comment,
                            Confirmations = result[i].Confirmations,
                            Fee = result[i].Fee,
                            Size = result[i].Size,
                            Time = result[i].Time,
                            TotalInput = result[i].TotalInput,
                            TotalOutput = result[i].TotalOutput,
                            TxId = result[i].TxId,
                            Vout = result[i].Vout
                        });
                    }
                    response.Result = Newtonsoft.Json.Linq.JToken.FromObject(list);
                }
                else
                {
                    response.Result = null;
                }
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }

        public static async Task<ApiResponse> SendRawTransaction(SendRawTransactionInputsIM[] senders, SendRawTransactionOutputsIM[] receivers, string changeAddress, long lockTime, long feeRate)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Transaction trans = new Transaction();
                string result = await trans.SendRawTransaction(senders, receivers, changeAddress, lockTime, feeRate);
                if (result != null)
                {
                    response.Result = Newtonsoft.Json.Linq.JToken.FromObject(result);
                }
                else
                {
                    response.Result = null;
                }
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }

        public static async Task<ApiResponse> ListSinceBlock(string blockHash, long confirmations)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Transaction trans = new Transaction();
                List<Payment> list = new List<Payment>();
                ListSinceBlockOM result = await trans.ListSinceBlock(blockHash, confirmations);
                if (result != null)
                {
                    response.Result = Newtonsoft.Json.Linq.JToken.FromObject(result);
                }
                else
                {
                    response.Result = null;
                }
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }

        public static async Task<ApiResponse> EstimateRawTransaction(SendRawTransactionInputsIM[] senders, SendRawTransactionOutputsIM[] receivers, string changeAddress, long feeRate)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Transaction trans = new Transaction();
                TxFeeForSend txFee = new TxFeeForSend();
                EstimateRawTransactionOM result = await trans.EstimateRawTransaction(senders, receivers, changeAddress, feeRate);
                if (result != null)
                {
                    txFee.TotalFee = result.totalFee;
                    txFee.TotalSize = result.totalSize;
                    txFee.Change = result.Change;
                    response.Result = Newtonsoft.Json.Linq.JToken.FromObject(txFee);
                }
                else
                {
                    response.Result = null;
                }
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                var errorMsg = string.IsNullOrEmpty(ex.ErrorMsg) ? ex.ToString() : ex.ErrorMsg;
                response.Error = new ApiError(ex.ErrorCode, errorMsg);
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }

        public static async Task<ApiResponse> SendNotify(string txId)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Transaction trans = new Transaction();
                List<Payment> list = new List<Payment>();
                await trans.SendNotify(txId);

                response.Result = "";
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }

        public static async Task<ApiResponse> GetTransaction(string txId)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Transaction trans = new Transaction();
                List<Payment> list = new List<Payment>();
                TransactionOM result = await trans.GetTransaction(txId);
                if (result != null)
                {
                    response.Result = Newtonsoft.Json.Linq.JToken.FromObject(result);
                }
                else
                {
                    response.Result = null;
                }
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }
    }
}
