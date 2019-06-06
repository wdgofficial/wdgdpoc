// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace FiiiChain.Framework
{
    public enum OSPlatformType
    {
        Linux,
        OSX,
        WINDOWS,
        OTHERS
    }

    public static class RunTime
    {
        public static OSPlatformType GetOSPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return OSPlatformType.Linux;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return OSPlatformType.OSX;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return OSPlatformType.WINDOWS;
            }
            return OSPlatformType.OTHERS;
        }
    }
}
