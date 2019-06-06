// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Business;
using WDG.Models;
using WDG.Utility.Api;
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Models;
using System;
using System.Collections.Generic;

namespace WDG.Wallet.Win.Biz.Services
{
    public class AccountsService : ServiceBase<AccountsService>
    {
        public Result<AccountInfo> GetDefaultAccount()
        {
            ApiResponse response = AccountsApi.GetDefaultAccount().Result;
            return GetResult<AccountInfo>(response);
        }

        public Result<List<AccountInfo>> GetAddressesByTag(string tag = "*")
        {
            ApiResponse response = AccountsApi.GetAddressesByTag(tag).Result;
            var result = GetResult<List<AccountInfo>>(response);
            return result;
        }

        public Result<AccountInfo> GetNewAddress(string tag)
        {
            ApiResponse response = AccountsApi.GetNewAddress(tag).Result;
            var result = GetResult<AccountInfo>(response);
            return result;
        }

        public Result<bool> SetDefaultAccount(string account)
        {
            ApiResponse response = AccountsApi.SetDefaultAccount(account).Result;
            var result = GetResult<bool>(response);
            return result;
        }

        public Result<AddressInfo> ValidateAddress(string account)
        {
            ApiResponse response = AccountsApi.ValidateAddress(account).Result;
            var result = GetResult<AddressInfo>(response);
            return result;
        }

        public Result SetAccountTag(string address, string tag)
        {
            ApiResponse response = AccountsApi.ValidateAddress(address).Result;
            if (response.HasError)
                return new Result() { IsFail = true, ApiResponse = response };

            AddressInfo addressInfo = response.GetResult<AddressInfo>();
            if (addressInfo.IsValid)
            {
                var res = AccountsApi.SetAccountTag(address, tag).Result;
                return GetResult(res);
            }

            return new Result() { IsFail = true, ApiResponse = response };
        }

        public enum AccountCategory
        {
            ALL = 0,
            ChangeAccount = 1,
            CreateAccount = 2,
            WatchOnly = 3
        }

        public Result<List<AccountInfo>> GetPageAccountCategory(AccountCategory accountCategory = AccountCategory.ALL)
        {
            ApiResponse response = AccountsApi.GetPageAccountCategory((int)accountCategory).Result;
            var result = GetResult<List<AccountInfo>>(response);
            return result;
        }
    }
}
