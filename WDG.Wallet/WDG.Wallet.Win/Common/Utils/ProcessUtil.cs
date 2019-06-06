// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace WDG.Wallet.Win.Common.Utils
{
    public class ProcessUtil
    {
        public static List<int> Find(string progressName, string args)
        {
            List<int> result = new List<int>();
            try
            {
                ManagementClass mngmtClass = new ManagementClass("Win32_Process");
                var instances = mngmtClass.GetInstances();
                foreach (ManagementObject o in instances)
                {
                    var name = o["Name"].ToString().ToLower();

                    if (!name.Equals(progressName.ToLower()))
                        continue;

                    var commandLine = o["CommandLine"].ToString().ToLower();

                    if (commandLine.EndsWith((args.ToLower())))
                    {
                        var processid = o["ProcessId"].ToString();
                        result.Add(int.Parse(processid));
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Singleton.Debug(ex.ToString());
            }
            return result;
        }

        public static void Kill(List<int> processIds)
        {
            foreach (var id in processIds)
            {
                Process.GetProcessById(id).Kill();
            }
        }
    }
}
