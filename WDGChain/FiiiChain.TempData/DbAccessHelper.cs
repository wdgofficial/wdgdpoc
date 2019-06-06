// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.Framework;
using FiiiChain.IModules;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.TempData
{
    public static class DbAccessHelper
    {
        public static string Version = "0.1.1";

        public static IHotDataAccess GetDataAccessInstance()
        {
            IHotDataAccess dataAccess = null;
            var type = RunTime.GetOSPlatform();
            switch (type)
            {
                case OSPlatformType.Linux:
                case OSPlatformType.OSX:
                    dataAccess = new LightDbAccess();
                    break;
                case OSPlatformType.WINDOWS:
                    dataAccess = new LevelDbAccess();
                    break;
                default:
                    LogHelper.Error("other system");
                    dataAccess = new LightDbAccess();
                    break;
            }
            return dataAccess;
        }
    }
}
