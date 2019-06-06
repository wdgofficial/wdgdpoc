// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDG.Wallet.Win.Common.Detects
{
    public class DetectInRule
    {
        const string OpenRule = "netsh advfirewall firewall add rule name = \"WDG\" protocol=UDP dir =in localport=20302,20312,20101,20111 action = allow";
        const string DelRule = "netsh advfirewall firewall delete rule name=\"WDG\" dir=in";

        public static void UpdateInRule()
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.Verb = "runas";
            p.Start();

            p.StandardInput.WriteLine(DelRule);
            p.StandardInput.WriteLine(OpenRule + "&exit");
            //p.StandardInput.WriteLine(strInput + "&exit");
            string strOuput = p.StandardOutput.ReadToEnd();
            WDG.Utility.Logger.Singleton.Info(strOuput);
            p.WaitForExit();
            p.Close();
        }
    }
}
