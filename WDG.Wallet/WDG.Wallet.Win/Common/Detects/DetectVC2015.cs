// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WDG.Wallet.Win.Common.Detects
{
    public class DetectVC2015
    {
        private List<string> GetInstalledApp()
        {
            List<string> appNames = new List<string>();
            string tempType = null;
            object displayName = null, uninstallString = null, releaseType = null;
            RegistryKey currentKey = null;
            RegistryKey pregkey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall");//获取指定路径下的键
            try
            {
                foreach (string item in pregkey.GetSubKeyNames())               //循环所有子键
                {
                    currentKey = pregkey.OpenSubKey(item);
                    displayName = currentKey.GetValue("DisplayName");           //获取显示名称
                    uninstallString = currentKey.GetValue("UninstallString");   //获取卸载字符串路径
                    releaseType = currentKey.GetValue("ReleaseType");           //发行类型,值是Security Update为安全更新,Update为更新
                    bool isSecurityUpdate = false;
                    if (releaseType != null)
                    {
                        tempType = releaseType.ToString();
                        if (tempType == "Security Update" || tempType == "Update")
                            isSecurityUpdate = true;
                    }
                    if (!isSecurityUpdate && displayName != null && uninstallString != null)
                    {
                        appNames.Add(displayName.ToString().ToLower());
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Singleton.Error(ex.ToString());
            }
            if (pregkey != null)
                pregkey.Close();
            return appNames;
        }

        public bool Detect()
        {
            var startText = "microsoft visual c++ ";
            var apps = GetInstalledApp();
            var vcs = apps.Where(x => x.StartsWith(startText));
            foreach (var name in vcs)
            {
                var str = name.Replace(startText, "");
                var splitStrs= str.Split(' ');
                int version = 0;
                if (int.TryParse(splitStrs[0], out version) && version >= 2015)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
