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
    public class BlockChainEngine
    {
        public async Task<BlockChainStatusOM> GetBlockChainStatus()
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithNoParameters("GetBlockChainStatus", 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
            BlockChainStatusOM responseValue = response.GetResult<BlockChainStatusOM>();
            return responseValue;
        }

        public async Task StopEngine()
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithNoParameters("StopEngine", 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
        }

        public async Task<long> GetBlockCount()
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithNoParameters("GetBlockCount", 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
            long responseValue = response.GetResult<long>();
            return responseValue;
        }

        public async Task<string> GetBlockHash(long height)
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithParameterList("GetBlockHash", new List<object> { height }, 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
            string responseValue = response.GetResult<string>();
            return responseValue;
        }

        public async Task<BlockInfoOM> GetBlock(string hash, int formate = 0)
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithParameterList("GetBlock", new List<object> { hash, formate }, 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
            BlockInfoOM responseValue = response.GetResult<BlockInfoOM>();
            return responseValue;
        }

        public async Task<BlockHeaderOM> GetBlockHeader(string hash, int formate = 0)
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithParameterList("GetBlockHeader", new List<object> { hash, formate }, 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
            BlockHeaderOM responseValue = response.GetResult<BlockHeaderOM>();
            return responseValue;
        }

        public async Task<ChainTipsOM[]> GetChainTips()
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithNoParameters("GetChainTips", 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
            ChainTipsOM[] responseValue = response.GetResult<ChainTipsOM[]>();
            return responseValue;
        }

        public async Task<BlockDifficultyOM> GetDifficulty()
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithNoParameters("GetDifficulty", 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
            BlockDifficultyOM responseValue = response.GetResult<BlockDifficultyOM>();
            return responseValue;
        }

        public async Task<BlockInfoOM> GenerateNewBlock(string minerName, string address, int format = 0)
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithParameterList("GenerateNewBlock", new List<object> { minerName, address, format }, 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
            BlockInfoOM responseValue = response.GetResult<BlockInfoOM>();
            return responseValue;
        }

        public async Task SubmitBlock(string blockData)
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithParameterList("SubmitBlock", new [] { blockData }, 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
        }

        public async Task<long> EstimateSmartFee()
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithNoParameters("EstimateSmartFee", 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
            long responseValue = response.GetResult<long>();
            return responseValue;
        }

        public async Task<MessageOM> SignMessage(string address, string message)
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithParameterList("SignMessage", new List<object> { address, message }, 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
            MessageOM responseValue = response.GetResult<MessageOM>();
            return responseValue;
        }
    }
}
