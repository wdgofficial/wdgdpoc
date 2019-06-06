// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Utility.Api;
using WDG.Wallet.Win.Common;

namespace WDG.Wallet.Win.Models
{
    public class Result<T> : Result
    {
        public T Value { get; set; }
    }

    public interface IResult
    {
        
    }

    public class Result : IResult
    {
        public bool IsFail { get; set; }
        public int ErrorCode { get; set; }
        public ApiResponse ApiResponse { get; set; }

        public string GetErrorMsg()
        {
            if (ErrorCode != 0)
            {
                return LanguageService.Default.GetErrorMsg(ErrorCode);
            }
            if(ApiResponse!=null && ApiResponse.Error!=null && ApiResponse.HasError)
            {
                return LanguageService.Default.GetErrorMsg(ApiResponse.Error.Code);
            }
            return null;
        }
    }
}
