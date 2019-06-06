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
    public class UTXO
    {
        public async Task<TxOutSetOM> GetTxOutSetInfo()
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithNoParameters("GetTxOutSetInfo", 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
            TxOutSetOM responseValue = response.GetResult<TxOutSetOM>();
            return responseValue;
        }

        public async Task<List<UnspentOM>> ListUnspent(int minConfirmations, int maxConfirmations = 9999, string[] addresses = null)
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithParameterList("ListUnspent", new List<object> { minConfirmations, maxConfirmations, addresses }, 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
            List<UnspentOM> responseValue = response.GetResult<List<UnspentOM>>();
            return responseValue;
        }

        public async Task<long> GetUnconfirmedBalance()
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithNoParameters("GetUnconfirmedBalance", 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
            long responseValue = response.GetResult<long>();
            return responseValue;
        }

        public async Task<TxOutOM> GetTxOut(string txid, int vout, bool unconfirmed)
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithParameterList("GetTxOut", new List<object> { txid, vout, unconfirmed }, 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
            TxOutOM responseValue = response.GetResult<TxOutOM>();
            return responseValue;
        }

        public async Task<bool> LockUnspent(bool isLocked, ListLockUnspentIM[] transaction)
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithParameterList("LockUnspent", new List<object> { isLocked, transaction }, 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
            bool responseValue = response.GetResult<bool>();
            return responseValue;
        }

        public async Task<List<ListLockUnspentIM>> ListLockUnspent()
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithNoParameters("ListLockUnspent", 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
            List<ListLockUnspentIM> responseValue = response.GetResult<List<ListLockUnspentIM>>();
            return responseValue;
        }

        public async Task<ListPageUnspentOM> ListPageUnspent(long minConfirmations, int currentPage, int pageSize, long maxConfirmations = 9999999, long minAmount = 1, long maxAmount = long.MaxValue, bool isDesc = false)
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithParameterList("ListPageUnspent", new List<object> { minConfirmations, currentPage, pageSize, maxConfirmations, minAmount, maxAmount, isDesc }, 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
            ListPageUnspentOM responseValue = response.GetResult<ListPageUnspentOM>();
            return responseValue;
        }
    }
}
