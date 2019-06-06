// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FiiiChain.DataAgent;
using FiiiChain.Messages;
using FiiiChain.Consensus;
using FiiiChain.Framework;
using FiiiChain.Entities;
using FiiiChain.IModules;

namespace FiiiChain.Business
{
    public class UtxoComponent
    {
        public void Initialize()
        {
            var accountDac = AccountDac.Default;
            var accountIds = new AccountComponent().GetAllAccounts().Select(a => a.Id).ToArray();

            UtxoSet.Initialize(accountIds);

            foreach (var id in accountIds)
            {
                this.RefreshUtxoSet(id);
            }
        }

        public void AddMonitoredAccountId(string accountId)
        {
            if (UtxoSet.Instance != null)
            {
                UtxoSet.Instance.AddAccountId(accountId);
            }
        }

        public void RemoveAccountId(string accountId)
        {
            if (UtxoSet.Instance != null)
            {
                UtxoSet.Instance.RemoveAccountId(accountId);
            }
        }

        public void AddNewUtxoToSet(UtxoMsg utxo)
        {
            if (UtxoSet.Instance != null)
            {
                UtxoSet.Instance.AddUtxoRecord(utxo);
            }
        }

        public void RemoveUtxoFromSet(string transactionHash, int outputIndex)
        {
            if (UtxoSet.Instance != null)
            {
                UtxoSet.Instance.RemoveUtxoRecord(transactionHash, outputIndex);
            }
        }

        public void RefreshUtxoSet(string accountId)
        {
            var outputDac = OutputDac.Default;
            var accountDac = AccountDac.Default;
            var txDac = TransactionDac.Default;

            var account = accountDac.SelectById(accountId);

            if (account != null && UtxoSet.Instance != null)
            {
                var set = UtxoSet.Instance.GetUtxoSetByAccountId(accountId);

                if (set != null)
                {
                    set.Clear();

                    //load from database
                    var outputsInDB = outputDac.SelectUnspentByReceiverId(accountId);

                    foreach (var output in outputsInDB)
                    {
                        var msg = new UtxoMsg();
                        msg.AccountId = output.ReceiverId;
                        msg.TransactionHash = output.TransactionHash;
                        msg.OutputIndex = output.Index;
                        msg.Amount = output.Amount;
                        msg.IsConfirmed = true;
                        msg.IsWatchedOnly = account.WatchedOnly;

                        var txEntity = txDac.SelectByHash(output.TransactionHash);
                        msg.BlockHash = txEntity != null ? txEntity.BlockHash : null;

                        set.Add(msg);
                    }

                    var outputsInTxPool = TransactionPool.Instance.GetTransactionOutputsByAccountId(accountId);

                    foreach (var txHash in outputsInTxPool.Keys)
                    {
                        foreach (var output in outputsInTxPool[txHash])
                        {
                            var msg = new UtxoMsg();
                            msg.AccountId = accountId;
                            msg.TransactionHash = txHash;
                            msg.OutputIndex = output.Index;
                            msg.Amount = output.Amount;
                            msg.IsConfirmed = false;
                            msg.IsWatchedOnly = account.WatchedOnly;

                            set.Add(msg);
                        }
                    }

                }
            }
        }

        private void RefreshWholeUtxoSet()
        {
            var accountIds = UtxoSet.Instance.GetAllAccountIds();

            foreach (var accountId in accountIds)
            {
                this.RefreshUtxoSet(accountId);
            }
        }

        public long GetConfirmedBlanace(string accountId)
        {
            this.RefreshUtxoSet(accountId);
            return UtxoSet.Instance.GetAccountBlanace(accountId, true);
        }

        public long GetUnconfirmedBalance(string accountId)
        {
            this.RefreshUtxoSet(accountId);
            return UtxoSet.Instance.GetAccountBlanace(accountId, false);
        }

        public long GetOutputCounts()
        {
            return OutputDac.Default.CountSelfUnspentOutputs();
        }

        public long GetTransactionCounts()
        {
            return TransactionDac.Default.CountSelfUnspentTransactions();
        }

