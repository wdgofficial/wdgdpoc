// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.
using EdjCase.JsonRpc.Router.Abstractions;
using FiiiChain.Business;
using FiiiChain.Consensus;
using FiiiChain.Data;
using FiiiChain.DataAgent;
using FiiiChain.DTO;
using FiiiChain.DTO.Transaction;
using FiiiChain.Entities;
using FiiiChain.Entities.CacheModel;
using FiiiChain.Framework;
using FiiiChain.IModules;
using FiiiChain.Messages;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using static FiiiChain.Framework.ErrorCode;

namespace FiiiChain.Wallet.API
{
    public class TransactionController : BaseRpcController
    {
        private IMemoryCache _cache;

        public TransactionController(IMemoryCache memoryCache) { _cache = memoryCache; }

        public IRpcMethodResult SendToAddress(string toAddress, long amount, string comment, string tag, bool deductFeeFromAmount, string changeAddress = null)
        {
            try
            {
                string result = null;
                var utxoComponent = new UtxoComponent();
                var txComponent = new TransactionComponent();
                var settingComponent = new SettingComponent();
                var addressBookComponent = new AddressBookComponent();
                var accountComponent = new AccountComponent();
                var transactionCommentComponent = new TransactionCommentComponent();

                if (!AccountIdHelper.AddressVerify(toAddress))
                {
                    throw new CommonException(ErrorCode.Service.Transaction.TO_ADDRESS_INVALID);
                }

                var blockComponent = new BlockComponent();
                var lastBlockHeight = blockComponent.GetLatestHeight();
                long start = 0;
                long limit = 10;
                var setting = settingComponent.GetSetting();
                var utxos = utxoComponent.GetAllConfirmedOutputs(start, limit);
                var tx = new TransactionMsg();
                var totalSize = tx.Serialize().Length;

                var output = new OutputMsg();
                output.Amount = amount;
                output.Index = 0;
                output.LockScript = Script.BuildLockScipt(toAddress);
                output.Size = output.LockScript.Length;
                tx.Outputs.Add(output);
                totalSize += output.Serialize().Length;

                var totalInput = 0L;
                var index = 0;
                double totalAmount = amount;
                double totalFee = setting.FeePerKB * ((double)totalSize / 1024.0);

                while (index < utxos.Count)
                {
                    //排除被锁定的
                    bool isLocked = false;
                    List<ListLockUnspentOM> lockList = Startup.lockUnspentList;
                    foreach (var item in lockList)
                    {
                        Output outputLocked = txComponent.GetOutputEntiyByIndexAndTxHash(item.txid, item.vout);
                        if (outputLocked.ReceiverId == utxos[index].AccountId)
                        {
                            isLocked = true;
                            break;
                        }
                    }
                    if (isLocked)
                    {
                        getNextBatchUtxos(limit, ref start, ref index, ref utxos);
                        continue;
                    }

                    var account = accountComponent.GetAccountById(utxos[index].AccountId);

                    if (account != null && !string.IsNullOrWhiteSpace(account.PrivateKey))
                    {
                        var utxoTX = txComponent.GetTransactionMsgByHash(utxos[index].TransactionHash);
                        Block utxoBlock = blockComponent.GetBlockEntiytByHash(utxos[index].BlockHash);
                        //排除交易池中的交易
                        TransactionMsg transactionMsg = TransactionPool.Instance.GetTransactionByInputTx(utxos[index].TransactionHash, utxos[index].OutputIndex);
                        if (transactionMsg != null)
                        {
                            getNextBatchUtxos(limit, ref start, ref index, ref utxos);
                            continue;
                        }
                        if (utxoTX == null || utxoBlock == null)
                        {
                            getNextBatchUtxos(limit, ref start, ref index, ref utxos);
                            continue;
                        }

                        if (!utxoBlock.IsVerified)
                        {
                            getNextBatchUtxos(limit, ref start, ref index, ref utxos);
                            continue;
                        }

                        if (Time.EpochTime < utxoTX.Locktime)
                        {
                            getNextBatchUtxos(limit, ref start, ref index, ref utxos);
                            continue;
                        }

                        if (utxoTX.InputCount == 1 && utxoTX.Inputs[0].OutputTransactionHash == Base16.Encode(HashHelper.EmptyHash()))
                        {
                            var blockHeight = utxoBlock.Height;

                            if (lastBlockHeight - blockHeight < 100L)
                            {
                                getNextBatchUtxos(limit, ref start, ref index, ref utxos);
                                continue;
                            }
                        }

                        var input = new InputMsg();
                        input.OutputTransactionHash = utxos[index].TransactionHash;
                        input.OutputIndex = utxos[index].OutputIndex;
                        input.UnlockScript = Script.BuildUnlockScript(input.OutputTransactionHash, input.OutputIndex, Base16.Decode(DecryptPrivateKey(account.PrivateKey)), Base16.Decode(account.PublicKey));
                        input.Size = input.UnlockScript.Length;
                        tx.Inputs.Add(input);

                        var size = input.Serialize().Length;
                        totalSize += size;
                        totalFee += setting.FeePerKB * ((double)size / 1024.0);
                        totalInput += utxos[index].Amount;
                    }
                    else
                    {
                        getNextBatchUtxos(limit, ref start, ref index, ref utxos);
                        continue;
                    }

                    if (!deductFeeFromAmount)
                    {
                        totalAmount = amount + totalFee;
                    }

                    if (totalInput >= (long)Math.Ceiling(totalAmount))
                    {
                        var size = output.Serialize().Length;

                        if ((totalInput - (long)Math.Ceiling(totalAmount)) > (setting.FeePerKB * (double)size / 1024.0))
                        {
                            totalSize += size;
                            totalFee += setting.FeePerKB * ((double)size / 1024.0);

                            if (!deductFeeFromAmount)
                            {
                                totalAmount = amount + totalFee;
                            }
                            //添加找零地址
                            Account newAccount = null;
                            if (string.IsNullOrEmpty(changeAddress))
                            {
                                newAccount = accountComponent.GenerateNewAccount();
                                if (setting.Encrypt)
                                {
                                    if (!string.IsNullOrWhiteSpace(_cache.Get<string>("WalletPassphrase")))
                                    {
                                        newAccount.PrivateKey = AES128.Encrypt(newAccount.PrivateKey, _cache.Get<string>("WalletPassphrase"));
                                        accountComponent.UpdatePrivateKeyAr(newAccount);
                                    }
                                    else
                                    {
                                        throw new CommonException(ErrorCode.Service.Wallet.WALLET_HAS_BEEN_LOCKED);
                                    }
                                }
                            }
                            else
                            {
                                newAccount = accountComponent.GetAccountById(changeAddress);
                                if (newAccount == null)
                                {
                                    throw new CommonException(ErrorCode.Engine.Transaction.Verify.CHANGE_ADDRESS_IS_INVALID);
                                }
                            }

                            var newOutput = new OutputMsg();
                            newOutput.Amount = totalInput - (long)Math.Ceiling(totalAmount);
                            newOutput.Index = 1;
                            newOutput.LockScript = Script.BuildLockScipt(newAccount.Id);
                            newOutput.Size = newOutput.LockScript.Length;
                            tx.Outputs.Add(newOutput);
                        }

                        break;
                    }
                    getNextBatchUtxos(limit, ref start, ref index, ref utxos);
                    //index++;
                    //if (index >= utxos.Count)
                    //{
                    //    start += 10;
                    //    utxos = utxoComponent.GetAllConfirmedOutputs(start, limit);

                    //    if (utxos.Count == 0)
                    //    {
                    //        break;
                    //    }
                    //    else
                    //    {
                    //        index = 0;
                    //    }
                    //}
                }
                if (totalInput < totalAmount)
                {
                    throw new CommonException(ErrorCode.Service.Transaction.BALANCE_NOT_ENOUGH);
                }

                if (deductFeeFromAmount)
                {
                    output.Amount -= (long)Math.Ceiling(totalFee);

                    if (output.Amount <= 0)
                    {
                        throw new CommonException(ErrorCode.Service.Transaction.SEND_AMOUNT_LESS_THAN_FEE);
                    }
                }

                tx.Timestamp = Time.EpochTime;
                tx.Hash = tx.GetHash();
                txComponent.UpdateTxInputToSpent(tx);
                txComponent.CreateNewTransaction(tx, setting.FeePerKB);
                Startup.P2PBroadcastTransactionAction(tx.Hash);
                result = tx.Hash;

                if (!string.IsNullOrWhiteSpace(tag))
                {
                    addressBookComponent.SetTag(toAddress, tag);
                    var bookitem = addressBookComponent.GetByAddress(toAddress);
                    if (bookitem != null)
                        CacheManager.Default.Put(DataCatelog.AddressBook, bookitem.Address, bookitem);
                }

                if (!string.IsNullOrWhiteSpace(comment))
                {
                    transactionCommentComponent.Add(tx.Hash, 0, comment);
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

        private EstimateTxFeeOM EstimateTxFee(string fromAccount, SendManyOutputIM[] receivers, string[] feeDeductAddresses,out TransactionMsg tx,out long feerate)
        {
            try
            {
                #region Verity
                if (receivers == null || receivers.Length == 0)
                {
                    throw new CommonException(ErrorCode.Service.Transaction.TO_ADDRESS_INVALID);
                }

                foreach (var address in feeDeductAddresses)
                {
                    if (receivers.Where(r => r.address == address).Count() == 0)
                    {
                        throw new CommonException(ErrorCode.Service.Transaction.FEE_DEDUCT_ADDRESS_INVALID);
                    }
                }
                #endregion

                EstimateTxFeeOM result = new EstimateTxFeeOM();

                tx = new TransactionMsg();
                double totalOutput = 0;
                var totalSize = tx.Serialize().Length;
                
                #region Generate Outputs
                foreach (var receiver in receivers)
                {
                    if (!AccountIdHelper.AddressVerify(receiver.address))
                    {
                        throw new CommonException(ErrorCode.Service.Transaction.TO_ADDRESS_INVALID);
                    }

                    var output = new OutputMsg();
                    output.Amount = receiver.amount;
                    output.Index = tx.Outputs.Count;
                    output.LockScript = Script.BuildLockScipt(receiver.address);
                    output.Size = output.LockScript.Length;
                    tx.Outputs.Add(output);

                    totalSize += output.Serialize().Length;
                    totalOutput += receiver.amount;
                }
                #endregion
                
                var utxoComponent = new UtxoComponent();
                var txComponent = new TransactionComponent();
                var settingComponent = new SettingComponent();
                var accountComponent = new AccountComponent();
                var blockComponent = new BlockComponent();

                var lastBlockHeight = blockComponent.GetLatestHeight();

                long start = 0;
                long limit = 10;
                var setting = settingComponent.GetSetting();
                var utxos = utxoComponent.GetAllConfirmedOutputs(start, limit);
                
                
                var totalInput = 0L;
                var index = 0;
                feerate = setting.FeePerKB;
                double totalFee = setting.FeePerKB * ((double)totalSize / 1024.0);
                double totalAmount = totalOutput;

                while (index < utxos.Count)
                {
                    //排除被锁定的
                    bool isLocked = false;
                    List<ListLockUnspentOM> lockList = Startup.lockUnspentList;
                    foreach (var item in lockList)
                    {
                        Output outputLocked = txComponent.GetOutputEntiyByIndexAndTxHash(item.txid, item.vout);
                        if (outputLocked.ReceiverId == utxos[index].AccountId)
                        {
                            isLocked = true;
                            break;
                        }
                    }
                    if (isLocked)
                    {
                        getNextBatchUtxos(limit, ref start, ref index, ref utxos);
                        continue;
                    }

                    var account = accountComponent.GetAccountById(utxos[index].AccountId);

                    if (account != null && !string.IsNullOrWhiteSpace(account.PrivateKey))
                    {
                        var utxoTX = txComponent.GetTransactionMsgByHash(utxos[index].TransactionHash);
                        Block utxoBlock = blockComponent.GetBlockEntiytByHash(utxos[index].BlockHash);
                        //排除交易池中的交易
                        TransactionMsg transactionMsg = TransactionPool.Instance.GetTransactionByInputTx(utxos[index].TransactionHash, utxos[index].OutputIndex);
                        if (transactionMsg != null)
                        {
                            getNextBatchUtxos(limit, ref start, ref index, ref utxos);
                            continue;
                        }
                        if (utxoTX == null || utxoBlock == null)
                        {
                            getNextBatchUtxos(limit, ref start, ref index, ref utxos);
                            continue;
                        }

                        if (!utxoBlock.IsVerified)
                        {
                            getNextBatchUtxos(limit, ref start, ref index, ref utxos);
                            continue;
                        }

                        if (Time.EpochTime < utxoTX.Locktime)
                        {
                            getNextBatchUtxos(limit, ref start, ref index, ref utxos);
                            continue;
                        }

                        if (utxoTX.InputCount == 1 && utxoTX.Inputs[0].OutputTransactionHash == Base16.Encode(HashHelper.EmptyHash()))
                        {
                            var blockHeight = utxoBlock.Height;

                            if (lastBlockHeight - blockHeight < 100L)
                            {
                                getNextBatchUtxos(limit, ref start, ref index, ref utxos);
                                continue;
                            }
                        }

                        var input = new InputMsg();
                        input.OutputTransactionHash = utxos[index].TransactionHash;
                        input.OutputIndex = utxos[index].OutputIndex;
                        input.UnlockScript = Script.BuildUnlockScript(input.OutputTransactionHash, input.OutputIndex, Base16.Decode(DecryptPrivateKey(account.PrivateKey)), Base16.Decode(account.PublicKey));
                        input.Size = input.UnlockScript.Length;
                        tx.Inputs.Add(input);

                        var size = input.Serialize().Length;
                        totalSize += size;
                        totalFee += setting.FeePerKB * ((double)size / 1024.0);
                        totalInput += utxos[index].Amount;
                    }
                    else
                    {
                        getNextBatchUtxos(limit, ref start, ref index, ref utxos);
                        continue;
                    }

                    if (feeDeductAddresses == null || feeDeductAddresses.Length == 0)
                    {
                        totalAmount = totalOutput + totalFee;
                    }

                    if (totalInput >= (long)Math.Ceiling(totalAmount))
                    {
                        var size = tx.Outputs[0].Serialize().Length;

                        if ((totalInput - (long)Math.Ceiling(totalAmount)) > (setting.FeePerKB * (double)size / 1024.0))
                        {
                            totalSize += size;
                            totalFee += setting.FeePerKB * ((double)size / 1024.0);

                            if (feeDeductAddresses == null || feeDeductAddresses.Length == 0)
                            {
                                totalAmount = totalOutput + totalFee;
                            }
                            //添加找零地址
                            Account newAccount = accountComponent.GetAccountById(account.Id);

                            if (newAccount == null)
                            {
                                throw new CommonException(ErrorCode.Engine.Transaction.Verify.CHANGE_ADDRESS_IS_INVALID);
                            }

                            var newOutput = new OutputMsg();
                            newOutput.Amount = totalInput - (long)Math.Ceiling(totalAmount);
                            newOutput.Index = tx.Outputs.Count;
                            newOutput.LockScript = Script.BuildLockScipt(newAccount.Id);
                            newOutput.Size = newOutput.LockScript.Length;
                            tx.Outputs.Add(newOutput);
                        }

                        break;
                    }
                    getNextBatchUtxos(limit, ref start, ref index, ref utxos);
                }

                var totalAmountLong = (long)Math.Ceiling(totalAmount);
                if (totalInput < totalAmountLong)
                {
                    throw new CommonException(ErrorCode.Service.Transaction.BALANCE_NOT_ENOUGH);
                }

                if (feeDeductAddresses != null && feeDeductAddresses.Length > 0)
                {
                    var averageFee = (long)Math.Ceiling(totalFee / feeDeductAddresses.Length);

                    List<string> feeDeductAddrs = new List<string>();
                    feeDeductAddrs.AddRange(feeDeductAddresses);
                    for (int i = 0; i < receivers.Length; i++)
                    {
                        if (feeDeductAddrs.Contains(receivers[i].address))
                        {
                            if (tx.Outputs[i].Amount > averageFee)
                            {
                                tx.Outputs[i].Amount -= averageFee;
                                feeDeductAddrs.Remove(receivers[i].address);
                            }
                        }
                    }

                    if (feeDeductAddrs.Count > 0)
                    {
                        throw new CommonException(ErrorCode.Service.Transaction.SEND_AMOUNT_LESS_THAN_FEE);
                    }
                }

                result.totalFee = (long)Math.Ceiling(totalFee);
                result.totalSize = totalSize;
                return result;
            }
            catch (CommonException ce)
            {
                throw ce;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IRpcMethodResult EstimateTxFeeForSendMany(string fromAccount, SendManyOutputIM[] receivers, string[] feeDeductAddresses)
        {
            try
            {
                TransactionMsg tx;
                long feeRate;
                var estimateTxFee = EstimateTxFee(fromAccount, receivers, feeDeductAddresses, out tx, out feeRate);

                return Ok(estimateTxFee);
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

        public IRpcMethodResult SendMany(string fromAccount, SendManyOutputIM[] receivers, string[] feeDeductAddresses, string changeAddress = null)
        {
            try
            {
                string result = null;

                var txComponent = new TransactionComponent();
                var addressBookComponent = new AddressBookComponent();
                var transactionCommentComponent = new TransactionCommentComponent();

                TransactionMsg tx;
                long feeRate;
                var estimateTxFee = EstimateTxFee(fromAccount, receivers, feeDeductAddresses, out tx,out feeRate);

                tx.Timestamp = Time.EpochTime;
                tx.Hash = tx.GetHash();
                txComponent.UpdateTxInputToSpent(tx);
                txComponent.CreateNewTransaction(tx,feeRate);
                Startup.P2PBroadcastTransactionAction(tx.Hash);
                result = tx.Hash;

                for (int i = 0; i < receivers.Length; i++)
                {
                    var receiver = receivers[i];

                    if (!string.IsNullOrWhiteSpace(receiver.tag))
                    {
                        addressBookComponent.SetTag(receiver.address, receiver.tag);
                        var bookitem = addressBookComponent.GetByAddress(receiver.address);
                        if (bookitem != null)
                            CacheManager.Default.Put(DataCatelog.AddressBook, bookitem.Address, bookitem);
                    }

                    if (!string.IsNullOrWhiteSpace(receiver.comment))
                    {
                        transactionCommentComponent.Add(tx.Hash, i, receiver.comment);
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
        
        public IRpcMethodResult EstimateTxFeeForSendToAddress(string toAddress, long amount, string comment, string commentTo, bool deductFeeFromAmount)
        {
            try
            {
                EstimateTxFeeOM result = new EstimateTxFeeOM();
                var utxoComponent = new UtxoComponent();
                var txComponent = new TransactionComponent();
                var settingComponent = new SettingComponent();
                var addressBookComponent = new AddressBookComponent();
                var accountComponent = new AccountComponent();

                if (!AccountIdHelper.AddressVerify(toAddress))
                {
                    throw new CommonException(ErrorCode.Service.Transaction.TO_ADDRESS_INVALID);
                }

                long start = 0;
                long limit = 10;
                var setting = settingComponent.GetSetting();
                var utxos = utxoComponent.GetAllConfirmedOutputs(start, limit);
                var tx = new TransactionMsg();
                var totalSize = tx.Serialize().Length;

                var output = new OutputMsg();
                output.Amount = amount;
                output.Index = 0;
                output.LockScript = Script.BuildLockScipt(toAddress);
                output.Size = output.LockScript.Length;
                tx.Outputs.Add(output);
                totalSize += output.Serialize().Length;

                var blockComponent = new BlockComponent();
                var lastBlockHeight = blockComponent.GetLatestHeight();

                var totalInput = 0L;
                var index = 0;
                double totalAmount = amount;
                double totalFee = setting.FeePerKB * ((double)totalSize / 1024.0);

                while (index < utxos.Count)
                {
                    //排除被锁定的
                    bool isLocked = false;
                    List<ListLockUnspentOM> lockList = Startup.lockUnspentList;
                    foreach (var item in lockList)
                    {
                        Output outputLocked = txComponent.GetOutputEntiyByIndexAndTxHash(item.txid, item.vout);
                        if (outputLocked.ReceiverId == utxos[index].AccountId)
                        {
                            isLocked = true;
                            break;
                        }
                    }
                    if (isLocked)
                    {
                        getNextBatchUtxos(limit, ref start, ref index, ref utxos);
                        continue;
                    }

                    var account = accountComponent.GetAccountById(utxos[index].AccountId);

                    if (account != null && !string.IsNullOrWhiteSpace(account.PrivateKey))
                    {
                        var utxoTX = txComponent.GetTransactionMsgByHash(utxos[index].TransactionHash);
                        Block utxoBlock = blockComponent.GetBlockEntiytByHash(utxos[index].BlockHash);
                        //排除交易池中的交易
                        TransactionMsg transactionMsg = TransactionPool.Instance.GetTransactionByInputTx(utxos[index].TransactionHash, utxos[index].OutputIndex);
                        if (transactionMsg != null)
                        {
                            getNextBatchUtxos(limit, ref start, ref index, ref utxos);
                            continue;
                        }
                        if (utxoTX == null || utxoBlock == null)
                        {
                            getNextBatchUtxos(limit, ref start, ref index, ref utxos);
                            continue;
                        }

                        if (!utxoBlock.IsVerified)
                        {
                            getNextBatchUtxos(limit, ref start, ref index, ref utxos);
                            continue;
                        }

                        if (Time.EpochTime < utxoTX.Locktime)
                        {
                            getNextBatchUtxos(limit, ref start, ref index, ref utxos);
                            continue;
                        }

                        if (utxoTX.InputCount == 1 && utxoTX.Inputs[0].OutputTransactionHash == Base16.Encode(HashHelper.EmptyHash()))
                        {
                            var blockHeight = utxoBlock.Height;

                            if (lastBlockHeight - blockHeight < 100L)
                            {
                                getNextBatchUtxos(limit, ref start, ref index, ref utxos);
                                continue;
                            }
                        }

                        var input = new InputMsg();
                        input.OutputTransactionHash = utxos[index].TransactionHash;
                        input.OutputIndex = utxos[index].OutputIndex;
                        input.UnlockScript = Script.BuildUnlockScript(input.OutputTransactionHash, input.OutputIndex, Base16.Decode(DecryptPrivateKey(account.PrivateKey)), Base16.Decode(account.PublicKey));
                        input.Size = input.UnlockScript.Length;
                        tx.Inputs.Add(input);

                        var size = input.Serialize().Length;
                        totalSize += size;
                        totalFee += setting.FeePerKB * ((double)size / 1024.0);
                        totalInput += utxos[index].Amount;
                    }
                    else
                    {
                        getNextBatchUtxos(limit, ref start, ref index, ref utxos);
                        continue;
                    }

                    if (!deductFeeFromAmount)
                    {
                        totalAmount = amount + totalFee;
                    }

                    if (totalInput >= (long)Math.Ceiling(totalAmount))
                    {
                        var size = output.Serialize().Length;

                        if ((totalInput - (long)Math.Ceiling(totalAmount)) > (setting.FeePerKB * (double)size / 1024.0))
                        {
                            totalSize += size;
                            totalFee += setting.FeePerKB * ((double)size / 1024.0);
                        }

                        break;
                    }

                    getNextBatchUtxos(limit, ref start, ref index, ref utxos);
                    //index++;
                    //if (index >= utxos.Count)
                    //{
                    //    start += 10;
                    //    utxos = utxoComponent.GetAllConfirmedOutputs(start, limit);

                    //    if (utxos.Count == 0)
                    //    {
                    //        break;
                    //    }
                    //    else
                    //    {
                    //        index = 0;
                    //    }
                    //}
                }

                if (totalInput < totalAmount)
                {
                    throw new CommonException(ErrorCode.Service.Transaction.BALANCE_NOT_ENOUGH);
                }

                if (deductFeeFromAmount)
                {
                    output.Amount -= (long)Math.Ceiling(totalFee);

                    if (output.Amount <= 0)
                    {
                        throw new CommonException(ErrorCode.Service.Transaction.SEND_AMOUNT_LESS_THAN_FEE);
                    }
                }
                
                result.totalFee = (long)Math.Ceiling(totalFee);
                result.totalSize = totalSize;

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

        private void getNextBatchUtxos(long limit, ref long start, ref int index, ref List<UtxoMsg> utxos)
        {
            index++;

            if (index >= utxos.Count)
            {
                start += 10;
                utxos = new UtxoComponent().GetAllConfirmedOutputs(start, limit);

                if (utxos.Count > 0)
                {
                    index = 0;
                }
            }
        }

        public IRpcMethodResult SetTxFee(long feePerKB)
        {
            try
            {
                var settingComponent = new SettingComponent();
                var setting = settingComponent.GetSetting();

                setting.FeePerKB = feePerKB;
                settingComponent.SaveSetting(setting);

                return Ok();
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

        public IRpcMethodResult SetConfirmations(long confirmations)
        {
            try
            {
                var settingComponent = new SettingComponent();
                var setting = settingComponent.GetSetting();

                setting.Confirmations = confirmations;
                settingComponent.SaveSetting(setting);

                return Ok();
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

        public IRpcMethodResult GetTxSettings()
        {
            LogHelper.Info("this is beginner");
            try
            {
                LogHelper.Info("this is setting init");
                var settingComponent = new SettingComponent();
                LogHelper.Info("this is get setting");
                var setting = settingComponent.GetSetting();
                LogHelper.Info("this is result");
                var result = new GetTxSettingsOM();
                LogHelper.Info("this is setting Confirmations");
                result.Confirmations = setting.Confirmations;
                LogHelper.Info("this is setting FeePerKB");
                result.FeePerKB = setting.FeePerKB;
                LogHelper.Info("this is encrypt");
                result.Encrypt = setting.Encrypt;
                LogHelper.Info("this is return");
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
        /// 列出以前的区块 by wangshibang
        /// </summary>
        /// <param name="blockHash"></param>
        /// <param name="confirmations"></param>
        /// <returns>返回值是ListSinceBlockOM对象</returns>
        public IRpcMethodResult ListSinceBlock(string blockHash, long confirmations)
        {
            try
            {
                BlockComponent component = new BlockComponent();
                //先调用接口获取区块总高度
                long height = component.GetLatestHeight();
                //获取指定BlockHash之后的大于等于指定确认次数的所有Block
                ListSinceBlock block = component.ListSinceBlock(blockHash, height, confirmations);

                return Ok(block);
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

        public IRpcMethodResult ListPageSinceBlock(string blockHash, long confirmations,int currentPage, int pageSize)
        {
            try
            {
                BlockComponent component = new BlockComponent();
                //先调用接口获取区块总高度
                long height = component.GetLatestHeight();
                //获取指定BlockHash之后的大于等于指定确认次数的所有Block
                ListSinceBlock block = component.ListPageSinceBlock(blockHash, height, confirmations, currentPage, pageSize);

                return Ok(block);
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
        /// 多对多交易
        /// </summary>
        /// <param name="senders"></param>
        /// <param name="receivers"></param>
        /// <param name="changeAddress"></param>
        /// <param name="feeRate"></param>
        /// <returns></returns>
        public IRpcMethodResult SendRawTransaction(SendRawTransactionInputsIM[] senders, SendRawTransactionOutputsIM[] receivers, string changeAddress, long lockTime, long feeRate)
        {
            try
            {
                string result = null;
                TransactionComponent txComponent = new TransactionComponent();
                AccountComponent accountComponent = new AccountComponent();
                TransactionCommentComponent transactionCommentComponent = new TransactionCommentComponent();
                BlockComponent blockComponent = new BlockComponent();
                var lastBlockHeight = blockComponent.GetLatestHeight();

                TransactionMsg tx = new TransactionMsg();
                double totalOutput = 0;
                var totalSize = tx.Serialize().Length;

                if (receivers == null || receivers.Length == 0)
                {
                    throw new CommonException(ErrorCode.Service.Transaction.TO_ADDRESS_INVALID);
                }
                //遍历所有的接收者，先验证地址合法性，向TransactionMsg中添加output，计算output的TotalSize和TotalOutput
                foreach (var receiver in receivers)
                {
                    if (!AccountIdHelper.AddressVerify(receiver.Address))
                    {
                        throw new CommonException(ErrorCode.Service.Transaction.TO_ADDRESS_INVALID);
                    }
                    //先把所有的输出添加到TransactionMsg中
                    if (receiver.Amount == 0)
                    {
                        throw new Exception($"receiver amount can not be zero, the receiver is {receiver}");
                    }
                    var output = new OutputMsg();
                    output.Amount = receiver.Amount;
                    output.Index = tx.Outputs.Count;
                    output.LockScript = Script.BuildLockScipt(receiver.Address);
                    output.Size = output.LockScript.Length;
                    tx.Outputs.Add(output);

                    //计算出所有的输出大小和输出金额
                    totalSize += output.Serialize().Length;
                    totalOutput += receiver.Amount;
                }

                //根据output的TotalSize和汇率计算总金额TotalAmount
                var totalInput = 0L;
                double totalFee = feeRate * (totalSize / 1024.0);
                double totalAmount = totalOutput;

                var hashIndexs = senders.Select(x => x.TxId + x.Vout);

                //排除被锁定的
                var lockList = Startup.lockUnspentList.ToArray();
                if (lockList.Any(x => hashIndexs.Contains(x.txid + x.vout)))
                {
                    throw new Exception("transaction is locked");
                }

                //拿到所有的发送数据
                var outputs = OutputDac.Default.SelectByHashAndIndexs(hashIndexs);
                if (outputs.Any(x => x.Spent))
                {
                    throw new CommonException(Engine.UTXO.UTXO_IS_SPENT);
                }

                if (TransactionPool.Instance.HasCostUtxo(hashIndexs))
                {
                    throw new CommonException(Engine.UTXO.UTXO_IS_SPENT);
                }

                var accounts = CacheManager.Default.Get<Account>(DataCatelog.Accounts);
                
                foreach (var output in outputs)
                {
                    //判断account是否为空和是不是观察者账户（没有私钥）
                    var account = accounts.FirstOrDefault(x => x.Id == output.ReceiverId);
                    if (account == null || string.IsNullOrWhiteSpace(account.PrivateKey))
                    {
                        throw new Exception($"account is invalid, invalid account is {output.ReceiverId}");
                    }
                    
                    //根据交易Id获取获取TransactionMsg和Block，然后进行有效性判断
                    var outputTx = TransactionDac.Default.SelectByHash(output.TransactionHash);
                    Block utxoBlock = blockComponent.GetBlockEntiytByHash(outputTx.BlockHash);

                    if (outputTx == null || utxoBlock == null)
                    {
                        throw new Exception("utxo transaction is null or utxo block is null");
                    }

                    if (!utxoBlock.IsVerified)
                    {
                        throw new Exception($"utxo block is unverified, the unverified blockhash is {utxoBlock.Hash}, txid is {output.TransactionHash}, vout is {output.Index}");
                    }

                    if (Time.EpochTime < outputTx.LockTime)
                    {
                        throw new Exception("utxo is locked");
                    }

                    //判断coinbase区块高度是否在100以内
                    var txInputs = InputDac.Default.SelectByTransactionHash(output.TransactionHash);
                    if (txInputs.Count == 1 && txInputs[0].OutputTransactionHash == Base16.Encode(HashHelper.EmptyHash()))
                    {
                        var blockHeight = utxoBlock.Height;

                        if (lastBlockHeight - blockHeight < 100L)
                        {
                            throw new Exception("coinbase is in 100 blocks");
                        }
                    }
                    //新建InputMsg，并计算区块大小和总的费用
                    var input = new InputMsg();
                    input.OutputTransactionHash = output.TransactionHash;
                    input.OutputIndex = output.Index;
                    input.UnlockScript = Script.BuildUnlockScript(input.OutputTransactionHash, input.OutputIndex, Base16.Decode(DecryptPrivateKey(account.PrivateKey)), Base16.Decode(account.PublicKey));
                    input.Size = input.UnlockScript.Length;
                    tx.Inputs.Add(input);
                    var size = input.Serialize().Length;
                    totalSize += size;
                    totalFee += feeRate * (size / 1024.0);
                    totalInput += output.Amount;
                }

                //计算总费用 = 总的输出+总的手续费
                totalAmount = totalOutput + totalFee;
                //判断总的输入和总的输出
                if (totalInput >= (long)Math.Ceiling(totalAmount))
                {
                    var size = tx.Outputs[0].Serialize().Length;
                    //因为需要找零，多了一个输出
                    if ((totalInput - (long)Math.Ceiling(totalAmount)) > (feeRate * (double)size / 1024.0))
                    {
                        totalSize += size;
                        totalFee += feeRate * (size / 1024.0);

                        totalAmount = totalOutput + totalFee;
                        //已经存在的地址不需要判断是否加密
                        var newAccount = accountComponent.GetAccountById(changeAddress);
                        if (newAccount == null)
                        {
                            throw new CommonException(ErrorCode.Service.Account.ACCOUNT_NOT_FOUND);
                        }
                        //找零，新建一个输出
                        var newOutput = new OutputMsg();
                        newOutput.Amount = totalInput - (long)Math.Ceiling(totalAmount);
                        newOutput.Index = tx.Outputs.Count;
                        newOutput.LockScript = Script.BuildLockScipt(changeAddress);
                        newOutput.Size = newOutput.LockScript.Length;
                        tx.Outputs.Add(newOutput);
                    }
                }
                else
                {
                    throw new CommonException(ErrorCode.Service.Transaction.BALANCE_NOT_ENOUGH);
                }

                var totalAmountLong =(long)Math.Ceiling(totalAmount);
                if (totalInput < totalAmountLong)
                {
                    throw new CommonException(ErrorCode.Service.Transaction.BALANCE_NOT_ENOUGH);
                }

                tx.Timestamp = Time.EpochTime;
                tx.Locktime = lockTime;
                tx.Hash = tx.GetHash();
                txComponent.UpdateTxInputToSpent(tx);
                txComponent.CreateNewTransaction(tx, feeRate);
                Startup.P2PBroadcastTransactionAction(tx.Hash);
                result = tx.Hash;

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
        /// 估算总的费用
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="changeAddress"></param>
        /// <param name="feeRate"></param>
        /// <returns>EstimateRawTransactionOM</returns>
        public IRpcMethodResult EstimateRawTransaction(SendRawTransactionInputsIM[] senders, SendRawTransactionOutputsIM[] receivers, string changeAddress, long feeRate)
        {
            try
            {
                var utxoComponent = new UtxoComponent();
                TransactionComponent txComponent = new TransactionComponent();
                var settingComponent = new SettingComponent();
                var setting = settingComponent.GetSetting();
                var addressBookComponent = new AddressBookComponent();
                var accountComponent = new AccountComponent();
                var transactionCommentComponent = new TransactionCommentComponent();
                var blockComponent = new BlockComponent();
                var lastBlockHeight = blockComponent.GetLatestHeight();

                var tx = new TransactionMsg();
                double totalOutput = 0;
                var totalSize = tx.Serialize().Length;

                if (receivers == null || receivers.Length == 0)
                {
                    throw new CommonException(ErrorCode.Service.Transaction.TO_ADDRESS_INVALID);
                }
                //遍历所有的接收者，先验证地址合法性，向TransactionMsg中添加output，计算TotalSize和TotalOutput
                foreach (var receiver in receivers)
                {
                    if (!AccountIdHelper.AddressVerify(receiver.Address))
                    {
                        throw new CommonException(ErrorCode.Service.Transaction.TO_ADDRESS_INVALID);
                    }
                    //先把所有的输出添加到TransactionMsg中
                    var output = new OutputMsg();
                    output.Amount = receiver.Amount;
                    output.Index = tx.Outputs.Count;
                    output.LockScript = Script.BuildLockScipt(receiver.Address);
                    output.Size = output.LockScript.Length;
                    tx.Outputs.Add(output);

                    //计算出所有的输出大小和输出金额
                    totalSize += output.Serialize().Length;
                    totalOutput += receiver.Amount;
                }

                //根据output的TotalSize和汇率计算总金额TotalAmount
                var totalInput = 0L;
                double totalFee = feeRate * (totalSize / 1024.0);
                double totalAmount = totalOutput;

                foreach (var sender in senders)
                {
                    //排除被锁定的
                    List<ListLockUnspentOM> lockList = Startup.lockUnspentList;
                    foreach (var item in lockList)
                    {
                        if (item.txid == sender.TxId && item.vout == sender.Vout)
                        {
                            throw new Exception("transaction is locked");
                        }
                    }
                    //先判断账号是不是自己的，根据Txid和vout获取output，然后根据ReceivedId获取Account
                    Output output = txComponent.GetOutputEntiyByIndexAndTxHash(sender.TxId, sender.Vout);
                    if (output.Spent)
                    {
                        throw new CommonException(Engine.UTXO.UTXO_IS_SPENT);
                    }
                    //判断交易池中是否有这个Utxo
                    TransactionMsg txMsg = TransactionPool.Instance.GetTransactionByInputTx(sender.TxId, sender.Vout);
                    if (txMsg != null)
                    {
                        throw new CommonException(Engine.UTXO.UTXO_IS_SPENT);
                    }
                    var account = accountComponent.GetAccountById(output.ReceiverId);

                    if (account != null && !string.IsNullOrWhiteSpace(account.PrivateKey))
                    {
                        TransactionMsg utxoTX = txComponent.GetTransactionMsgByHash(sender.TxId);
                        Block utxoBlock = blockComponent.GetBlockEntiytByHash(new TransactionComponent().GetTransactionEntityByHash(sender.TxId).BlockHash);

                        if (utxoTX == null || utxoBlock == null)
                        {
                            throw new Exception("utxo transaction is null or utxo block is null");
                        }

                        if (!utxoBlock.IsVerified)
                        {
                            throw new Exception("utxo block is unverified");
                        }

                        if (Time.EpochTime < utxoTX.Locktime)
                        {
                            throw new Exception("utxo is locked");
                        }

                        if (utxoTX.InputCount == 1 && utxoTX.Inputs[0].OutputTransactionHash == Base16.Encode(HashHelper.EmptyHash()))
                        {
                            var blockHeight = utxoBlock.Height;

                            if (lastBlockHeight - blockHeight < 100L)
                            {
                                throw new Exception("coinbase is in 100 blocks");
                            }
                        }

                        var input = new InputMsg();
                        input.OutputTransactionHash = sender.TxId;
                        input.OutputIndex = sender.Vout;
                        input.UnlockScript = Script.BuildUnlockScript(input.OutputTransactionHash, input.OutputIndex, Base16.Decode(DecryptPrivateKey(account.PrivateKey)), Base16.Decode(account.PublicKey));
                        input.Size = input.UnlockScript.Length;
                        tx.Inputs.Add(input);

                        var size = input.Serialize().Length;
                        totalSize += size;
                        totalFee += feeRate * (size / 1024.0);
                        totalInput += output.Amount;
                    }
                    else
                    {
                        throw new Exception("account is invalid");
                    }
                }
                //计算总费用
                totalAmount = totalOutput + totalFee;
                if (totalInput >= (long)Math.Ceiling(totalAmount))
                {
                    var size = tx.Outputs[0].Serialize().Length;

                    if ((totalInput - (long)Math.Ceiling(totalAmount)) > (feeRate * (double)size / 1024.0))
                    {
                        totalSize += size;
                        totalFee += feeRate * (size / 1024.0);

                        totalAmount = totalOutput + totalFee;

                        var newAccount = accountComponent.GetAccountById(changeAddress);

                        var newOutput = new OutputMsg();
                        newOutput.Amount = totalInput - (long)Math.Ceiling(totalAmount);
                        newOutput.Index = tx.Outputs.Count;
                        newOutput.LockScript = Script.BuildLockScipt(changeAddress);
                        newOutput.Size = newOutput.LockScript.Length;
                        tx.Outputs.Add(newOutput);
                    }
                    else
                    {
                        throw new Exception("balance not enough");
                    }
                }
                else
                {
                    throw new CommonException(ErrorCode.Service.Transaction.BALANCE_NOT_ENOUGH);
                }
                EstimateRawTransactionOM om = new EstimateRawTransactionOM();
                om.totalSize = (long)(Math.Ceiling(totalAmount));
                om.totalFee = (long)(Math.Ceiling(totalFee));
                om.Change = totalInput - (long)Math.Ceiling(totalAmount);
                var totalAmountLong = (long)(Math.Ceiling(totalAmount));
                if (totalInput < totalAmountLong)
                {
                    throw new CommonException(ErrorCode.Service.Transaction.BALANCE_NOT_ENOUGH);
                }

                return Ok(om);
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

        public IRpcMethodResult SendNotify(string txId)
        {
            try
            {
                NotifyComponent component = new NotifyComponent();
                component.ProcessNewTxReceived(txId);
                return Ok();
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
        /// 根据交易Id获取交易信息
        /// </summary>
        /// <param name="txId"></param>
        /// <returns></returns>
        public IRpcMethodResult GetTransaction(string txId)
        {
            try
            {
                Transaction item = new TransactionComponent().GetTransactionEntityByHash(txId);
                if (item == null)
                {
                    return null;
                }
                BlockComponent component = new BlockComponent();
                TransactionOM transaction = new TransactionOM();

                transaction.BlockHash = item.BlockHash;
                transaction.Confirmations = string.IsNullOrEmpty(item.BlockHash) ? 0 : component.GetLatestHeight() - component.GetBlockEntiytByHash(item.BlockHash).Height + 1;
                transaction.ExpiredTime = item.ExpiredTime;
                transaction.Fee = item.Fee;
                transaction.Hash = item.Hash;
                transaction.Id = item.Id;
                transaction.IsDiscarded = item.IsDiscarded;
                transaction.LockTime = item.LockTime;
                transaction.Size = item.Size;
                transaction.Timestamp = item.Timestamp;
                transaction.TotalInput = item.TotalInput;
                transaction.TotalOutput = item.TotalOutput;
                transaction.Version = item.Version;
                transaction.Inputs = item.Inputs.Select(p => new InputsOM { AccountId = p.AccountId, Amount = p.Amount, IsDiscarded = p.IsDiscarded, Vout = p.OutputIndex, Txid = p.OutputTransactionHash, Size = p.Size, UnlockScript = p.UnlockScript }).ToList();
                transaction.Outputs = item.Outputs.Select(p => new OutputsOM { Amount = p.Amount, Vout = p.Index, IsDiscarded = p.IsDiscarded, LockScript = p.LockScript, ReceiverId = p.ReceiverId, Size = p.Size, Spent = p.Spent, Txid = p.TransactionHash }).ToList();
                return Ok(transaction);
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

        public IRpcMethodResult ListTransactions(string account, int count, int skip = 0, bool includeWatchOnly = true)
        {
            try
            {
                var txComponent = new TransactionComponent();
                var accountComponent = new AccountComponent();
                var addressBookComponent = new AddressBookComponent();
                var utxoComponent = new UtxoComponent();
                var blockComponent = new BlockComponent();
                var transactionCommentComponent = new TransactionCommentComponent();

                List<PaymentOM> result = new List<PaymentOM>();
                var accounts = CacheManager.Default.Get<Account>(DataCatelog.Accounts);
                var addressBook = CacheManager.Default.Get<AddressBookItem>(DataCatelog.AddressBook);
                var latestHeight = blockComponent.GetLatestHeight();

                var payments = CacheManager.Default.Get<PaymentCache>(DataCatelog.Payment);

                var resultItems = payments.OrderByDescending(x => x.time).Skip(skip).Take(count).ToList();
                
                BlockDac blockDac = BlockDac.Default;
                var txhashs = resultItems.Select(x => x.txId).Distinct().ToList();
                var blockTransList = blockDac.GetBlocksByTxHash(txhashs);
                resultItems.ForEach(x =>
                {
                    var bt = blockTransList.FirstOrDefault(b => b.TransHash == x.txId);
                    if (bt != null)
                    {
                        var confirmations = latestHeight - bt.Height + 1;
                        x.confirmations = confirmations;
                        x.blockHash = bt.BlockHash;
                        x.blockIndex = 0;
                        x.blockTime = bt.Timestamp;
                    }
                    else
                    {
                        x.confirmations = 0;
                    }
                    var currentaddressbook = addressBook.FirstOrDefault(a => a.Address == x.address);
                    if (currentaddressbook != null)
                    {
                        x.account = currentaddressbook.Tag;
                    }
                });


                return Ok(resultItems);
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
        /// 根据输入判断txid是否存在
        /// </summary>
        /// <param name="txId"></param>
        /// <param name="vout"></param>
        /// <returns></returns>
        public IRpcMethodResult GetTxHashByInput(string txId, int vout)
        {
            try
            {
                TransactionComponent txComponent = new TransactionComponent();
                var hash = txComponent.GetTransactionHashByInputTx(txId, vout);

                return Ok(hash);
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

        private string DecryptPrivateKey(string privateKey)
        {
            var setting = new SettingComponent().GetSetting();

            if (setting.Encrypt)
            {
                if (!string.IsNullOrWhiteSpace(_cache.Get<string>("WalletPassphrase")))
                {
                    try
                    {
                        return AES128.Decrypt(privateKey, _cache.Get<string>("WalletPassphrase"));
                    }
                    catch
                    {
                        throw new CommonException(ErrorCode.Service.Transaction.WALLET_DECRYPT_FAIL);
                    }
                }
                else
                {
                    throw new CommonException(ErrorCode.Service.Transaction.WALLET_DECRYPT_FAIL);
                }
            }
            else
            {
                return privateKey;
            }
        }

        public IRpcMethodResult CreateRawTransaction(SendRawTransactionInputsIM[] senders, SendRawTransactionOutputsIM[] receivers, string changeAddress, long lockTime, long feeRate)
        {
            try
            {
                var utxoComponent = new UtxoComponent();
                TransactionComponent txComponent = new TransactionComponent();
                TransactionMsgExtend extend = new TransactionMsgExtend();
                var settingComponent = new SettingComponent();
                var setting = settingComponent.GetSetting();
                var addressBookComponent = new AddressBookComponent();
                var accountComponent = new AccountComponent();
                var transactionCommentComponent = new TransactionCommentComponent();
                var blockComponent = new BlockComponent();
                var lastBlockHeight = blockComponent.GetLatestHeight();

                TransactionMsg tx = new TransactionMsg();
                List<SignInfo> signList = new List<SignInfo>(); 
                double totalOutput = 0;
                var totalSize = tx.Serialize().Length;

                if (receivers == null || receivers.Length == 0)
                {
                    throw new CommonException(ErrorCode.Service.Transaction.TO_ADDRESS_INVALID);
                }
                //遍历所有的接收者，先验证地址合法性，向TransactionMsg中添加output，计算output的TotalSize和TotalOutput
                foreach (var receiver in receivers)
                {
                    if (!AccountIdHelper.AddressVerify(receiver.Address))
                    {
                        throw new CommonException(ErrorCode.Service.Transaction.TO_ADDRESS_INVALID);
                    }
                    //先把所有的输出添加到TransactionMsg中
                    if (receiver.Amount == 0)
                    {
                        throw new Exception($"receiver amount can not be zero, the receiver is {receiver}");
                    }
                    var output = new OutputMsg();
                    output.Amount = receiver.Amount;
                    output.Index = tx.Outputs.Count;
                    output.LockScript = Script.BuildLockScipt(receiver.Address);
                    output.Size = output.LockScript.Length;
                    tx.Outputs.Add(output);

                    //计算出所有的输出大小和输出金额
                    totalSize += output.Serialize().Length;
                    totalOutput += receiver.Amount;
                }

                //根据output的TotalSize和汇率计算总金额TotalAmount
                var totalInput = 0L;
                double totalFee = feeRate * (totalSize / 1024.0);
                double totalAmount = totalOutput;

                foreach (var sender in senders)
                {
                    //排除被锁定的
                    List<ListLockUnspentOM> lockList = Startup.lockUnspentList;
                    foreach (var item in lockList)
                    {
                        if (item.txid == sender.TxId && item.vout == sender.Vout)
                        {
                            throw new Exception("transaction is locked");
                        }
                    }
                    //先判断账号是不是自己的，根据Txid和vout获取output，然后根据ReceivedId获取Account
                    Output output = txComponent.GetOutputEntiyByIndexAndTxHash(sender.TxId, sender.Vout);
                    if (output.Spent)
                    {
                        throw new CommonException(Engine.UTXO.UTXO_IS_SPENT);
                    }
                    //排除交易池中的交易
                    TransactionMsg transactionMsg = TransactionPool.Instance.GetTransactionByInputTx(sender.TxId, sender.Vout);
                    if (transactionMsg != null)
                    {
                        throw new CommonException(Engine.UTXO.UTXO_IS_SPENT);
                    }
                    var account = accountComponent.GetAccountById(output.ReceiverId);

                    //判断account是否为空和是不是观察者账户（没有私钥）
                    if (account != null)
                    {
                        //根据交易Id获取获取TransactionMsg和Block，然后进行有效性判断
                        TransactionMsg utxoTX = txComponent.GetTransactionMsgByHash(sender.TxId);
                        Block utxoBlock = blockComponent.GetBlockEntiytByHash(new TransactionComponent().GetTransactionEntityByHash(sender.TxId).BlockHash);

                        if (utxoTX == null || utxoBlock == null)
                        {
                            throw new Exception("utxo transaction is null or utxo block is null");
                        }

                        if (!utxoBlock.IsVerified)
                        {
                            throw new Exception($"utxo block is unverified, the unverified blockhash is {utxoBlock.Hash}, txid is {sender.TxId}, vout is {sender.Vout}");
                        }

                        if (Time.EpochTime < utxoTX.Locktime)
                        {
                            throw new Exception("utxo is locked");
                        }

                        //判断coinbase区块高度是否在100以内
                        if (utxoTX.InputCount == 1 && utxoTX.Inputs[0].OutputTransactionHash == Base16.Encode(HashHelper.EmptyHash()))
                        {
                            var blockHeight = utxoBlock.Height;

                            if (lastBlockHeight - blockHeight < 100L)
                            {
                                throw new Exception("coinbase is in 100 blocks");
                            }
                        }
                        //新建InputMsg，并计算区块大小和总的费用
                        var input = new InputMsg();
                        input.OutputTransactionHash = sender.TxId;
                        input.OutputIndex = sender.Vout;
                        input.UnlockScript = "";
                        input.Size = input.UnlockScript.Length;
                        tx.Inputs.Add(input);

                        var size = input.Serialize().Length;
                        totalSize += size;
                        totalFee += feeRate * (size / 1024.0);
                        totalInput += output.Amount;

                        //加入SignInfo
                        SignInfo info = new SignInfo();
                        info.Address = account.Id;
                        info.Txid = sender.TxId;
                        info.Vout = sender.Vout;

                        signList.Add(info);
                    }
                    else
                    {
                        throw new Exception("account is invalid");
                    }
                }
                //计算总费用 = 总的输出+总的手续费
                totalAmount = totalOutput + totalFee;
                //判断总的输入和总的输出
                if (totalInput >= (long)Math.Ceiling(totalAmount))
                {
                    var size = tx.Outputs[0].Serialize().Length;
                    //因为需要找零，多了一个输出
                    if ((totalInput - (long)Math.Ceiling(totalAmount)) > (feeRate * (double)size / 1024.0))
                    {
                        totalSize += size;
                        totalFee += feeRate * (size / 1024.0);

                        totalAmount = totalOutput + totalFee;
                        //已经存在的地址不需要判断是否加密
                        var newAccount = accountComponent.GetAccountById(changeAddress);
                        if (newAccount == null)
                        {
                            throw new CommonException(ErrorCode.Service.Account.ACCOUNT_NOT_FOUND);
                        }
                        //找零，新建一个输出
                        var newOutput = new OutputMsg();
                        newOutput.Amount = totalInput - (long)Math.Ceiling(totalAmount);
                        newOutput.Index = tx.Outputs.Count;
                        newOutput.LockScript = Script.BuildLockScipt(changeAddress);
                        newOutput.Size = newOutput.LockScript.Length;
                        tx.Outputs.Add(newOutput);
                    }
                }
                else
                {
                    throw new CommonException(ErrorCode.Service.Transaction.BALANCE_NOT_ENOUGH);
                }

                var totalAmountLong = (long)(Math.Ceiling(totalAmount));
                if (totalInput < totalAmountLong)
                {
                    throw new CommonException(ErrorCode.Service.Transaction.BALANCE_NOT_ENOUGH);
                }

                tx.Timestamp = Time.EpochTime;
                tx.Locktime = lockTime;
                //获取没有签名的Hash
                tx.Hash = "";
                //构建TransactionMsgExtend
                extend.Msg = tx;
                extend.SignList = signList;
                return Ok(extend);
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

        public IRpcMethodResult SignRawTransaction(TransactionMsgExtend extend)
        {
            try
            {
                //address必须和outputHash，outputIndex的receivedId一致，否则会报错
                //TransactionMsg msg = Newtonsoft.Json.JsonConvert.DeserializeObject<TransactionMsg>(transactionMsg);
                //根据address获取privatekey
                AccountComponent accountComponent = new AccountComponent();
                //Account account = accountComponent.GetAccountById(address);
                //给msg的InputMsgUnlockScript签名，生成txhash
                if (extend != null)
                {
                    foreach (var input in extend.Msg.Inputs)
                    {
                        //根据input的txid查找对应的SignInfo
                        List<SignInfo> list = extend.SignList;
                        string address = list.SingleOrDefault(q => q.Txid == input.OutputTransactionHash && q.Vout == input.OutputIndex).Address;
                        Account account = accountComponent.GetAccountById(address);
                        if (account == null || string.IsNullOrEmpty(account.PrivateKey))
                        {
                            throw new CommonException(ErrorCode.Service.Account.ACCOUNT_NOT_FOUND);
                        }
                        input.UnlockScript = Script.BuildUnlockScript(input.OutputTransactionHash, input.OutputIndex, Base16.Decode(DecryptPrivateKey(account.PrivateKey)), Base16.Decode(account.PublicKey));
                        input.Size = input.UnlockScript.Length;
                    }
                }
                extend.Msg.Hash = extend.Msg.GetHash();
                return Ok(extend.Msg);
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

        public IRpcMethodResult BroadcastTransaction(TransactionMsg msg)
        {
            try
            {
                TransactionComponent txComponent = new TransactionComponent();
                txComponent.AddTransactionToPool(msg);
                Startup.P2PBroadcastTransactionAction(msg.Hash);
                return Ok();
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

        public IRpcMethodResult ListFilterTrans(FilterOM filter, int count, int skip = 0, bool includeWatchOnly = true)
        {
            try
            {
                var txComponent = new TransactionComponent();
                var accountComponent = new AccountComponent();
                var addressBookComponent = new AddressBookComponent();
                var utxoComponent = new UtxoComponent();
                var blockComponent = new BlockComponent();
                var transactionCommentComponent = new TransactionCommentComponent();

                List<PaymentOM> result = new List<PaymentOM>();
                var accounts = CacheManager.Default.Get<Account>(DataCatelog.Accounts);
                var addressBook = CacheManager.Default.Get<AddressBookItem>(DataCatelog.AddressBook);

                var latestHeight = blockComponent.GetLatestHeight();

                var payments = new List<PaymentCache>();
                
                
                Func<string, bool> accountfunc = null;
                Func<string, bool> catelogfunc = null;
                Func<string, bool> amountfunc = null;
                Func<string, bool> timefunc = null;
                Func<string, bool> func = key =>
                {
                    try
                    {
                        var ps = key.Split("_");
                        if (catelogfunc != null && !catelogfunc(ps[0]))
                            return false;
                        if (accountfunc != null && !accountfunc(ps[1]))
                            return false;
                        if (amountfunc != null && !amountfunc(ps[2]))
                            return false;
                        if (timefunc != null && !timefunc(ps[3]))
                            return false;
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                };

                switch (filter.TradeType)
                {
                    case 1:
                        catelogfunc = key=> key.Equals(PaymentCatelog.Receive);
                        break;
                    case 2:
                        catelogfunc = key => key.Equals(PaymentCatelog.Send);
                        break;
                    case 3:
                        catelogfunc = key => key.Equals(PaymentCatelog.Self);
                        break;
                    case 4:
                        catelogfunc = key => key.Equals(PaymentCatelog.Generate);
                        break;
                    default:
                        break;
                }

                if (filter.Account != "*" && !string.IsNullOrEmpty(filter.Account))
                {
                    accountfunc = key => key.Contains(filter.Account);
                }

                if (filter.Amount > 0)
                {
                    amountfunc = key => long.Parse(key) >= filter.Amount;
                }

                if (filter.EndTime > 0)
                {
                    timefunc = key =>
                    {
                        var time = long.Parse(key);
                        return time > filter.StartTime && time < filter.EndTime;
                    };
                }

                payments = CacheManager.Default.Get<PaymentCache>(DataCatelog.Payment, func);
                
                var resultItems = payments.OrderByDescending(x => x.time).Skip(skip).Take(count).ToList();

                var txhashs = resultItems.Select(x => x.txId).Distinct().ToList();
                BlockDac blockDac = BlockDac.Default;
                var blockTransList = blockDac.GetBlocksByTxHash(txhashs);
                resultItems.ForEach(x =>
                {
                    var bt = blockTransList.FirstOrDefault(b => b.TransHash == x.txId);
                    if (bt != null)
                    {
                        var confirmations = latestHeight - bt.Height + 1;
                        x.confirmations = confirmations;
                        x.blockHash = bt.BlockHash;
                        x.blockIndex = 0;
                        x.blockTime = bt.Timestamp;
                    }
                    else
                    {
                        x.confirmations = 0;
                    }
                    var currentaddressbook = addressBook.FirstOrDefault(a => a.Address == x.address);
                    if (currentaddressbook != null)
                    {
                        x.account = currentaddressbook.Tag;
                    }
                });
                
                return Ok(resultItems);
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
    }
}