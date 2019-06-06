// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.Messages
{
    public class GetAddrMsg : BasePayload
    {
        public int Count { get; set; }

        public override void Deserialize(byte[] bytes, ref int index)
        {
            var countBytes = new byte[4];

            Array.Copy(bytes, index, countBytes, 0, countBytes.Length);
            index += countBytes.Length;

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(countBytes);
            }

            this.Count = BitConverter.ToInt32(countBytes, 0);
        }

        public override byte[] Serialize()
        {
            var countBytes = BitConverter.GetBytes(this.Count);

            if(BitConverter.IsLittleEndian)
            {
                Array.Reverse(countBytes);
            }

            return countBytes;
        }
    }
}
