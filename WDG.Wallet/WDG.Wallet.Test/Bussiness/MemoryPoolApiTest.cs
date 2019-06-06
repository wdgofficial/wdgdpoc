// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

using WDG.Business;
using WDG.Models;
using WDG.Utility.Api;
using FluentScheduler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace WDG.Wallet.Test.Bussiness
{
    [TestClass]
    public class MemoryPoolApiTest : Registry
    {
        [TestMethod]
        public Dictionary<string, List<Payment>> GetPaymentInfoInMemPool()
        {
            List<string> listOld = new List<string>();
            Dictionary<string, List<Payment>> dic = new Dictionary<string, List<Payment>>();
            Schedule(async () =>
            {
                if (listOld == null)
                {
                    ApiResponse response = await MemoryPoolApi.GetAllTxInMemPool();
                    if (!response.HasError)
                    {
                        listOld = response.GetResult<List<string>>();
                    }
                    dic = new Dictionary<string, List<Payment>>();
                }
                else
                {
                    List<string> listNew = new List<string>();
                    ApiResponse response = await MemoryPoolApi.GetAllTxInMemPool();
                    if (!response.HasError)
                    {
                        listNew = response.GetResult<List<string>>();
                        foreach (string item in listNew)
                        {
                            if (!listOld.Contains(item))
                            {
                                ApiResponse payResponse = await MemoryPoolApi.GetPaymentInfoInMemPool(item);
                                if (!payResponse.HasError)
                                {
                                    List<Payment> list = payResponse.GetResult<List<Payment>>();
                                    dic.Add(item, list);
                                }
                            }
                        }
                    }
                    listOld = listNew;
                }
            }).ToRunNow().AndEvery(10).Seconds();
            return dic;
        }
    }
}
