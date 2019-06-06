// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.ShareModels
{
    public class MsgType
    {
        public const string StartMining = "StartMining";
        public const string StopMining = "StopMining";
        public const string Login = "Login";
        public const string ForgetBlock = "ForgetBlock";
        public const string HeartPool = "HeartPool";
    }
}
