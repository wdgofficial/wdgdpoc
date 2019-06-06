// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace FiiiChain.DataAgent
{
    public class CacheManager
    {
        private static CacheManager _default;
        public static CacheManager Default
        {
            get
            {
                if (_default == null)
                {
                    _default = new CacheManager();
                }
                return _default;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="catelog"></param>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="isMissInsert">为true时，如果不存在则添加，存在则不添加</param>
        public void Put<T>(string catelog, string key, T obj, bool isMissInsert = true) where T :class
        {
            CacheAccess.Default.Put(catelog, key, obj);
        }

        public void Put<T>(string catelog, IEnumerable<KeyValuePair<string, T>> keyValues, bool isMissInsert = true) where T : class
        {
            if (keyValues == null)
                return;

            CacheAccess.Default.Put(catelog, keyValues);
        }

        public void DeleteAll(string catelog)
        {
            CacheAccess.Default.Del(catelog);
        }

        public void DeleteByKey(string catelog, string key)
        {
            CacheAccess.Default.Del(catelog, key);
        }

        public void DeleteByKeys(string catelog, IEnumerable<string> keys)
        {
            CacheAccess.Default.Del(catelog, keys);
        }

        public List<T> Get<T>(string catelog, Func<string, bool> filterKey = null) where T : class
        {
            IEnumerable<string> allKeys = null;
            if (filterKey != null)
            {
                allKeys = CacheAccess.Default.GetCatelogKeys(catelog, key => filterKey(key));
            }
            else
            {
                allKeys = CacheAccess.Default.GetCatelogKeys(catelog);
            }
            if (allKeys == null || !allKeys.Any())
                return new List<T>();

            ConcurrentQueue<string> keys = new ConcurrentQueue<string>();
            
            var result = CacheAccess.Default.Get<T>(catelog, allKeys);
            return result;
        }

        public T Get<T>(string catelog,string key) where T : class
        {
            var result = CacheAccess.Default.Get<T>(catelog, key);
            return result;
        }

        public List<string> GetAllKeys(string catelog)
        {
            var allKeys = CacheAccess.Default.GetCatelogKeys(catelog);
            if (allKeys == null || !allKeys.Any())
                return new List<string>();
            return allKeys.ToList();
        }

        public IEnumerable<string> GetAllKeys(string catelog, Predicate<string> predicate)
        {
            var allKeys = CacheAccess.Default.GetCatelogKeys(catelog, predicate);
            return allKeys;
        }

    }
}