// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.Data;
using FiiiChain.Data.Accesses;
using FiiiChain.DataAgent;
using FiiiChain.Entities;
using FiiiChain.Entities.CacheModel;
using FiiiChain.Framework;
using FiiiChain.IModules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace FiiiChain.Business
{
    public class Ready
    {
        public static void ReadyCacheData(List<Account> accounts)
        {
            AccountDac.InsertAccountEvent = TransactionPool.Instance.AddAccount;

            UserSettingComponent component = new UserSettingComponent();
            var defaultAddr = component.GetDefaultAccount();
            CacheAccess.Default.Init(GlobalParameters.CACHE_FILE, defaultAddr);

            #region 预留

            //if (CacheAccess.Default.Init(GlobalParameters.CACHE_FILE, defaultAddr))
            //{
            //    //直接进行后续操作，留一个线程进行 数据校验和修复
            //}
            //else
            //{
            //    //需要在初始化之后再进行后续操作
            //}

            #endregion

            var accountTask = LoadAccount(accounts);
            var addressTask = LoadAddressBook();
            accountTask.Start();
            addressTask.Start();

            var totalAmount = OutputDac.Default.SumSelfUnspentOutputs();
            GlobalParameters.TotalAmount = totalAmount;

            Task.WaitAll(accountTask, addressTask);

            Task.Run(() =>
            {
                var outputTask = LoadOutputs();
                outputTask.Start();
                Task.WaitAll(outputTask);

                var txPoolPaymentTask = LoadTxPoolPayments();
                txPoolPaymentTask.Start();
                txPoolPaymentTask.ContinueWith(x => BackupOutput().Start());
                Task.WaitAll(txPoolPaymentTask);

                var paymentTask = LoadPayments();
                paymentTask.Start();
            });
            MiningPoolComponent.LoadMiningPools();
        }

        private static Task LoadAddressBook()
        {
            var result = new Task(() =>
            {
                AddressBookComponent addressBookComponent = new AddressBookComponent();
                var addressBooks = addressBookComponent.GetWholeAddressBook();

                var keyValues = addressBooks.Select(book => new KeyValuePair<string, AddressBookItem>(book.Address, book));
                CacheManager.Default.Put(DataCatelog.AddressBook, keyValues);
                addressBooks.Clear();
                keyValues = null;
            });
            return result;
        }

        private static Task LoadAccount(List<Account> accounts)
        {
            var result = new Task(() =>
            {
                var keyValues = accounts.Select(x => new KeyValuePair<string, Account>(x.Id, x));
                CacheManager.Default.Put(DataCatelog.Accounts, keyValues);
                accounts.Clear();
                keyValues = null;
            });
            return result;
        }

        private static Task LoadPayments()
        {
            var result = new Task(() =>
            {
                var inputDac = InputDac.Default;
                var outputDac = OutputDac.Default;
                var txDac = TransactionDac.Default;
                var start = 0;
                const int takeCount = 40;
                var isEnd = false;

                while (!isEnd)
                {
                    var items = txDac.SelectTransactions("*", start, takeCount, true);

                    foreach (var item in items)
                    {
                        item.Inputs = inputDac.SelectByTransactionHash(item.Hash);
                        item.Outputs = outputDac.SelectByTransactionHash(item.Hash);
                        var ps = Converters.ConvertToSelfPayment(item);
                        if (ps != null)
                        {
                            var keyValues = ps.Select(p => new KeyValuePair<string, PaymentCache>(p.ToString(), p));
                            CacheManager.Default.Put(DataCatelog.Payment, keyValues);
                            ps.Clear();

                            Thread.Sleep(1000);
                        }

                        TransactionPool.Remove(item.Hash);
                    }
                    start += takeCount;
                    if (items.Count < takeCount)
                        isEnd = true;
                }
            });
            return result;
        }

        private static Task LoadTxPoolPayments()
        {
            var result = new Task(() =>
            {
                TransactionPool.Instance.Load();

                var txComponent = new TransactionComponent();

                foreach (TransactionPoolItem item in TransactionPool.Instance.MainPool)
                {
                    if (txComponent.CheckTxExisted(item.Transaction.Hash, false) || BlacklistTxs.Current.IsBlacked(item.Transaction.Hash))
                    {
                        BlacklistTxs.Current.AddToBlackFile(item.Transaction);
                        TransactionPool.Instance.RemoveTransaction(item.Transaction.Hash);
                    }
                }

                foreach (TransactionPoolItem item in TransactionPool.Instance.IsolateTransactionPool)
                {
                    if (txComponent.CheckTxExisted(item.Transaction.Hash, false) || BlacklistTxs.Current.IsBlacked(item.Transaction.Hash))
                    {
                        BlacklistTxs.Current.AddToBlackFile(item.Transaction);
                        TransactionPool.Instance.RemoveTransaction(item.Transaction.Hash);
                    }
                }

                TransactionPool.Instance.ClearCostUtxo();

                TransactionPool.Instance.Init();
            });
            return result;
        }

        private static Task LoadOutputs()
        {
            Task result = new Task(() =>
            {
                OutputDac outputDac = OutputDac.Default;

                var idStr = CacheManager.Default.Get<string>(DataCatelog.Default, "MaxOutputId");
                if (string.IsNullOrEmpty(idStr))
                {
                    var outputs = outputDac.SelectSelfAll();
                    if (outputs == null || !outputs.Any())
                    {
                        CacheManager.Default.Put(DataCatelog.Default, "MaxOutputId", "0");
                    }
                    else
                    {
                        var keyValues = outputs.Select(x => new KeyValuePair<string, Output>(x.ToString(), x));
                        CacheManager.Default.Put(DataCatelog.Output, keyValues);
                        CacheManager.Default.Put(DataCatelog.Default, "MaxOutputId", outputs.Max(x => x.Id).ToString());
                        outputs.Clear();
                        keyValues = null;
                    }
                }
                else
                {
                    List<Output> outputs = null;
                    try
                    {
                        outputs = outputDac.SelectSelfAll(long.Parse(idStr.Replace("\"", "")));
                    }
                    catch
                    {
                        outputs = outputDac.SelectSelfAll();
                    }

                    if (outputs != null && outputs.Any())
                    {
                        var index = 0;
                        bool isEnd = false;
                        while (!isEnd)
                        {
                            var items = outputs.Skip(index).Take(5000);
                            if (items.Count() < 1000)
                                isEnd = true;

                            var keyValues = items.Select(x => new KeyValuePair<string, Output>(x.ToString(), x));
                            CacheManager.Default.Put(DataCatelog.Output, keyValues);
                            Thread.Sleep(1000);
                        }

                        CacheManager.Default.Put(DataCatelog.Default, "MaxOutputId", outputs.Max(x => x.Id).ToString());
                        outputs.Clear();
                    }
                }
            });
            return result;
        }

        private static Task BackupOutput()
        {
            var result = new Task(() =>
            {
                OutputDac outputDac = OutputDac.Default;
                var outputIds = outputDac.SelectDropedTxOutputIds();

                if (outputIds != null && outputIds.Any())
                {
                    var inputs = TransactionPool.Instance.MainPool.SelectMany(x => x.Transaction.Inputs);
                    var inputKeys = inputs.Select(x => x.OutputTransactionHash + x.OutputIndex);
                    var unSpentKeys = outputIds.TakeWhile(x => !inputKeys.Contains(x.Value));
                    var spentKeys = unSpentKeys as KeyValuePair<long, string>[] ?? unSpentKeys.ToArray();
                    if (spentKeys.Any())
                    {
                        var unSpentIds = spentKeys.Select(x => x.Key);
                        TaskQueue.AddWaitAction(() => { outputDac.UpdateOutputSpentState(unSpentIds); }, "BackupOutput");
                    }
                }

                //已经消费的数据，如果还是标志着未花费，需要更改花费的状态
                var spentItems = outputDac.SelectSpentedTxNotInOutputIds();
                if (spentItems != null && spentItems.Any())
                {
                    var spentIds = spentItems.Select(x => x.Key);
                    outputDac.UpdateOutputSpentState(spentIds, 1);

                    var updateMap = new Dictionary<string, Output>();
                    foreach (var item in spentItems)
                    {
                        var output = CacheManager.Default.Get<Output>(DataCatelog.Output, item.Value);
                        if (output != null)
                        {
                            output.Spent = true;
                            updateMap.Add(item.Value, output);
                        }
                    }
                    if (updateMap.Count > 0)
                        CacheManager.Default.Put(DataCatelog.Output, updateMap);
                }
            });
            return result;
        }
    }
}