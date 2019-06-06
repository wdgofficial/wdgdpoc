// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.Messages
{
    public class NewMiningPoolMsg : BasePayload
    {
        public NewMiningPoolMsg()
        {
            MinerInfo = new MiningMsg();
        }

        public MiningMsg MinerInfo { get; set; }

        public override void Deserialize(byte[] bytes, ref int index)
        {
            MinerInfo.Deserialize(bytes, ref index);
        }

        public override byte[] Serialize()
        {
            return MinerInfo.Serialize();
        }
    }
}