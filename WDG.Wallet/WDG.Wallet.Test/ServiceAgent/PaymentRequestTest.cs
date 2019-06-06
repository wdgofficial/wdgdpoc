// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

using WDG.DTO;
using WDG.ServiceAgent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace WDG.Wallet.Test.ServiceAgent
{
    [TestClass]
    public class PaymentRequestTest
    {
        [TestMethod]
        public async Task CreateNewPaymentRequest()
        {
            PaymentRequest payment = new PaymentRequest();
            PayRequestOM result = await payment.CreateNewPaymentRequest("fiiitFmH9Cqk5B9gTH3LqZzBtqb8pxgHJ7sVqY", "abc", 10000, "test");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetAllPaymentRequests()
        {
            PaymentRequest payment = new PaymentRequest();
            PayRequestOM[] result = await payment.GetAllPaymentRequests();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task DeletePaymentRequestsByIds()
        {
            PaymentRequest payment = new PaymentRequest();
            long[] arr = new long[] { 5, 6 };
            await payment.DeletePaymentRequestsByIds(arr);
        }
    }
}
