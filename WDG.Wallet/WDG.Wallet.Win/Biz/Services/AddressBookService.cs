// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Business;
using WDG.Models;
using WDG.Utility;
using WDG.Utility.Api;
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Models;
using System;
using System.Collections.Generic;

namespace WDG.Wallet.Win.Biz.Services
{
    public class AddressBookService : ServiceBase<AddressBookService>
    {
        public Result<bool> AddNewAddressBookItem(string account, string tag)
        {
            var check = VerfyAddress(FiiiCoinSetting.CurrentNetworkType, account);
            if (!check)
            {
                return new Result<bool>() { IsFail = true, ErrorCode = 70000001 };
            }
            ApiResponse response = AddressBookApi.AddNewAddressBookItem(account, tag).Result;
            return GetResult<bool>(response);
        }

        public Result<bool> UpsertAddrBookItem(string account, string tag, long id = -1)
        {
            var check = VerfyAddress(FiiiCoinSetting.CurrentNetworkType, account);
            if (!check)
            {
                return new Result<bool>() { IsFail = true, ErrorCode = 70000001 };
            }
            ApiResponse response = AddressBookApi.UpsertAddrBookItem(id, account, tag).Result;
            return GetResult<bool>(response);
        }

        public Result<List<AddressBookInfo>> GetAddressBook()
        {
            ApiResponse response = AddressBookApi.GetAddressBook().Result;
            return GetResult<List<AddressBookInfo>>(response);
        }


        public Result<AddressBookInfo> GetAddressBookItemByAddress(string address)
        {
            ApiResponse response = AddressBookApi.GetAddressBookItemByAddress(address).Result;
            return GetResult<AddressBookInfo>(response);
        }


        public Result<List<AddressBookInfo>> GetAddressBookByTag(string tag)
        {
            ApiResponse response = AddressBookApi.GetAddressBookByTag(tag).Result;
            return GetResult<List<AddressBookInfo>>(response);
        }

        public Result<bool> DeleteAddressBookByIds(params long[] ids)
        {
            ApiResponse response = AddressBookApi.DeleteAddressBookByIds(ids).Result;
            return GetResult<bool>(response);
        }


        public bool VerfyAddress(NetworkType network, string account)
        {
            bool result = false;
            try
            {
                result = AddressTools.AddressVerfy(network.ToString().ToLower(), account);
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
            }
            return result;
        }
    }
}