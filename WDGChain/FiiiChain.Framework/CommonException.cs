// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.Framework
{
    public class CommonException : Exception
    {
        public CommonException(int errorCode)
        {
            this.ErrorCode = errorCode;
        }

        public CommonException(int errorCode, Exception innerException):base("", innerException)
        {
            this.ErrorCode = errorCode;            
        }

        public int ErrorCode { get; set; }

        public override string Message
        {
            get
            {
                return "Error Code: " + ErrorCode;
            }
        }
    }
}
