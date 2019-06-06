// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

using WDG.Business;
using WDG.Models;
using WDG.Utility;
using WDG.Utility.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WDG.Wallet.Test.Bussiness
{
    [TestClass]
    public class TransactionApiTest
    {
        [TestMethod]
        public async Task TestSendMany()
        {
            string fromAccount = "1B7CXXyT2KQSHprzbprQDXnmkFEUTJU7yS";
            SendManyModel[] om = new SendManyModel[] { new SendManyModel { Address = "1317nkscoSnkZnGdFMqVawjJv3xxU5vyfb", Tag = "John", Amount = 100000000000 }, new SendManyModel { Address = "1No2SahjFuguswiSvHv1DqotTRdMNs4FH", Tag = null, Amount = 100000000000 } };
            string[] subtractFeeFromAmount = new string[] { "1317nkscoSnkZnGdFMqVawjJv3xxU5vyfb", "1No2SahjFuguswiSvHv1DqotTRdMNs4FH" };
            ApiResponse response = await TransactionApi.SendMany(fromAccount, om, subtractFeeFromAmount);
            // Client Use
            Assert.IsFalse(response.HasError);
            string result = response.GetResult<string>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task TestSendToAddress()
        {
            ApiResponse blockChainResponse = await BlockChainEngineApi.GetBlockChainStatus();
            if (!blockChainResponse.HasError)
            {
                //地址
                string address = "fiiitCPyohiEPn9q11AXCdvVDouoVvgojXBcVj";
                //地址校验
                BlockChainStatus blockChainStatus = blockChainResponse.GetResult<BlockChainStatus>();
                //验证address
                if (AddressTools.AddressVerfy(blockChainStatus.ChainNetwork, address))
                {
                    //判断是否加密
                    ApiResponse transactionResponse = await TransactionApi.GetTxSettings();
                    if (!transactionResponse.HasError)
                    {
                        TransactionFeeSetting settingResult = transactionResponse.GetResult<TransactionFeeSetting>();
                        if (settingResult.Encrypt)
                        {
                            //先解锁
                            string password = "P@ssw0rd$";
                            ApiResponse unlockResponse = await WalletManagementApi.WalletPassphrase(password);
                            if (!unlockResponse.HasError)
                            {
                                ApiResponse response = await TransactionApi.SendToAddress(address, 50000000, "this is your request", "John", false);
                                Assert.IsFalse(response.HasError);
                                string result = response.GetResult<string>();
                                Assert.IsNotNull(result);
                            }
                            ApiResponse lockResponse = await WalletManagementApi.WalletLock();
                        }
                        else
                        {
                            ApiResponse response = await TransactionApi.SendToAddress(address, 50000000, "this is your request", "John", false);
                            Assert.IsFalse(response.HasError);
                            string result = response.GetResult<string>();
                            Assert.IsNotNull(result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public async Task TestSetTxFee()
        {
            ApiResponse response = await TransactionApi.SetTxFee(100000);
            Assert.IsFalse(response.HasError);
        }

        [TestMethod]
        public async Task TestSetConfirmations()
        {
            ApiResponse response = await TransactionApi.SetConfirmations(2);
            Assert.IsFalse(response.HasError);
        }

        [TestMethod]
        public async Task TestGetTxSettings()
        {
            ApiResponse response = await TransactionApi.GetTxSettings();
            Assert.IsFalse(response.HasError);
            TransactionFeeSetting result = response.GetResult<TransactionFeeSetting>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task TestEstimateTxFeeForSendToAddress()
        {
            string address = "fiiitCPyohiEPn9q11AXCdvVDouoVvgojXBcVj";
            long amount = 100000000000;
            string commentTo = "Join";
            ApiResponse response = await TransactionApi.EstimateTxFeeForSendToAddress(address, amount, "", commentTo, false);
            Assert.IsFalse(response.HasError);
            TxFeeForSend result = response.GetResult<TxFeeForSend>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task TestEstimateTxFeeForSendMany()
        {
            string fromAccount = "fiiitCPyohiEPn9q11AXCdvVDouoVvgojXBcVj";
            SendManyModel[] om = new SendManyModel[] { new SendManyModel { Address = "fiiitCPyohiEPn9q11AXCdvVDouoVvgojXBcVj", Tag = "John", Amount = 100000000000 }, new SendManyModel { Address = "fiiitCPyohiEPn9q11AXCdvVDouoVvgojXBcVj", Tag = null, Amount = 100000000000 } };
            string[] subtractFeeFromAmount = new string[] { "fiiitCPyohiEPn9q11AXCdvVDouoVvgojXBcVj", "fiiitCPyohiEPn9q11AXCdvVDouoVvgojXBcVj" };
            ApiResponse response = await TransactionApi.EstimateTxFeeForSendMany(fromAccount, om, subtractFeeFromAmount);
            Assert.IsFalse(response.HasError);
            TxFeeForSend result = response.GetResult<TxFeeForSend>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task TestListTransactions()
        {
            ApiResponse response = await TransactionApi.ListTransactions("*", 10, 0, true);
            Assert.IsFalse(response.HasError);
            List<Payment> result = response.GetResult<List<Payment>>();
            Assert.IsNotNull(result);
        }
    }
}
