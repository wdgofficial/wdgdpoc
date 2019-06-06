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
    public class TransactionTest
    {
        [TestMethod]
        public async Task SendToAddress()
        {
            Transaction tran = new Transaction();
            string address = "1No2SahjFuguswiSvHv1DqotTRdMNs4FH";
            long amount = 100000000000;
            string commentTo = "Join";
            string result = await tran.SendToAddress(address, amount, "", commentTo, false);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task SendMany()
        {
            Transaction tran = new Transaction();
            string fromAccount = "";
            SendManyOM[] om = new SendManyOM[] { new SendManyOM { Address = "fiiit6LKJEWHqfCXjez6C1tiLhYwWCZkLAZhzL", Tag = "John", Amount = 100000000 }, new SendManyOM { Address = "fiiit2UREUAMhYGZ3u7K7H1kaVvpRS5Wnb36Jd", Tag = "myself", Amount = 100000000 } };
            string[] subtractFeeFromAmount = new string[] { "fiiit2UREUAMhYGZ3u7K7H1kaVvpRS5Wnb36Jd" };
            string result = await tran.SendMany(fromAccount, om, subtractFeeFromAmount);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task SendRawTransaction()
        {
            Transaction tran = new Transaction();
            SendRawTransactionInputsIM[] senders = new SendRawTransactionInputsIM[] { new SendRawTransactionInputsIM { TxId = "BE569E0820BD049C4B52B381F55A6400D55EF7D39FFC354724B104B3033D1100", Vout = 0 }, new SendRawTransactionInputsIM { TxId = "3700C4CC071F486702986EF123CA3BF4048C9F0506991C208F917048126D4C32", Vout = 0 } };
            SendRawTransactionOutputsIM[] receivers = new SendRawTransactionOutputsIM[] { new SendRawTransactionOutputsIM { Address = "fiiit6LKJEWHqfCXjez6C1tiLhYwWCZkLAZhzL", Amount = 100000000 }, new SendRawTransactionOutputsIM { Address = "fiiit2UREUAMhYGZ3u7K7H1kaVvpRS5Wnb36Jd", Amount = 100000000 } };
            string changeAddress = "fiiit2UREUAMhYGZ3u7K7H1kaVvpRS5Wnb36Jd";
            long lockTime = 0;
            long feeRate = 204800;
            string result = await tran.SendRawTransaction(senders, receivers, changeAddress, lockTime, feeRate);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task SetTxFee()
        {
            Transaction tran = new Transaction();
            await tran.SetTxFee(100000);
        }

        [TestMethod]
        public async Task SetConfirmations()
        {
            Transaction tran = new Transaction();
            await tran.SetConfirmations(2);
        }

        [TestMethod]
        public async Task GetTxSettings()
        {
            Transaction tran = new Transaction();
            TransactionFeeSettingOM om = await tran.GetTxSettings();
            Assert.IsNotNull(om);
        }

        [TestMethod]
        public async Task EstimateTxFeeForSendToAddress()
        {
            Transaction tran = new Transaction();
            string address = "1No2SahjFuguswiSvHv1DqotTRdMNs4FH";
            long amount = 100000000000;
            string commentTo = "Join";
            TxFeeForSendOM om = await tran.EstimateTxFeeForSendToAddress(address, amount, "", commentTo, false);
            Assert.IsNotNull(om);
        }

        [TestMethod]
        public async Task EstimateTxFeeForSendMany()
        {
            Transaction tran = new Transaction();
            string fromAccount = "1B7CXXyT2KQSHprzbprQDXnmkFEUTJU7yS";
            SendManyOM[] om = new SendManyOM[] { new SendManyOM { Address = "1317nkscoSnkZnGdFMqVawjJv3xxU5vyfb", Tag = "John", Amount = 100000000000 }, new SendManyOM { Address = "1No2SahjFuguswiSvHv1DqotTRdMNs4FH", Tag = null, Amount = 100000000000 } };
            string[] subtractFeeFromAmount = new string[] { "1317nkscoSnkZnGdFMqVawjJv3xxU5vyfb", "1No2SahjFuguswiSvHv1DqotTRdMNs4FH" };
            TxFeeForSendOM result = await tran.EstimateTxFeeForSendMany(fromAccount, om, subtractFeeFromAmount);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task ListTransactions()
        {
            Transaction tran = new Transaction();
            PaymentOM[] result = await tran.ListTransactions("*", 10, 0, true);
            Assert.IsNotNull(result);
        }
    }
}
