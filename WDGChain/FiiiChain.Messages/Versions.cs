// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.Messages
{
    public class Versions
    {
        public static int EngineVersion
        {
            get
            {
                return int.Parse(Resource.EngineVersion);
            }
        }

        public static int MsgVersion
        {
            get
            {
                return int.Parse(Resource.MsgVersion);
            }
        }

        public static int MinimumSupportVersion
        {
            get
            {
                return int.Parse(Resource.MinimumSupportVersion);
            }
        }
    }
}
