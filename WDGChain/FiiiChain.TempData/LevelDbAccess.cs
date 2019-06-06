// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FiiiChain.IModules;
using LevelDB;
using Newtonsoft.Json;

namespace FiiiChain.TempData
{
    public class LevelDbAccess : IHotDataAccess
    {
        DB _db;
        public bool Init(string cacheFile, string defaultAddr)
        {
            var file = Path.Combine(cacheFile, "levelDb");
            var options = new Options { CreateIfMissing = true };
            try
            {
                _db = new DB(options, file);
            }
            catch (UnauthorizedAccessException exUa)
            {
                throw new Exception($"An LevelDb exception occurs  new DB function,Details:{exUa.ToString()}");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            var val = Get<string>(DataCatelog.Default, CacheConfig.DefaultKey);
            if (string.IsNullOrEmpty(val) || !defaultAddr.Equals(Get<string>(DataCatelog.Default, CacheConfig.DefaultKey)))
            {
                Del(DataCatelog.Accounts);
                Del(DataCatelog.Default);
                Del(DataCatelog.AddressBook);
                Del(DataCatelog.Output);
                Del(DataCatelog.Payment);
                Put(DataCatelog.Default, CacheConfig.DefaultKey, defaultAddr);
                return false;
            }
            //var version = Get<string>(DataCatelog.Default, CacheConfig.Version);
            //if (string.IsNullOrEmpty(version) || !DbAccessHelper.Version.Equals(version))
            //{
            //    Del(DataCatelog.Output);
            //    Del(DataCatelog.Payment);
            //    //Put(DataCatelog.Default,CacheConfig.Version,)
            //    return false;
            //}
            return true;
        }
        public void Del(string catelog)
        {
            var keys = GetCatelogKeys(catelog);
            if (keys != null && keys.Count() > 0)
                Del(catelog, keys);
        }

        public void Del(string catelog, string key)
        {
            var cacheKey = GetCacheKey(catelog, key);
            _db.Delete(cacheKey);
        }

        public void Del(string catelog, IEnumerable<string> keys)
        {
            using (var batch = new WriteBatch())
            {
                var cacheKeys = keys.Select(key => GetCacheKey(catelog, key));
                foreach (var key in cacheKeys)
                {
                    batch.Delete(key);
                }
                _db.Write(batch);
            }
        }

        public T Get<T>(string catelog, string key) where T : class
        {
            try
            {
                var cacheKey = GetCacheKey(catelog, key);
                var json = _db.Get(cacheKey);
                if (typeof(T) == typeof(string))
                    return json as T;
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        public IEnumerable<string> GetCatelogKeys(string catelog)
        {
            var keys = from kv in _db as IEnumerable<KeyValuePair<string, string>> where kv.Key.StartsWith(catelog) select kv.Key;
            return keys.Select(key => key.Replace($"{catelog}_", ""));
        }
        
        /// <summary>
        /// 这个方法后续有修改2019.4.30
        /// </summary>
        /// <param name="catelog"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<string> GetCatelogKeys(string catelog, Predicate<string> predicate)
        {
            var keys = from kv in _db as IEnumerable<KeyValuePair<string, string>>
                       where kv.Key.StartsWith(catelog) && predicate(kv.Key.Replace($"{catelog}_", ""))
                       select kv.Key;
            return keys.Select(key => key.Replace($"{catelog}_", ""));
        }

        public void Put(string catelog, string key, string value)
        {
            var cacheKey = GetCacheKey(catelog, key);
            _db.Put(cacheKey, value);
        }

        public void Put<T>(string catelog, string key, T value) where T : class
        {
            var json = JsonConvert.SerializeObject(value);
            Put(catelog, key, json);
        }

        public void Put<T>(string catelog, IEnumerable<KeyValuePair<string, T>> keyValuePairs) where T : class
        {
            if (keyValuePairs == null)
                throw new ArgumentNullException("keyValuePairs");
            using (var batch = new WriteBatch())
            {
                foreach (var item in keyValuePairs)
                {
                    var key = GetCacheKey(catelog, item.Key);
                    _db.Put(key, JsonConvert.SerializeObject(item.Value));
                }
                var writeOptions = new WriteOptions { Sync = true };
                _db.Write(batch, writeOptions);
            }
        }
        
        private string GetCacheKey(string catelog, string key)
        {
            return $"{catelog}_{key}";
        }

        List<T> IHotDataAccess.Get<T>(string catelog, IEnumerable<string> keys)
        {
            var result = new List<T>();
            foreach (var key in keys)
            {
                var val = Get<T>(catelog, key);
                if (val != null)
                    result.Add(val);
            }
            return result;
        }
    }
}
