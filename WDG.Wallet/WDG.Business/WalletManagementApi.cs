// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

using WDG.ServiceAgent;
using WDG.Utility;
using WDG.Utility.Api;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WDG.Business
{
    public static class WalletManagementApi
    {
        public static async Task<ApiResponse> BackupWallet(string filePath)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                WalletManagement management = new WalletManagement();
                await management.BackupWallet(filePath);
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

        public static async Task<ApiResponse> RestoreWalletBackup(string filePath, string password = null)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                if (Path.GetExtension(filePath) == ".fcdatx" && string.IsNullOrEmpty(password))
                {
                    throw new ApiInvalidParametersException("password must not null");
                }
                else
                {
                    WalletManagement management = new WalletManagement();
                    await management.RestoreWalletBackup(filePath, password);
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

        public static async Task<ApiResponse> EncryptWallet(string password)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var regex = new Regex(@"((?=.*[0-9])(?=.*[a-zA-Z])(?=.*[!@#$%^&*\.])[a-zA-Z0-9!@#$%^&*\._-]+)|((?=.*[0-9])(?=.*[a-zA-Z])[a-zA-Z0-9]+)|((?=.*[0-9])(?=.*[!@#$%^&*\.])[0-9!@#$%^&*\._-]+)|((?=.*[!@#$%^&*\.])(?=.*[a-zA-Z])[a-zA-Z!@#$%^&*\._-]+).{7,30}");

                var mathches = regex.Matches(password).Cast<Match>();
                var gs = mathches.SelectMany(x => x.Groups.Cast<Group>());
                if (!gs.Any(x => x.Value.Equals(password) && x.Value.Length >= 8 && x.Value.Length <= 30))
                    throw new ApiCustomException(7000001, "invalid password");

                WalletManagement management = new WalletManagement();
                await management.EncryptWallet(password);
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

        public static async Task<ApiResponse> WalletPassphrase(string password)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                WalletManagement management = new WalletManagement();
                bool result = await management.WalletPassphrase(password);
                response.Result = Newtonsoft.Json.Linq.JToken.FromObject(result);
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

        public static async Task<ApiResponse> WalletLock()
        {
            ApiResponse response = new ApiResponse();
            try
            {
                WalletManagement management = new WalletManagement();
                await management.WalletLock();
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
        /// change password
        /// </summary>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public static async Task<ApiResponse> WalletPassphraseChange(string oldPassword, string newPassword)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                if (oldPassword == newPassword)
                {
                    throw new ApiCustomException(3000000, "The old password is not the same as the new password");
                }
                WalletManagement management = new WalletManagement();
                await management.WalletPassphraseChange(oldPassword, newPassword);
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
