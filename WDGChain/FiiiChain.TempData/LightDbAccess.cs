// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.IModules;
using LightDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FiiiChain.TempData
{
    public class LightDbAccess : IHotDataAccess
    {
        LightDB.LightDB db;
        Dictionary<string, Byte[]> tables = new Dictionary<string, byte[]>();

        private byte[] Decode(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        private string Encode(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        public bool Init(string cacheFile, string defaultAddr)
        {
            tables.Add(DataCatelog.Default, Decode(DataCatelog.Default));
            tables.Add(DataCatelog.Accounts, Decode(DataCatelog.Accounts));
            tables.Add(DataCatelog.AddressBook, Decode(DataCatelog.AddressBook));
            tables.Add(DataCatelog.Output, Decode(DataCatelog.Output));
            tables.Add(DataCatelog.Payment, Decode(DataCatelog.Payment));
            tables.Add(DataCatelog.Block, Decode(DataCatelog.Block));
            tables.Add(DataCatelog.BlockSimple, Decode(DataCatelog.BlockSimple));
            tables.Add(DataCatelog.Header, Decode(DataCatelog.Header));

            var file = Path.Combine(cacheFile, "lightDb");

            db = new LightDB.LightDB();
            if (!Directory.Exists(file))
                Directory.CreateDirectory(file);
            db.Open(file, new DBCreateOption() { MagicStr = CacheConfig.MagicStr });

            if (defaultAddr == null || !defaultAddr.Equals(Get<string>(DataCatelog.Default, CacheConfig.DefaultKey)))
            {
                this.Del(DataCatelog.Default);
                this.Del(DataCatelog.Accounts);
                this.Del(DataCatelog.AddressBook);
                this.Del(DataCatelog.Output);
                this.Del(DataCatelog.Payment);
                Put(DataCatelog.Default, "DefaultAddr", defaultAddr);
                return false;
            }

            //var version = Get<string>(DataCatelog.Default, CacheConfig.Version);
            //if (string.IsNullOrEmpty(version) || !DbAccessHelper.Version.Equals(version))
            //{
            //    Del(DataCatelog.Output);
            //    Del(DataCatelog.Payment);
            //    return false;
            //}

            Put(DataCatelog.Default, "DefaultAddr", defaultAddr);
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
            var tableid = tables[catelog];
            using (var snap = db.UseSnapShot())
            {
                var writetask = db.CreateWriteTask();
                {
                    var byteKey = Decode(key);
                    writetask.Delete(tableid, byteKey);
                    db.Write(writetask);
                }
            }
        }

        public void Del(string catelog, IEnumerable<string> keys)
        {
            var tableid = tables[catelog];
            using (var snap = db.UseSnapShot())
            {
                var keyList = keys.ToList();
                int index = 0;
                int size = 1000;
                var len = keys.Count();
                while (len > size * index)
                {
                    var removeKeys = keys.Skip(size * index).Take(1000);
                    var writetask = db.CreateWriteTask();
                    {
                        removeKeys.ToList().ForEach(key =>
                        {
                            var byteKey = Decode(key);
                            writetask.Delete(tableid, byteKey);
                        });
                        db.Write(writetask);
                    }
                    index++;
                }
            }
        }

        public T Get<T>(string catelog, string key) where T : class
        {
            using (var snap = db.UseSnapShot())
            {
                var tableId = tables[catelog];
                var keyfinder = snap.CreateKeyFinder(tableId);
                var byteKey = Decode(key);
                var itemKey = keyfinder.FirstOrDefault(x => Encode(x) == key);
                if (itemKey == null)
                {
                    return default(T);
                }
                else
                {
                    var data = snap.GetValue(tableId, byteKey);
                    if (data != null)
                    {
                        if (typeof(T) == typeof(string))
                            return data.typedvalue as T;
                        return JsonConvert.DeserializeObject<T>(data.typedvalue.ToString());
                    }
                    else
                    {
                        return default(T);
                    }
                }
            }
        }

        public IEnumerable<string> GetCatelogKeys(string catelog)
        {
            List<string> result = new List<string>();

            using (var snap = db.UseSnapShot())
            {
                var tableid = tables[catelog];
                var keyfinder = snap.CreateKeyFinder(tableid);
                if (keyfinder == null)
                    return result;

                var keys = keyfinder.Select(x => Encode(x));
                result.AddRange(keys);
                return result;
            }
        }

        public IEnumerable<string> GetCatelogKeys(string catelog, Predicate<string> predicate)
        {
            List<string> result = new List<string>();

            using (var snap = db.UseSnapShot())
            {
                var tableid = tables[catelog];
                var keyfinder = snap.CreateKeyFinder(tableid);
                if (keyfinder == null)
                    return result;

                var keys = keyfinder.Select(x => Encode(x)).Where(x => predicate(x));
                result.AddRange(keys);
                return result;
            }
        }

        public void Put(string catelog, string key, string value)
        {
            using (var snap = db.UseSnapShot())
            {
                var writetask = db.CreateWriteTask();
                {
                    var tableId = tables[catelog];

                    var byteKey = Decode(key);
                    var dbValue = LightDB.DBValue.FromValue(LightDB.DBValue.Type.String, value);
                    writetask.Put(tableId, byteKey, dbValue);
                    db.Write(writetask);
                }
            }
        }

        public void Put<T>(string catelog, string key, T value) where T : class
        {
            using (var snap = db.UseSnapShot())
            {
                var writetask = db.CreateWriteTask();
                {
                    var tableId = tables[catelog];

                    var byteKey = Decode(key);
                    var json = JsonConvert.SerializeObject(value);
                    var dbValue = LightDB.DBValue.FromValue(LightDB.DBValue.Type.String, json);
                    writetask.Put(tableId, byteKey, dbValue);
                    db.Write(writetask);
                }
            }
        }

        public void Put<T>(string catelog, IEnumerable<KeyValuePair<string, T>> keyValuePairs) where T : class
        {
            using (var snap = db.UseSnapShot())
            {
                var writetask = db.CreateWriteTask();
                {
                    var tableId = tables[catelog];

                    foreach (var value in keyValuePairs)
                    {
                        var byteKey = Decode(value.Key);
                        var json = JsonConvert.SerializeObject(value);
                        var dbValue = LightDB.DBValue.FromValue(LightDB.DBValue.Type.String, json);
                        writetask.Put(tableId, byteKey, dbValue);
                        db.Write(writetask);
                    }
                }
            }
        }

        List<T> IHotDataAccess.Get<T>(string catelog, IEnumerable<string> keys)
        {
            List<T> result = new List<T>();
            if (keys == null || keys.Count() == 0)
                return result;

            using (var snap = db.UseSnapShot())
            {
                var tableId = tables[catelog];
                var keyfinder = snap.CreateKeyFinder(tableId);

                var ks = keyfinder.Where(x => keys.Contains(Encode(x)));

                foreach (var key in keys)
                {
                    var byteKey = Decode(key);
                    var data = snap.GetValue(tableId, byteKey);
                    if (data != null)
                    {
                        result.Add(JsonConvert.DeserializeObject<T>(data.typedvalue.ToString()));
                    }
                }
            }
            return result;
        }
    }
}
