// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.Framework
{
    public class Base16
    {
        public static string Encode(byte[] bytes)
        {
            return SimpleBase.Base16.EncodeUpper(bytes);
        }

        public static byte[] Decode(string text)
        {
            return SimpleBase.Base16.Decode(text);
        }

        public static object Encode(object p)
        {
            throw new NotImplementedException();
        }
    }
}
