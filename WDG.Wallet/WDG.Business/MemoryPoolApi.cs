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
using System.Linq;
using System.Threading.Tasks;

namespace WDG.Business
{
    public static class MemoryPoolApi
    {
        public static async Task<ApiResponse> GetAllTxInMemPool()
        {
            ApiResponse response = new ApiResponse();
            try
            {
                MemoryPool pool = new MemoryPool();
                string[] result = await pool.GetAllTxInMemPool();
                if (result != null)
                {
                    response.Result = Newtonsoft.Json.Linq.JToken.FromObject(result.ToList());
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

        public static async Task<ApiResponse> GetPaymentInfoInMemPool(string txid)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                MemoryPool pool = new MemoryPool();
                List<Payment> list = new List<Payment>();
                PaymentOM[] result = await pool.GetPaymentInfoInMemPool(txid);
                if (result != null && result.Length > 0)
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
                    response.Result = Newtonsoft.Json.Linq.JToken.FromObject(result.ToList());
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
