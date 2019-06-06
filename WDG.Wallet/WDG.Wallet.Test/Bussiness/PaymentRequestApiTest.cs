// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

using WDG.Business;
using WDG.Models;
using WDG.Utility.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WDG.Wallet.Test.Bussiness
{
    [TestClass]
    public class PaymentRequestApiTest
    {
        [TestMethod]
        public async Task CreateNewPaymentRequest()
        {
            ApiResponse response = await PaymentRequestApi.CreateNewPaymentRequest("abc", 10000, "test");
            Assert.IsFalse(response.HasError);
            PayRequest result = response.GetResult<PayRequest>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetAllPaymentRequests()
        {
            ApiResponse response = await PaymentRequestApi.GetAllPaymentRequests();
            Assert.IsFalse(response.HasError);
            List<PayRequest> result = response.GetResult<List<PayRequest>>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task DeletePaymentRequestsByIds()
        {
            ApiResponse response = await PaymentRequestApi.DeletePaymentRequestsByIds(new long[] { 5,6 });
            Assert.IsFalse(response.HasError);
            bool result = response.GetResult<bool>();
            Assert.IsNotNull(result);
        }
    }
}
