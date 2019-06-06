// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace FiiiChain.Framework
{
    public class HashHelper
    {
        public static byte[] Hash(byte[] bytes)
        {
            var sha256 = SHA256Managed.Create();
            return sha256.ComputeHash(bytes);
        }

        public static byte[] DoubleHash(byte[] bytes)
        {
            return Hash(
                    Hash(bytes)
                );
        }

        public static byte[] Hash160(byte[] bytes)
        {
            var sha256 = SHA256Managed.Create();
            var ripemd160 = RIPEMD160Managed.Create();

            return ripemd160.ComputeHash(
                    sha256.ComputeHash(bytes)
                );
        }

        public static byte[] EmptyHash()
        {
            return new byte[32];
        }
    }
}
