// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WDG.Wallet.Win.Common.Detects
{
    public class DetectNetCore
    {
        Version VERSION;

        private bool Detect()
        {
            var dir = @"C:\Program Files\dotnet\shared\Microsoft.NETCore.App";
            var dir_x86 = @"C:\Program Files (x86)\dotnet\shared\Microsoft.NETCore.App";
            string currentDir = null;
            if (!Directory.Exists(dir) && !Directory.Exists(dir_x86))
                return false;

            if (Directory.Exists(dir))
                currentDir = dir;
            else
                currentDir = dir_x86;

            var dirs = Directory.GetDirectories(currentDir);
            if (currentDir == null || !currentDir.Any())
                return false;
            List<Version> lsv = new List<Version>();
            foreach (var item in dirs)
            {
                try
                {
                    var name = item.Split('\\').LastOrDefault();
                    var splitIndex = item.IndexOf("-");
                    name = name.Split('-')[0];
                    Version version = new Version(name);
                    lsv.Add(version);
                }
                catch
                { }
            }

            return lsv.Any(x => x > VERSION);
        }

        public bool DetectVersion(string ver)
        {
            VERSION = new Version(ver);
            return Detect();
        }
    }
}
