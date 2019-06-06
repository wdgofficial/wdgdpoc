// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.Consensus;
using FiiiChain.Data;
using FiiiChain.Data.Accesses;
using FiiiChain.Entities;
using FiiiChain.Entities.CacheModel;
using FiiiChain.Entities.ExtensionModels;
using FiiiChain.Framework;
using FiiiChain.IModules;
using FiiiChain.Messages;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace FiiiChain.DataAgent
{
    public class TransactionPool
    {
        private static TransactionPool instance;
        public SafeCollection<TransactionPoolItem> MainPool;
        public SafeCollection<TransactionPoolItem> IsolateTransactionPool;
        public const string containerName = "Transaction";

        public static void Remove(string name)
        {
            Storage.Instance.Delete(containerName, name);
        }

        private string GetAddress(string lockScript)
        {
            var publicKeyHash = Base16.Decode(Script.GetPublicKeyHashFromLockScript(lockScript));
            var address = AccountIdHelper.CreateAccountAddressByPublicKeyHash(publicKeyHash);
            return address;
        }

        public Dictionary<string, string> MyLockScripts = new Dictionary<string, string>();

        public TransactionPool()
        {
            this.MainPool = new SafeCollection<TransactionPoolItem>();
            this.IsolateTransactionPool = new SafeCollection<TransactionPoolItem>();
        }
        /// <summary>
        /// 删除双花的交易
        /// </summary>
        /// <param name="costItems"></param>
        public void ClearCostUtxo(List<string> costItems = null)
        {
            if (costItems == null)
            {
                var inputDac = InputDac.Default;
                costItems = inputDac.GetCostUtxos();
            }
            var costTx = MainPool.Where(x => x.Transaction.Inputs.Any(input => costItems.Contains($"{input.OutputTransactionHash}_{input.OutputIndex}")));
            foreach (var item in costTx)
            {
                BlacklistTxs.Current.Add(item.Transaction.Hash);
                BlacklistTxs.Current.AddToBlackFile(item.Transaction);
                this.RemoveTransaction(item.Transaction.Hash);
            }
        }

        public void Load()
        {
            var keys = Storage.Instance.GetAllKeys(containerName);

            var items = keys.Select(key => Storage.Instance.GetData<TransactionPoolItem>(containerName, key)).ToList();
            items.RemoveAll(x => null == x);
            //remove BlackHash
            var blackeds = items.Where(x => BlacklistTxs.Current.IsBlacked(x.Transaction.Hash));
            foreach (var blacked in blackeds)
            {
                BlacklistTxs.Current.AddToBlackFile(blacked.Transaction);
                BlacklistTxs.Current.Add(blacked.Transaction.Hash);
            }

            items.RemoveAll(x => BlacklistTxs.Current.IsBlacked(x.Transaction.Hash));

            var mainPoolItems = items;// items.Where(x => !x.Isolate);
            this.MainPool.AddRange(mainPoolItems);

            var lockScripts = items.SelectMany(x => x.Transaction.Outputs).Select(x => x.LockScript).Distinct();

            var accountList = CacheManager.Default.Get<Account>(DataCatelog.Accounts);
            if (accountList == null || accountList.Count == 0)
                return;

            var addressList = accountList.Select(x => x.Id);
            foreach (var lockScript in lockScripts)
            {
                var address = GetAddress(lockScript);
                if (addressList.Contains(address))
                {
                    MyLockScripts.Add(lockScript, address);
                }
            }
        }

        Func<string, bool> UtxoIsSpentFunc = key => CacheManager.Default.Get<Output>(DataCatelog.Output, key) != null;

        public void Init()
        {
            var items = MainPool.ToArray();

            var myTxs = items.Where(x =>
                x.Transaction.Outputs.Any(outpot => MyLockScripts.ContainsKey(outpot.LockScript)) ||
                x.Transaction.Inputs.Any(input =>
                    UtxoIsSpentFunc($"{input.OutputTransactionHash}_{input.OutputIndex}")));

            foreach (var mytx in myTxs)
            {
                var entity = Converters.ConvertToPayment(mytx);

                foreach (var payment in entity)
                {
                    CacheManager.Default.Put(DataCatelog.Payment, payment.ToString(), payment, false);
                }
            }
        }

        public static TransactionPool Instance
        {
            get
            {
                if (instance == null)
                    instance = new TransactionPool();
                return instance;
            }
        }

        public int Count
        {
            get
            {
                return this.MainPool.Count;
            }
        }

        public void AddNewTransaction(long feeRate, TransactionMsg transaction)
        {
            if (transaction.Inputs.Select(x => x.OutputTransactionHash + x.OutputIndex).Distinct().Count() < transaction.InputCount)
            {
                BlacklistTxs.Current.Add(transaction.Hash);
                LogHelper.Warn($"Transaction has Repeat inputs in {transaction.Hash}");
                return;
            }

            if (!MainPool.Any(item => item.Transaction.Hash == transaction.Hash))
            {
                var item = new TransactionPoolItem(feeRate, transaction);
                item.Isolate = false;
                this.MainPool.Add(item);
                //LogHelper.Debug($"current date time is {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fffffff")}");
                if (transaction != null)
                {
                    var entity = Converters.ConvertToPayment(item, true);
                    //LogHelper.Debug($"current date time is {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fffffff")}");
                    foreach (var payment in entity)
                    {
                        CacheManager.Default.Put(DataCatelog.Payment, payment.ToString(), payment, false);
                    }
                    //LogHelper.Debug($"current date time is {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fffffff")}");
                }

                Storage.Instance.PutData(containerName, transaction.Hash, item);
                //LogHelper.Debug($"current date time is {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fffffff")}");
            }
        }

        public void AddIsolateTransaction(long feeRate, TransactionMsg transaction)
        {
            if (!this.IsolateTransactionPool.Any(item => item.Transaction.Hash == transaction.Hash))
            {
                var item = new TransactionPoolItem(feeRate, transaction);
                item.Isolate = true;
                this.IsolateTransactionPool.Add(item);
                if (transaction != null)
                {
                    var entity = Converters.ConvertToPayment(item, true);
                    foreach (var payment in entity)
                    {
                        CacheManager.Default.Put(DataCatelog.Payment, payment.ToString(), payment, false);
                    }
                }
                Storage.Instance.PutData(containerName, transaction.Hash, item);
            }
        }

        public void MoveIsolateTransactionToMainPool(string trasactionHash)
        {
            var txItem = this.IsolateTransactionPool.FirstOrDefault(i => i.Transaction.Hash == trasactionHash);

            if (txItem != null)
            {
                txItem.Isolate = false;
                this.MainPool.Add(txItem);
                this.IsolateTransactionPool.Remove(txItem);
            }
        }

        public void RemoveTransaction(string txHash)
        {
            TransactionPoolItem currentTx = null;

            if (MainPool.Count > 0)
            {
                currentTx = MainPool.FirstOrDefault(t => t.Transaction.Hash == txHash);
                if (currentTx != null)
                {
                    MainPool.Remove(currentTx);
                }
            }

            if (IsolateTransactionPool.Count > 0)
            {
                var item = IsolateTransactionPool.FirstOrDefault(t => t.Transaction.Hash == txHash);
                if (item != null)
                {
                    IsolateTransactionPool.Remove(item);
                    if (currentTx == null)
                        currentTx = item;
                }
            }

            if (currentTx != null)
            {
                var match = $"_{currentTx.Transaction.Timestamp}_{txHash}_";
                
                var keys = CacheManager.Default.GetAllKeys(DataCatelog.Payment,
                    key => key != null && key.Contains((match)));

                if (keys != null && keys.Any())
                {
                    CacheManager.Default.DeleteByKeys(DataCatelog.Payment, keys);
                }

                Storage.Instance.Delete(containerName, currentTx.Transaction.Hash);
            }
        }

        public void RemoveTransactions(IEnumerable<string> txHashes)
        {
            List<TransactionPoolItem> txItems = new List<TransactionPoolItem>();

            var items = MainPool.Where(x => txHashes.Contains(x.Transaction.Hash)).ToList();
            if (items.Any())
            {
                txItems.AddRange(items);
                MainPool.RemoveAll(x => items.Contains(x));
            }

            var isolateItems = IsolateTransactionPool.Where(x => txHashes.Contains(x.Transaction.Hash)).ToList();
            if (isolateItems.Any())
            {
                txItems.AddRange(isolateItems);
                IsolateTransactionPool.RemoveAll(x => isolateItems.Contains(x));
            }

            if (txItems.Any())
            {
                var matches = txItems.Select(x => $"_{x.Transaction.Timestamp}_{x.Transaction.Hash}_");

                var keys = CacheManager.Default.GetAllKeys(DataCatelog.Payment,
                    key => key != null && matches.Any(match => key.Contains(match)));

                if (keys != null && keys.Any())
                {
                    CacheManager.Default.DeleteByKeys(DataCatelog.Payment, keys);
                }

                txItems.ForEach(tx => { Storage.Instance.Delete(containerName, tx.Transaction.Hash); });
            }
        }

        public TransactionMsg GetMaxFeeRateTransaction()
        {
            if (MainPool.Count > 0)
            {
                var item = MainPool.OrderByDescending(t => t.FeeRate).FirstOrDefault();

                if (item != null)
                {
                    return item.Transaction;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public TransactionMsg SearchByTransactionHash(string hash)
        {
            var item = this.MainPool.FirstOrDefault(t => t.Transaction.Hash == hash);
            if (item != null)
                return item.Transaction;
            else
                return null;
        }

        public List<TransactionMsg> SearchByAccountId(string accountId)
        {
            var publicKeyText = Base16.Encode(
                    AccountIdHelper.GetPublicKeyHash(accountId)
                );

            var result = this.MainPool.Where(t => t.Transaction.Outputs.Where(o => o.LockScript.Contains(publicKeyText)).Count() > 0).
                Select(t => t.Transaction).ToList();

            return result;
        }

        public Dictionary<string, List<OutputMsg>> GetTransactionOutputsByAccountId(string accountId)
        {
            var dict = new Dictionary<string, List<OutputMsg>>();

            foreach (TransactionPoolItem item in this.MainPool)
            {
                var tx = item.Transaction;
                if (!dict.ContainsKey(tx.Hash))
                {
                    dict.Add(tx.Hash, new List<OutputMsg>());
                }

                foreach (var output in tx.Outputs)
                {
                    var publicKeyHash = Script.GetPublicKeyHashFromLockScript(output.LockScript);
                    var id = AccountIdHelper.CreateAccountAddressByPublicKeyHash(Base16.Decode(publicKeyHash));

                    if (id == accountId)
                    {
                        dict[tx.Hash].Add(output);
                    }
                }
            }

            return dict;
        }

        public OutputMsg GetOutputMsg(string transactionHash, int index)
        {
            foreach (TransactionPoolItem item in this.MainPool)
            {
                var tx = item.Transaction;

                if (tx.Hash == transactionHash && tx.Outputs.Count > index)
                {
                    return tx.Outputs[index];
                }
            }

            return null;
        }

        public bool CheckUTXOSpent(string currentTxHash, string outputTxHash, int outputIndex)
        {
            foreach (TransactionPoolItem item in this.MainPool)
            {
                var tx = item.Transaction;

                if (tx.Hash == currentTxHash)
                {
                    continue;
                }

                foreach (var input in tx.Inputs)
                {
                    if (input.OutputTransactionHash == outputTxHash && input.OutputIndex == outputIndex)
                    {
                        return true;
                    }
                }
            }

            foreach (TransactionPoolItem item in this.IsolateTransactionPool)
            {
                var tx = item.Transaction;

                if (tx.Hash == currentTxHash)
                {
                    continue;
                }

                foreach (var input in tx.Inputs)
                {
                    if (input.OutputTransactionHash == outputTxHash && input.OutputIndex == outputIndex)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public List<TransactionMsg> GetIsolateTransactions()
        {
            return this.IsolateTransactionPool.Select(i => i.Transaction).ToList();
        }

        public List<string> GetAllTransactionHashes()
        {
            var hashes = new List<string>();
            foreach (TransactionPoolItem item in this.MainPool)
            {
                hashes.Add(item.Transaction.Hash);
            }

            foreach (TransactionPoolItem item in this.IsolateTransactionPool)
            {
                hashes.Add(item.Transaction.Hash);
            }

            return hashes;
        }

        public List<TransactionMsg> GetAllTransactions()
        {
            var txList = new List<TransactionMsg>();

            foreach (TransactionPoolItem item in this.MainPool)
            {
                txList.Add(item.Transaction);
            }

            foreach (TransactionPoolItem item in this.IsolateTransactionPool)
            {
                txList.Add(item.Transaction);
            }

            return txList;
        }

        public IEnumerable<TransactionMsg> GetTransactionByHashes(IEnumerable<string> hashes)
        {
            List<TransactionMsg> txs = new List<TransactionMsg>();

            var allTxs = new List<TransactionMsg>();
            allTxs.AddRange(this.MainPool.Select(x => x.Transaction));
            allTxs.AddRange(this.IsolateTransactionPool.Select(x => x.Transaction));

            txs.AddRange(allTxs.Where(x => hashes.Contains(x.Hash)));

            return txs;
        }

        public TransactionMsg GetTransactionByInputTx(string inputTxHash, int inputTxIndx)
        {
            foreach (TransactionPoolItem item in this.MainPool)
            {
                var tx = item.Transaction;

                if (tx.Inputs.Any(i => i.OutputTransactionHash == inputTxHash && i.OutputIndex == inputTxIndx))
                {
                    return tx;
                }
            }

            foreach (TransactionPoolItem item in this.IsolateTransactionPool)
            {
                var tx = item.Transaction;

                if (tx.Inputs.Any(i => i.OutputTransactionHash == inputTxHash && i.OutputIndex == inputTxIndx))
                {
                    return tx;
                }
            }
            return null;
        }

        public bool HasCostUtxo(IEnumerable<string> hashIndexs)
        {
            var items = MainPool.ToArray();
            var inputs = items.SelectMany(x => x.Transaction.Inputs);
            var hashIndexList = inputs.Select(input => input.OutputTransactionHash + input.OutputIndex);
            var removeCount = hashIndexList.ToList().RemoveAll(x => hashIndexs.Contains(x));
            return removeCount > 0;
        }

        public void AddAccount(string address)
        {
            if (!MyLockScripts.ContainsValue(address))
            {
                var lockScript = Script.BuildLockScipt(address);
                MyLockScripts.Add(lockScript, address);
            }
        }

        /// <summary>
        /// 获取指定数量的未花费交易
        /// </summary>
        /// <param name="count"></param>
        public List<TransactionMsg> GetTxsWithoutRepeatCost(int count, long maxSize, int maxInputCount = 20)
        {
            var txs = MainPool.OrderByDescending(x => x.FeeRate).ToArray();

            BlockMsg blockMsg = new BlockMsg();

            List<TransactionMsg> result = new List<TransactionMsg>();
            List<string> utxos = new List<string>();
            int num = 0;
            int index = 0;
            int size = 0;

            List<TransactionPoolItem> errorTxMsgs = new List<TransactionPoolItem>();

            while (num < count && size < maxSize)
            {
                if (txs.Length <= index)
                    break;

                var tx = txs[index];

                var hashIndexs = tx.Transaction.Inputs.Select(input => input.OutputTransactionHash + input.OutputIndex);
                //拿到可用的Utxo 数量
                int unspentCount = 0;
                
                unspentCount = InputDac.Default.SelectUnSpentCount(hashIndexs);

                if (unspentCount < hashIndexs.Count())
                {
                    index++;
                    continue;
                }

                //判断确认次数
                var localHeight = GlobalParameters.LocalHeight;
                List<BlockConstraint> constraints = new List<BlockConstraint>();

                var hashes = tx.Transaction.Inputs.Select(input => input.OutputTransactionHash);
                constraints = BlockDac.Default.GetConstraintsByBlockHashs(hashes);

                if (!constraints.Any())
                {
                    errorTxMsgs.Add(tx);
                    index++;
                    continue;
                }

                //Coinbase 确认次数小于100
                if (constraints.Any(x => x.IsCoinBase && localHeight - x.Height < 100))
                {
                    index++;
                    continue;
                }

                //非 Coinbase 确认次数小于7
                if (constraints.Any(x => !x.IsCoinBase && localHeight - x.Height < 7))
                {
                    index++;
                    continue;
                }

                if (!result.Any())
                {
                    if (size + tx.Transaction.Size >= maxSize)
                        break;
                    result.Add(tx.Transaction);

                    utxos.AddRange(hashIndexs);
                    num++;
                    if (utxos.Count >= maxInputCount)
                        break;
                }
                else
                {
                    if (utxos.Any(p => hashIndexs.Contains(p))) //已经被花费
                    {
                        continue;
                    }
                    else
                    {
                        if (size + tx.Transaction.Size >= maxSize)
                            break;
                        result.Add(tx.Transaction);
                        utxos.AddRange(hashIndexs);
                        num++;
                        if (utxos.Count >= maxInputCount)
                            break;
                    }
                }

                index++;
            }
            //矿池需要删除掉无效的交易
            if (errorTxMsgs.Any())
            {
                var errorHashes = errorTxMsgs.Select(x => x.Transaction.Hash);
                BlacklistTxs.Current.Add(errorHashes);
                foreach (var errorMsg in errorTxMsgs)
                {
                    BlacklistTxs.Current.AddToBlackFile(errorMsg.Transaction);
                    this.RemoveTransaction(errorMsg.Transaction.Hash);
                }
            }

            return result;
        }
    }
}