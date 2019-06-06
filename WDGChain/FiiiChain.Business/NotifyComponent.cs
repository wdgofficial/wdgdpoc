// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.DataAgent;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.Business
{
    public class NotifyComponent
    {
        public void SetCallbackApp(string appFilePath)
        {
            Notify.Current.CallbackApp = appFilePath;
        }

        public void ProcessNewTxReceived(string txHash)
        {
            Notify.Current.NewTxReceived(txHash);
        }
    }
}
