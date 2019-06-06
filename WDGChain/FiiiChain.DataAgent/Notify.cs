// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using FiiiChain.Framework;


namespace FiiiChain.DataAgent
{
    public class Notify
    {
        public static Notify Current;
        public string CallbackApp { get; set; }

        static Notify()
        {
            Current = new Notify();
        }

        public void NewTxReceived(string txHash)
        {
            LogHelper.Info($"Notify-NewTxReceived,CallbackApp={CallbackApp},txHash={txHash}");
            if (string.IsNullOrWhiteSpace(CallbackApp) || string.IsNullOrWhiteSpace(txHash))
            {
                LogHelper.Error($"Notify-NewTxReceived,CallbackApp is null or txHash is null");
                return;
            }

            try
            {
                ProcessStartInfo info = new ProcessStartInfo(CallbackApp);
                info.Arguments = txHash;
                info.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(info);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message, ex);
            }

            //ProcessStartInfo info = new ProcessStartInfo(CallbackApp.Split(' ')[0]);
            //if (!string.IsNullOrEmpty(CallbackApp.Split(' ')[1]) && CallbackApp.Split(' ')[1].Contains("%s"))
            //{
            //    info.Arguments = CallbackApp.Split(' ')[1].Replace("%s", txHash);
            //}
            //try
            //{
            //    info.WindowStyle = ProcessWindowStyle.Hidden;
            //    Process.Start(info);
            //}
            //catch(Exception ex)
            //{
            //    LogHelper.Error(ex.Message, ex);
            //}
        }
    }
}
