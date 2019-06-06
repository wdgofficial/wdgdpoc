// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.Framework
{
    public class Base58
    {
        public static string Encode(byte[] bytes)
        {
            return SimpleBase.Base58.Bitcoin.Encode(bytes);
        }

        public static byte[] Decode(string text)
        {
            return SimpleBase.Base58.Bitcoin.Decode(text); 
        }
    }
}
