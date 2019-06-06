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
    public class AddressBook
    {
        public async Task AddNewAddressBookItem(string address, string label)
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithParameterList("AddNewAddressBookItem", new List<object> { address, label }, 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
        }

        public async Task<AddressBookInfoOM[]> GetAddressBook()
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithNoParameters("GetAddressBook", 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
            AddressBookInfoOM[] responseValue = response.GetResult<AddressBookInfoOM[]>();
            return responseValue;
        }

        public async Task<AddressBookInfoOM> GetAddressBookItemByAddress(string address)
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithParameterList("GetAddressBookItemByAddress", new[] { address }, 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
            AddressBookInfoOM responseValue = response.GetResult<AddressBookInfoOM>();
            return responseValue;
        }

        public async Task<AddressBookInfoOM[]> GetAddressBookByTag(string tag)
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithParameterList("GetAddressBookByTag", new[] { tag }, 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
            AddressBookInfoOM[] responseValue = response.GetResult<AddressBookInfoOM[]>();
            return responseValue;
        }

        public async Task DeleteAddressBookByIds(long[] ids)
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithParameterList("DeleteAddressBookByIds", new[] { ids}, 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
        }

        public async Task UpsertAddrBookItem(long id, string address, string tag)
        {
            AuthenticationHeaderValue authHeaderValue = null;
            RpcClient client = new RpcClient(new Uri(WalletNetwork.NetWork), authHeaderValue, null, null, "application/json");
            RpcRequest request = RpcRequest.WithParameterList("UpsertAddrBookItem", new List<object> { id, address, tag }, 1);
            RpcResponse response = await client.SendRequestAsync(request);
            if (response.HasError)
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
        }
    }
}
