// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

using WDG.DTO;
using WDG.Models;
using WDG.ServiceAgent;
using WDG.Utility;
using WDG.Utility.Api;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WDG.Business
{
    public static class AccountsApi
    {
        public static async Task<ApiResponse> GetAddressesByTag(string tag = "")
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Accounts account = new Accounts();
                List<AccountInfo> list = new List<AccountInfo>();
                AccountInfoOM[] result = await account.GetAddressesByTag(tag);
                if (result != null)
                {
                    for (int i = 0; i < result.Length; i++)
                    {
                        list.Add(new AccountInfo()
                        {
                            Address = result[i].Address,
                            Balance = result[i].Balance,
                            IsDefault = result[i].IsDefault,
                            PublicKey = result[i].PublicKey,
                            Tag = result[i].Tag,
                            WatchOnly = result[i].WatchOnly
                        });
                    }
                    response.Result = Newtonsoft.Json.Linq.JToken.FromObject(list);
                }
                else
                {
                    response.Result = null;
                }
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }

        public static async Task<ApiResponse> GetAccountByAddress(string address)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Accounts account = new Accounts();
                AccountInfo info = new AccountInfo();
                AccountInfoOM result = await account.GetAccountByAddress(address);
                if (result != null)
                {
                    info.Address = result.Address;
                    info.Balance = result.Balance;
                    info.IsDefault = result.IsDefault;
                    info.PublicKey = result.PublicKey;
                    info.Tag = result.Tag;
                    info.WatchOnly = result.WatchOnly;
                    response.Result = Newtonsoft.Json.Linq.JToken.FromObject(info);
                }
                else
                {
                    response.Result = null;
                }
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }

        public static async Task<ApiResponse> GetNewAddress(string tag)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Accounts account = new Accounts();
                AccountInfo info = new AccountInfo();
                AccountInfoOM result = await account.GetNewAddress(tag);
                if (result != null)
                {
                    info.Address = result.Address;
                    info.Balance = result.Balance;
                    info.IsDefault = result.IsDefault;
                    info.PublicKey = result.PublicKey;
                    info.Tag = result.Tag;
                    info.WatchOnly = result.WatchOnly;
                    response.Result = Newtonsoft.Json.Linq.JToken.FromObject(info);
                }
                else
                {
                    response.Result = null;
                }
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }

        public static async Task<ApiResponse> GetDefaultAccount()
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Accounts account = new Accounts();
                AccountInfo info = new AccountInfo();
                AccountInfoOM result = await account.GetDefaultAccount();
                if (result != null)
                {
                    info.Address = result.Address;
                    info.Balance = result.Balance;
                    info.IsDefault = result.IsDefault;
                    info.PublicKey = result.PublicKey;
                    info.Tag = result.Tag;
                    info.WatchOnly = result.WatchOnly;
                    response.Result = Newtonsoft.Json.Linq.JToken.FromObject(info);
                }
                else
                {
                    response.Result = null;
                }
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }

        public static async Task<ApiResponse> SetDefaultAccount(string address)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Accounts account = new Accounts();
                await account.SetDefaultAccount(address);
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }

        public static async Task<ApiResponse> ValidateAddress(string address)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Accounts account = new Accounts();
                AddressInfo info = new AddressInfo();
                AddressInfoOM result = await account.ValidateAddress(address);
                if (result != null)
                {
                    info.Address = result.Address;
                    info.Account = result.Account;
                    info.Addresses = result.Addresses;
                    info.Hdkeypath = result.Hdkeypath;
                    info.Hdmasterkeyid = result.Hdmasterkeyid;
                    info.Hex = result.Hex;
                    info.IsCompressed = result.IsCompressed;
                    info.IsMine = result.IsMine;
                    info.IsScript = result.IsScript;
                    info.IsValid = result.IsValid;
                    info.IsWatchOnly = result.IsWatchOnly;
                    info.PubKey = result.PubKey;
                    info.Script = result.Script;
                    info.ScriptPubKey = result.ScriptPubKey;
                    info.Sigrequired = result.Sigrequired;
                    response.Result = Newtonsoft.Json.Linq.JToken.FromObject(info);
                }
                else
                {
                    response.Result = null;
                }
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }

        public static async Task<ApiResponse> SetAccountTag(string address, string tag)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Accounts account = new Accounts();
                await account.SetAccountTag(address, tag);
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }

        /// <summary>
        /// 分类获取账号，1：所有找零账户，2：所有创建账户，3：所有观察者账户，0或者其他：所有账户信息
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public static async Task<ApiResponse> GetPageAccountCategory(int category, int pageSize = 0, int pageCount = int.MaxValue, bool isSimple = true)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Accounts account = new Accounts();
                List<AccountInfo> list = new List<AccountInfo>();
                var obj = await account.GetPageAccountCategory(category, pageSize, pageCount, isSimple);
                
                if (obj != null)
                {
                    if (isSimple)
                    {
                        var result = obj as PageAccountSimpleOM;
                        foreach (var item in result.Accounts)
                        {
                            list.Add(new AccountInfo()
                            {
                                Address = item.Id,
                                Tag = item.Tag
                            });
                        }
                    }
                    else
                    {
                        var result = obj as PageAccountDetailOM;
                        foreach (var item in result.Accounts)
                        {
                            list.Add(new AccountInfo()
                            {
                                Address = item.Id,
                                Balance = item.Balance,
                                IsDefault = item.IsDefault,
                                PublicKey = item.PublicKey,
                                Tag = item.Tag,
                                WatchOnly = item.WatchOnly
                            });
                        }
                    }

                    response.Result = Newtonsoft.Json.Linq.JToken.FromObject(list);
                }
                else
                {
                    response.Result = null;
                }
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }
    }
}