        public void GetBalanceInDB(out long confirmedBalance, out long unconfirmedBalance)
        {
            //return OutputDac.Default.SumSelfUnspentOutputs();
            confirmedBalance = 0;
            unconfirmedBalance = 0;
            var outputDac = OutputDac.Default;
            var txDac = TransactionDac.Default;
            var blockDac = BlockDac.Default;

            var lastHeight = -1L;
            var lastBlock = blockDac.SelectLast();

            if (lastBlock != null)
            {
                lastHeight = lastBlock.Height;
            }

            List<BalanceHelper> entities = outputDac.SelectAllBalanceHelper();
            confirmedBalance = entities.Where(p =>
                                                (!p.IsDiscarded && p.TotalInput == 0 && (lastHeight - p.Height) >= 100) ||
                                               (!p.IsDiscarded && p.TotalInput != 0 && p.IsVerified && p.LockTime <= Time.EpochTime)).Sum(p => p.Amount);

            unconfirmedBalance = entities.Sum(p => p.Amount) - confirmedBalance;
        }

        public long GetTotalBalance()
        {
            var outputDac = OutputDac.Default;
            var txDac = TransactionDac.Default;
            var blockDac = BlockDac.Default;

            var lastHeight = -1L;
            var lastBlock = blockDac.SelectLast();

            if (lastBlock != null)
            {
                lastHeight = lastBlock.Height;
            }
            
            List<BalanceHelper> entities = outputDac.SelectAllBalanceHelper();

            return entities.Where(p=>!p.IsDiscarded).Sum(p => p.Amount);
        }

        //public List<UtxoMsg> GetAllConfirmedOutputs()
        //{
        //    return UtxoSet.Instance.GetAllUnspentOutputs();
        //}


        public List<UtxoMsg> GetAllConfirmedOutputs(long start, long limit)
        {
            List<UtxoMsg> utxoMsgs = new List<UtxoMsg>();

            var outputDac = OutputDac.Default;
            var txDac = TransactionDac.Default;
            var blockDac = BlockDac.Default;
            var accountDac = AccountDac.Default;

            var lastHeight = -1L;
            var lastBlock = blockDac.SelectLast();

            if (lastBlock != null)
            {
                lastHeight = lastBlock.Height;
            }
            var outputs = outputDac.SelectAllUnspentOutputs(start, limit);

            foreach (var output in outputs)
            {
                var msg = new UtxoMsg();
                msg.AccountId = output.ReceiverId;
                msg.TransactionHash = output.TransactionHash;
                msg.OutputIndex = output.Index;
                msg.Amount = output.Amount;
                msg.IsConfirmed = true;
                var account = accountDac.SelectById(msg.AccountId);
                msg.IsWatchedOnly = account.WatchedOnly;

                //var txEntity = txDac.SelectByHash(output.TransactionHash);
                msg.BlockHash = output.BlockHash; //txEntity != null ? txEntity.BlockHash : null;
                utxoMsgs.Add(msg);
            }

            return utxoMsgs;
        }

        //result is List<txid + "," + vout>
        public long GetAllUnconfirmedBalance()
        {
            var accountDac = AccountDac.Default;
            long totalAmount = 0;

            var accounts = CacheManager.Default.Get<Account>(DataCatelog.Accounts).Select(x => x.Id);

            var items = TransactionPool.Instance.MainPool.ToArray();

            var outputs= items.SelectMany(x => x.Transaction.Outputs);
            var goutputs = outputs.GroupBy(x => x.LockScript);
            var utxos = goutputs.Where(x => accounts.Contains(AccountIdHelper.CreateAccountAddressByPublicKeyHash(
                       Base16.Decode(Script.GetPublicKeyHashFromLockScript(x.Key))))).SelectMany(x=>x.ToArray());

            totalAmount += utxos.Sum(x => x.Amount);

            return totalAmount;
        }

        public List<Output> SelectAllUnspentOutputs()
        {
            return OutputDac.Default.SelectAllUnspentOutputs();
        }
    }
}