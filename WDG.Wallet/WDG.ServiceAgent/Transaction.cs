// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

using EdjCase.JsonRpc.Client;
using EdjCase.JsonRpc.Core;
using WDG.DTO;
using WDG.Utility;
using WDG.Utility.Api;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WDG.ServiceAgent
{
    public class Transaction
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="toAddress"></param>
        /// <param name="amount"></param>
        /// <param name="comment"></param>
        /// <param name="commentTo"></param>
        /// <param name="subtractFeeFromAmount"></param>
        /// <returns></returns>
        public async Task<string> SendToAddress(string toAddress, long amount, string comment = "", string commentTo = "", bool subtractFeeFromAmount = false)
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithParameterList("SendToAddress", new List<object> { toAddress, amount, comment, commentTo, subtractFeeFromAmount }, 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
            string responseValue = response.GetResult<string>();
            return responseValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromAccount"></param>
        /// <param name="many"></param>
        /// <param name="subtractFeeFromAmount"></param>
        /// <returns></returns>
        public async Task<string> SendMany(string fromAccount, SendManyOM[] many, string[] subtractFeeFromAmount = null)
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithParameterList("SendMany", new List<object> { fromAccount, many, subtractFeeFromAmount }, 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
            string responseValue = response.GetResult<string>();
            return responseValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transactionFeePerKilobyte"></param>
        /// <returns></returns>
        public async Task SetTxFee(long transactionFeePerKilobyte)
        {
            AuthenticationHeaderValue authHeaderValue = null;
            //List<object> list = transactionFeePerKilobyte.ToList().ConvertAll(s => (object)s);
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithParameterList("SetTxFee", new List<object> { transactionFeePerKilobyte }, 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="confirmations"></param>
        /// <returns></returns>
        public async Task SetConfirmations(long confirmations)
        {
            AuthenticationHeaderValue authHeaderValue = null;
            //List<object> list = confirmations.ToList().ConvertAll(s => (object)s);
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithParameterList("SetConfirmations", new List<object> { confirmations }, 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<TransactionFeeSettingOM> GetTxSettings()
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithNoParameters("GetTxSettings", 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
            TransactionFeeSettingOM responseValue = response.GetResult<TransactionFeeSettingOM>();
            return responseValue;
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
        public async Task<TxFeeForSendOM> EstimateTxFeeForSendToAddress(string toAddress, long amount, string comment = "", string commentTo = "", bool subtractFeeFromAmount = false)
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithParameterList("EstimateTxFeeForSendToAddress", new List<object> { toAddress, amount, comment, commentTo, subtractFeeFromAmount }, 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
            TxFeeForSendOM responseValue = response.GetResult<TxFeeForSendOM>();
            return responseValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromAccount"></param>
        /// <param name="many"></param>
        /// <param name="subtractFeeFromAmount"></param>
        /// <returns></returns>
        public async Task<TxFeeForSendOM> EstimateTxFeeForSendMany(string fromAccount, SendManyOM[] many, string[] subtractFeeFromAmount = null)
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithParameterList("EstimateTxFeeForSendMany", new List<object> { fromAccount, many, subtractFeeFromAmount }, 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
            TxFeeForSendOM responseValue = response.GetResult<TxFeeForSendOM>();
            return responseValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="account">The name of an account to get transactinos from. Use an empty string ("") to get transactions for the default account. Default is * to get transactions for all accounts.</param>
        /// <param name="count">The number of the most recent transactions to list. Default is 10</param>
        /// <param name="skip">The number of the most recent transactions which should not be returned. Allows for pagination of results. Default is 0</param>
        /// <param name="includeWatchOnly">If set to true, include watch-only addresses in details and calculations as if they were regular addresses belonging to the wallet. If set to false(the default), treat watch-only addresses as if they didn’t belong to this wallet</param>
        /// <returns></returns>
        public async Task<PaymentOM[]> ListTransactions(string account = "*", long count = 10, int skip = 0, bool includeWatchOnly = true)
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithParameterList("ListTransactions", new List<object> { account, count, skip, includeWatchOnly }, 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
            PaymentOM[] responseValue = response.GetResult<PaymentOM[]>();
            return responseValue;
        }

        public async Task<PaymentOM[]> ListFilterTrans(FilterIM filter, int count, int skip = 0, bool includeWatchOnly = true)
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithParameterList("ListFilterTrans", new List<object> { filter, count, skip, includeWatchOnly }, 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
            PaymentOM[] responseValue = response.GetResult<PaymentOM[]>();
            return responseValue;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="senders"></param>
        /// <param name="receivers"></param>
        /// <param name="changeAddress"></param>
        /// <param name="lockTime"></param>
        /// <param name="feeRate"></param>
        /// <returns></returns>
        public async Task<string> SendRawTransaction(SendRawTransactionInputsIM[] senders, SendRawTransactionOutputsIM[] receivers, string changeAddress, long lockTime, long feeRate)
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithParameterList("SendRawTransaction", new List<object> { senders, receivers, changeAddress, lockTime, feeRate }, 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
            string responseValue = response.GetResult<string>();
            return responseValue;
        }

        public async Task<ListSinceBlockOM> ListSinceBlock(string blockHash, long confirmations)
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithParameterList("ListSinceBlock", new List<object> { blockHash, confirmations }, 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
            ListSinceBlockOM responseValue = response.GetResult<ListSinceBlockOM>();
            return responseValue;
        }

        public async Task<EstimateRawTransactionOM> EstimateRawTransaction(SendRawTransactionInputsIM[] senders, SendRawTransactionOutputsIM[] receivers, string changeAddress, long feeRate)
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithParameterList("EstimateRawTransaction", new List<object> { senders, receivers, changeAddress, feeRate }, 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
            EstimateRawTransactionOM responseValue = response.GetResult<EstimateRawTransactionOM>();
            return responseValue;
        }

        public async Task SendNotify(string txId)
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithParameterList("SendNotify", new List<object> { txId }, 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
        }

        public async Task<TransactionOM> GetTransaction(string txId)
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithParameterList("GetTransaction", new List<object> { txId }, 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
            TransactionOM responseValue = response.GetResult<TransactionOM>();
            return responseValue;
        }
    }
}
