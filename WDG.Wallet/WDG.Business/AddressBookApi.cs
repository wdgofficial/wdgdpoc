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
    public static class AddressBookApi
    {
        public static async Task<ApiResponse> AddNewAddressBookItem(string address, string label)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                AddressBook book = new AddressBook();
                await book.AddNewAddressBookItem(address, label);
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


        public static async Task<ApiResponse> UpsertAddrBookItem(long id, string address, string label)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                AddressBook book = new AddressBook();
                await book.UpsertAddrBookItem(id, address, label);
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



        public static async Task<ApiResponse> GetAddressBook()
        {
            ApiResponse response = new ApiResponse();
            try
            {
                AddressBook book = new AddressBook();
                List<AddressBookInfo> list = new List<AddressBookInfo>();
                AddressBookInfoOM[] result = await book.GetAddressBook();
                if (result != null)
                {
                    for (int i = 0; i < result.Length; i++)
                    {
                        list.Add(new AddressBookInfo()
                        {
                            Address = result[i].Address,
                            Id = result[i].Id,
                            Tag = result[i].Tag,
                            Timestamp = result[i].Timestamp
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

        public static async Task<ApiResponse> GetAddressBookItemByAddress(string address)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                AddressBook book = new AddressBook();
                AddressBookInfo info = new AddressBookInfo();
                AddressBookInfoOM result = await book.GetAddressBookItemByAddress(address);
                if (result != null)
                {


                    info.Address = result.Address;
                    info.Id = result.Id;
                    info.Tag = result.Tag;
                    info.Timestamp = result.Timestamp;
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

        public static async Task<ApiResponse> GetAddressBookByTag(string tag)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                AddressBook book = new AddressBook();
                List<AddressBookInfo> list = new List<AddressBookInfo>();
                AddressBookInfoOM[] result = await book.GetAddressBookByTag(tag);
                if (result != null)
                {
                    for (int i = 0; i < result.Length; i++)
                    {
                        list.Add(new AddressBookInfo()
                        {
                            Address = result[i].Address,
                            Id = result[i].Id,
                            Tag = result[i].Tag,
                            Timestamp = result[i].Timestamp
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

        public static async Task<ApiResponse> DeleteAddressBookByIds(long[] id)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                AddressBook book = new AddressBook();
                await book.DeleteAddressBookByIds(id);
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
