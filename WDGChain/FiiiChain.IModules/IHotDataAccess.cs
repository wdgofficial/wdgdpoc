// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.IModules
{
    public static class CacheConfig
    {
        public static string MagicStr = "FiiiChain";
        public static string DefaultKey = "DefaultAddr";
        public static string Version = "Version";
    }

    public static class DataCatelog
    {
        public static string Default = "Default";
        public static string Payment = "Payment";
        public static string Accounts = "Accounts";
        public static string AddressBook = "AddressBook";
        public static string Output = "Output";
        public static string Header = "Header";
        public static string Block = "Block";
        public static string BlockSimple = "BlockSimple";
    }


    public interface IHotDataAccess
    {
        bool Init(string cacheFile, string defaultAddr);

        void Put(string catelog, string key, string value);
        void Put<T>(string catelog, string key, T value) where T : class;
        void Put<T>(string catelog, IEnumerable<KeyValuePair<string, T>> keyValuePairs) where T : class;

        T Get<T>(string catelog, string key) where T : class;
        List<T> Get<T>(string catelog, IEnumerable<string> keys) where T : class;
        IEnumerable<string> GetCatelogKeys(string catelog);
        IEnumerable<string> GetCatelogKeys(string catelog, Predicate<string> predicate);

        void Del(string catelog);
        void Del(string catelog, string key);
        void Del(string catelog, IEnumerable<string> keys);
    }
}