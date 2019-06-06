// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.Messages
{
    public class NewTxMsg : BasePayload
    {
        public string Hash { get; set; }

        public override void Deserialize(byte[] bytes, ref int index)
        {
            var hashBytes = new byte[32];
            Array.Copy(bytes, index, hashBytes, 0, hashBytes.Length);
            index += hashBytes.Length;

            this.Hash = Base16.Encode(hashBytes);
        }

        public override byte[] Serialize()
        {
            return Base16.Decode(Hash);
        }
    }
}
