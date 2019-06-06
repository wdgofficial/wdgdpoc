// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using NSec.Cryptography;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.Framework
{
    public class RandomNumber
    {
        public static byte[] Generate(int size)
        {
            return RandomGenerator.Default.GenerateBytes(size);
        }
    }
}
