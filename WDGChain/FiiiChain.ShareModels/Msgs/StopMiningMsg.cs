// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.ShareModels.Msgs
{
    [Serializable]
    public class StopMiningMsg
    {
        public static StopMiningMsg CreateNew()
        {
            var msg = new StopMiningMsg();
            return msg;
        }
        
        public StopReason StopReason;

        public int CurrentHeight;

        public string StartMsgId;

        public long StopTime;
    }
}