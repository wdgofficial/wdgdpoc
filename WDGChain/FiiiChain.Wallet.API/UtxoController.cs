// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.
using EdjCase.JsonRpc.Router.Abstractions;
using FiiiChain.Business;
using FiiiChain.Consensus;
using FiiiChain.DataAgent;
using FiiiChain.DTO;
using FiiiChain.DTO.Utxo;
using FiiiChain.Entities;
using FiiiChain.Framework;
using FiiiChain.Messages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiiiChain.Wallet.API
{
    public class UtxoController : BaseRpcController
    {
        public IRpcMethodResult GetTxOut(string txid, int vount, bool unconfirmed = false)
        {
            try
            {
                GetTxOutOM result = null;

                var txComponent = new TransactionComponent();
                var blockComponent = new BlockComponent();
                string blockHash = null;
                TransactionMsg tx = txComponent.GetTransactionMsgFromDB(txid, out blockHash);

                if (tx == null && unconfirmed)
                {
                    tx = txComponent.GetTransactionMsgFromPool(txid);
                }


                if (tx != null && tx.OutputCount > vount)
                {
                    var output = tx.Outputs[vount];
                    long confirmations = 0;

                    if (!string.IsNullOrWhiteSpace(blockHash))
                    {
                        var block = blockComponent.GetBlockEntiytByHash(blockHash);

                        if (block != null)
                        {
                            var latestHeight = blockComponent.GetLatestHeight();

                            if (latestHeight > block.Height)
                            {
                                confirmations = latestHeight - block.Height;
                            }
                        }
                    }

                    result = new GetTxOutOM();
                    result.bestblock = blockHash;
                    result.confirmations = confirmations;
                    result.value = output.Amount;
                    result.scriptPubKey = output.LockScript;
                    result.version = tx.Version;
                    result.coinbase = (tx.InputCount == 1 && tx.Inputs[0].OutputTransactionHash == Base16.Encode(HashHelper.EmptyHash()));
                }

                return Ok(result);
            }
            catch (CommonException ce)
            {
                return Error(ce.ErrorCode, ce.Message, ce);
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.UNKNOWN_ERROR, ex.Message, ex);
            }
        }

        public IRpcMethodResult GetTxOutSetInfo()
        {
            try
            {
                GetTxOutSetInfoOM result = new GetTxOutSetInfoOM();
                var blockComponent = new BlockComponent();
                var utxoComponent = new UtxoComponent();

                result.height = blockComponent.GetLatestHeight();
                result.bestblock = null;

                if (result.height >= 0)
                {
                    var bestBlock = blockComponent.GetBlockEntiytByHeight(result.height);
                    result.bestblock = bestBlock != null ? bestBlock.Hash : null;
                }

                result.transactions = utxoComponent.GetTransactionCounts();
                result.txouts = utxoComponent.GetOutputCounts();

                long confirmedBalance, unconfirmedBalance;
                utxoComponent.GetBalanceInDB(out confirmedBalance, out unconfirmedBalance);
                result.total_amount = confirmedBalance;
                return Ok(result);
            }
            catch (CommonException ce)
            {
                return Error(ce.ErrorCode, ce.Message, ce);
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.UNKNOWN_ERROR, ex.Message, ex);
            }
        }

        /// <summary>
        /// CoinEgg专用，获取数据库中为未费的所有总额
        /// </summary>
        /// <returns></returns>
        public IRpcMethodResult GetTotalBalance()
        {
            try
            {
                GetTxOutSetInfoOM result = new GetTxOutSetInfoOM();
                var blockComponent = new BlockComponent();
                var utxoComponent = new UtxoComponent();

                result.height = blockComponent.GetLatestHeight();
                result.bestblock = null;

                if (result.height >= 0)
                {
                    var bestBlock = blockComponent.GetBlockEntiytByHeight(result.height);
                    result.bestblock = bestBlock != null ? bestBlock.Hash : null;
                }

                result.transactions = utxoComponent.GetTransactionCounts();
                result.txouts = utxoComponent.GetOutputCounts();

                result.total_amount = GlobalParameters.TotalAmount;
                return Ok(result);
            }
            catch (CommonException ce)
            {
                return Error(ce.ErrorCode, ce.Message, ce);
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.UNKNOWN_ERROR, ex.Message, ex);
            }
        }

        public IRpcMethodResult ListUnspent(int minConfirmations, int maxConfirmations = 9999999, string[] addresses = null)
        {
            try
            {
                var result = new List<ListUnspentOM>();
                var txComponent = new TransactionComponent();
                var blockComponent = new BlockComponent();
                var outputComponent = new UtxoComponent();
                var accountComponent = new AccountComponent();
                var addressBookComponent = new AddressBookComponent();

                var accounts = accountComponent.GetAllAccounts();
                //List<Transaction> transactions = txComponent.GetTransactionEntitiesContainUnspentUTXO();
                List<Output> outputs = txComponent.GetOutputEntitesContainUnspentUTXOByTxHash(Time.EpochTime);
                var latestHeight = blockComponent.GetLatestHeight();

                List<ListLockUnspentOM> lockList = Startup.lockUnspentList;

                foreach (var output in outputs)
                {
                    //排除交易池中的
                    TransactionMsg spentUtxo = TransactionPool.Instance.GetTransactionByInputTx(output.TransactionHash, output.Index);
                    if (spentUtxo != null)
                    {
                        continue;
                    }
                    //获取output的时候已经过滤了locktime
                    var blockEntity = blockComponent.GetBlockEntiytByHash(output.BlockHash);

                    //if(blockMsg != null)
                    if (blockEntity != null)
                    {
                        var confirmations = latestHeight - blockEntity.Height + 1;
                        if (confirmations >= minConfirmations && confirmations <= maxConfirmations)
                        {
                            bool isLocked = lockList.Any(x => output.TransactionHash == x.txid && output.Index == x.vout);
                            if (isLocked)
                            {
                                continue;
                            }

                            var pubKeyHash = Script.GetPublicKeyHashFromLockScript(output.LockScript);
                            var address = AccountIdHelper.CreateAccountAddressByPublicKeyHash(Base16.Decode(pubKeyHash));

                            var account = accounts.FirstOrDefault(a => a.Id == address);
                            if (account != null)
                            {
                                if (addresses == null || addresses.Contains(address))
                                {
                                    result.Add(new ListUnspentOM
                                    {
                                        txid = output.TransactionHash,
                                        vout = output.Index,
                                        address = address,
                                        account = addressBookComponent.GetTagByAddress(address),
                                        scriptPubKey = output.LockScript,
                                        redeemScript = null,
                                        amount = output.Amount,
                                        confirmations = confirmations,
                                        spendable = string.IsNullOrWhiteSpace(account.PrivateKey),
                                        solvable = false
                                    });
                                }
                            }
                        }
                    }
                }

                return Ok(result);
            }
            catch (CommonException ce)
            {
                return Error(ce.ErrorCode, ce.Message, ce);
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.UNKNOWN_ERROR, ex.Message, ex);
            }
        }

        /// <summary>
        /// 分页获取未花费的输出
        /// </summary>
        /// <param name="minConfirmations">最小确认数</param>
        /// <param name="currentPage">当前页数</param>
        /// <param name="pageSize">页面记录数</param>
        /// <param name="maxConfirmations">最大确认数</param>
        /// <param name="minAmount">最小金额</param>
        /// <param name="maxAmount">最大金额</param>
        /// <param name="isDesc">是否逆序</param>
        /// <returns></returns>
        public IRpcMethodResult ListPageUnspent(long minConfirmations, int currentPage, int pageSize, long maxConfirmations = 9999999, long minAmount = 1, long maxAmount = long.MaxValue, bool isDesc = false)
        {
            try
            {
                List<ListUnspentOM> unspentOMList = new List<ListUnspentOM>();
                var txComponent = new TransactionComponent();
                var blockComponent = new BlockComponent();
                var outputComponent = new UtxoComponent();
                var accountComponent = new AccountComponent();
                var addressBookComponent = new AddressBookComponent();

                var accounts = accountComponent.GetAllAccounts();
                var latestHeight = blockComponent.GetLatestHeight();
                PageUnspent pageUnspent = txComponent.GetOutputEntitesContainUnspentUTXOByTxHash(minAmount, maxAmount, currentPage, pageSize, isDesc, Time.EpochTime, minConfirmations, maxConfirmations, latestHeight);
                List<ListLockUnspentOM> lockList = Startup.lockUnspentList;

                foreach (var output in pageUnspent.Outputs)
                {
                    //排除被锁定的
                    var isLocked = lockList.Any(item => output.TransactionHash == item.txid && output.Index == item.vout);
                    if (isLocked)
                    {
                        continue;
                    }
                    //排除交易池中已经花费的
                    TransactionMsg spentUtxo = TransactionPool.Instance.GetTransactionByInputTx(output.TransactionHash, output.Index);
                    if (spentUtxo != null)
                    {
                        continue;
                    }
                    var pubKeyHash = Script.GetPublicKeyHashFromLockScript(output.LockScript);
                    var address = AccountIdHelper.CreateAccountAddressByPublicKeyHash(Base16.Decode(pubKeyHash));

                    var account = accounts.FirstOrDefault(a => a.Id == address);
                    if (account != null)
                    {
                        unspentOMList.Add(new ListUnspentOM
                        {
                            txid = output.TransactionHash,
                            vout = output.Index,
                            address = address,
                            account = addressBookComponent.GetTagByAddress(address),
                            scriptPubKey = output.LockScript,
                            redeemScript = null,
                            amount = output.Amount,
                            confirmations = output.Confirmations,
                            spendable = output.Spent,//能否花费
                            solvable = false
                        });
                    }
                }

                return Ok(new ListPageUnspentOM { UnspentOMList = unspentOMList, Count = pageUnspent.Count });
            }
            catch (CommonException ce)
            {
                return Error(ce.ErrorCode, ce.Message, ce);
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.UNKNOWN_ERROR, ex.Message, ex);
            }
        }

        public IRpcMethodResult ListPageUnspentNew(int currentPage, int pageSize)
        {
            try
            {
                List<ListUnspentOM> unspentOMList = new List<ListUnspentOM>();
                var txComponent = new TransactionComponent();
                var blockComponent = new BlockComponent();
                var outputComponent = new UtxoComponent();
                var accountComponent = new AccountComponent();
                var addressBookComponent = new AddressBookComponent();

                var accounts = accountComponent.GetAllAccounts();
                var latestHeight = blockComponent.GetLatestHeight();
                PageUnspent pageUnspent = txComponent.GetOutputEntitesContainUnspentUTXOByTxHash(currentPage, pageSize, Time.EpochTime, latestHeight);
                List<ListLockUnspentOM> lockList = Startup.lockUnspentList;

                foreach (var output in pageUnspent.Outputs)
                {
                    //排除被锁定的
                    var isLocked = lockList.Any(item => output.TransactionHash == item.txid && output.Index == item.vout);
                    if (isLocked)
                    {
                        continue;
                    }
                    //排除交易池中已经花费的
                    TransactionMsg spentUtxo = TransactionPool.Instance.GetTransactionByInputTx(output.TransactionHash, output.Index);
                    if (spentUtxo != null)
                    {
                        continue;
                    }
                    var pubKeyHash = Script.GetPublicKeyHashFromLockScript(output.LockScript);
                    var address = AccountIdHelper.CreateAccountAddressByPublicKeyHash(Base16.Decode(pubKeyHash));

                    var account = accounts.FirstOrDefault(a => a.Id == address);
                    if (account != null)
                    {
                        unspentOMList.Add(new ListUnspentOM
                        {
                            txid = output.TransactionHash,
                            vout = output.Index,
                            address = address,
                            account = addressBookComponent.GetTagByAddress(address),
                            scriptPubKey = output.LockScript,
                            redeemScript = null,
                            amount = output.Amount,
                            confirmations = output.Confirmations,
                            spendable = output.Spent,//能否花费
                            solvable = false
                        });
                    }
                }

                return Ok(new ListPageUnspentOM { UnspentOMList = unspentOMList, Count = pageUnspent.Count });
            }
            catch (CommonException ce)
            {
                return Error(ce.ErrorCode, ce.Message, ce);
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.UNKNOWN_ERROR, ex.Message, ex);
            }
        }

        /// <summary>
        /// 获取指定金额以下的UTXO（UTXO拆分合并专用）
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public IRpcMethodResult ListPageUnspentByAmount(int currentPage, int pageSize, long maxAmount, long minAmount = 0)
        {
            try
            {
                List<ListUnspentOM> unspentOMList = new List<ListUnspentOM>();
                var txComponent = new TransactionComponent();
                var blockComponent = new BlockComponent();
                var outputComponent = new UtxoComponent();
                var accountComponent = new AccountComponent();
                var addressBookComponent = new AddressBookComponent();

                var accounts = accountComponent.GetAllAccounts();
                var latestHeight = blockComponent.GetLatestHeight();
                PageUnspent pageUnspent = txComponent.GetOutputEntitesContainUnspentUTXOByTxHash(currentPage, pageSize, Time.EpochTime, latestHeight, maxAmount, minAmount);
                List<ListLockUnspentOM> lockList = Startup.lockUnspentList;

                foreach (var output in pageUnspent.Outputs)
                {
                    //排除被锁定的
                    var isLocked = lockList.Any(item => output.TransactionHash == item.txid && output.Index == item.vout);
                    if (isLocked)
                    {
                        continue;
                    }
                    //排除交易池中已经花费的
                    TransactionMsg spentUtxo = TransactionPool.Instance.GetTransactionByInputTx(output.TransactionHash, output.Index);
                    if (spentUtxo != null)
                    {
                        continue;
                    }
                    var pubKeyHash = Script.GetPublicKeyHashFromLockScript(output.LockScript);
                    var address = AccountIdHelper.CreateAccountAddressByPublicKeyHash(Base16.Decode(pubKeyHash));

                    var account = accounts.FirstOrDefault(a => a.Id == address);
                    if (account != null)
                    {
                        unspentOMList.Add(new ListUnspentOM
                        {
                            txid = output.TransactionHash,
                            vout = output.Index,
                            address = address,
                            account = addressBookComponent.GetTagByAddress(address),
                            scriptPubKey = output.LockScript,
                            redeemScript = null,
                            amount = output.Amount,
                            confirmations = output.Confirmations,
                            spendable = output.Spent,//能否花费
                            solvable = false
                        });
                    }
                }

                return Ok(new ListPageUnspentOM { UnspentOMList = unspentOMList, Count = pageUnspent.Count });
            }
            catch (CommonException ce)
            {
                return Error(ce.ErrorCode, ce.Message, ce);
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.UNKNOWN_ERROR, ex.Message, ex);
            }
        }

        public IRpcMethodResult GetUnconfirmedBalance()
        {
            try
            {
                var utxoComponent = new UtxoComponent();
                long result = utxoComponent.GetAllUnconfirmedBalance();

                long confirmedBalance, unconfirmedBalance;
                utxoComponent.GetBalanceInDB(out confirmedBalance, out unconfirmedBalance);
                result += unconfirmedBalance;

                return Ok(result);
            }
            catch (CommonException ce)
            {
                return Error(ce.ErrorCode, ce.Message, ce);
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.UNKNOWN_ERROR, ex.Message, ex);
            }
        }

        /// <summary>
        /// 锁定未花费的输出
        /// </summary>
        /// <param name="isLocked">true为已锁定，false为未锁定</param>
        /// <param name="transaction">需要锁定的交易</param>
        /// <returns></returns>
        public IRpcMethodResult LockUnspent(bool isLocked, ListLockUnspentOM[] transaction)
        {
            try
            {
                /* 1、把输入的参数用个静态变量存储起来
                 * 2、当转账或其他交易的时候先判断是否在静态变量中存在，如果存在就跳过，
                 * 3、注意修改对应的转账接口
                 */
                //根据Transaction的txid和vout获取outputList的ReceivedId
                TransactionComponent component = new TransactionComponent();
                AccountComponent account = new AccountComponent();
                List<Account> accounts = account.GetAllAccounts();
                List<string> accountIdList = accounts.Select(q => q.Id).ToList();
                foreach (var item in transaction)
                {
                    string receivedId = component.GetOutputEntiyByIndexAndTxHash(item.txid, item.vout)?.ReceiverId;
                    //只锁定自己的账户
                    if (accountIdList.Contains(receivedId))
                    {
                        if (isLocked)
                        {
                            Startup.lockUnspentList.Add(item);
                        }
                        else
                        {
                            Startup.lockUnspentList.Remove(Startup.lockUnspentList.FirstOrDefault(p => p.txid == item.txid && p.vout == item.vout));
                        }
                    }
                }
                //去除重复数据
                Startup.lockUnspentList = Startup.lockUnspentList.GroupBy(p => new { p.txid, p.vout }).Select(q => q.First()).ToList();
                return Ok(isLocked);
            }
            catch (CommonException ce)
            {
                return Error(ce.ErrorCode, ce.Message, ce);
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.UNKNOWN_ERROR, ex.Message, ex);
            }
        }

        /// <summary>
        /// 列出锁定的未花费
        /// </summary>
        /// <returns>ListLockUnspentOM对象数组</returns>
        public IRpcMethodResult ListLockUnspent()
        {
            try
            {
                return Ok(Startup.lockUnspentList);
            }
            catch (CommonException ce)
            {
                return Error(ce.ErrorCode, ce.Message, ce);
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.UNKNOWN_ERROR, ex.Message, ex);
            }
        }

        public IRpcMethodResult SetAutoAddress(bool isEnable)
        {
            try
            {
                UserSettingComponent userSettingComponent = new UserSettingComponent();
                userSettingComponent.SetEnableAutoAccount(isEnable);
                return Ok();
            }
            catch (Exception ex)
            {
                return Error(ex.HResult, ex.Message, ex);
            }
        }
    }
}