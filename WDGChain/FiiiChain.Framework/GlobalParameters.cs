// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.Framework
{
    public class GlobalParameters
    {
        static GlobalParameters()
        {
            LocalHeight = -1;
            LocalConfirmedHeight = -1;
        }

        public static bool IsTestnet { get; set; }
        public static long LocalHeight { get; set; }
        public static long LatestBlockTime { get; set; }
        public static long LocalConfirmedHeight { get; set; }

        /// <summary>
        /// 统计所有的上链交易
        /// </summary>
        public static long TotalAmount;

        private const String CACHE_FILE_TEST = "Temp/cache_test";
        private const String CACHE_FILE_MAIN = "Temp/cache";

        public const String CACHE_FILE_DIR = "Temp";

        public static String CACHE_FILE
        {
            get
            {
                return IsTestnet ? CACHE_FILE_TEST : CACHE_FILE_MAIN;
            }
        }
    }
}
