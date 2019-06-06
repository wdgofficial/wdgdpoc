// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.Messages;
using FiiiChain.Entities;
using FiiiChain.DataAgent;
using System;
using System.Collections.Generic;
using System.Text;
using FiiiChain.Data;
using FiiiChain.Framework;
using FiiiChain.Consensus;
using System.Linq;
using FiiiChain.Entities.ExtensionModels;
using FiiiChain.Data.Accesses;
using FiiiChain.Business.Extensions;
using FiiiChain.Business.ParamsModel;
using FiiiChain.Entities.CacheModel;
using FiiiChain.IModules;

namespace FiiiChain.Business
{
    public class TransactionComponent
    {
        //public TransactionMsg CreateNewTransactionMsg(string senderAccountId, Dictionary<string, long> receiverIdAndValues, long fee)
        //{
        //    var outputDac = OutputDac.Default;
        //    var accountDac = AccountDac.Default;

        //    var account = accountDac.SelectById(senderAccountId);

        //    if (account == null || account.WatchedOnly)
        //    {
        //        //TODO: throw exception;
        //        return null;
        //    }

        //    var balance = UtxoSet.Instance.GetAccountBlanace(senderAccountId, true);
        //    long totalOutput = fee;
        //    long totalInput = 0;

        //    foreach (var key in receiverIdAndValues.Keys)
        //    {
        //        totalOutput += receiverIdAndValues[key];
        //    }

        //    if (totalOutput > balance)
        //    {
        //        //TODO: throw exception
        //        return null;
        //    }

        //    var transaction = new TransactionMsg();
        //    transaction.Timestamp = Time.EpochTime;
        //    transaction.Locktime = 0;

        //    var outputs = outputDac.SelectUnspentByReceiverId(senderAccountId);

        //    if (outputs == null || !outputs.Any())
        //        return null;

        //    List<string> hashIndexs = new List<string>();

        //    foreach (var output in outputs)
        //    {
        //        var inputMsg = new InputMsg();
        //        inputMsg.OutputTransactionHash = output.TransactionHash;
        //        inputMsg.OutputIndex = output.Index;
        //        inputMsg.UnlockScript = Script.BuildUnlockScript(output.TransactionHash, output.Index, Base16.Decode(account.PrivateKey), Base16.Decode(account.PublicKey));
        //        inputMsg.Size = inputMsg.UnlockScript.Length;

        //        transaction.Inputs.Add(inputMsg);
        //        hashIndexs.Add(output.TransactionHash + output.Index.ToString());

        //        totalInput += output.Amount;
        //        if (totalInput >= totalOutput)
        //        {
        //            break;
        //        }
        //    }

        //    TaskQueue.AddWaitAction(() =>
        //    {
        //        outputDac.UpdateSpentStatuses(hashIndexs);
        //    });


        //    int index = 0;

        //    foreach (var key in receiverIdAndValues.Keys)
        //    {
        //        var value = receiverIdAndValues[key];

        //        var outputMsg = new OutputMsg();
        //        outputMsg.Index = index;
        //        outputMsg.Amount = value;
        //        outputMsg.LockScript = Script.BuildLockScipt(key);
        //        outputMsg.Size = outputMsg.LockScript.Length;

        //        transaction.Outputs.Add(outputMsg);
        //        index++;
        //    }

        //    if (totalInput > totalOutput)
        //    {
        //        var value = totalInput - totalOutput;

        //        var outputMsg = new OutputMsg();
        //        outputMsg.Index = index;
        //        outputMsg.Amount = value;
        //        outputMsg.LockScript = Script.BuildLockScipt(senderAccountId);
        //        outputMsg.Size = outputMsg.LockScript.Length;

        //        transaction.Outputs.Add(outputMsg);
        //        index++;
        //    }

        //    transaction.Hash = transaction.GetHash();

        //    return transaction;
        //}

        //public TransactionMsg CreateNewTransactionMsg(List<UtxoMsg> utxos, Dictionary<string, long> receiverIdAndValues)
        //{
        //    var outputDac = OutputDac.Default;
        //    var accountDac = AccountDac.Default;

        //    var transaction = new TransactionMsg();
        //    transaction.Timestamp = Time.EpochTime;
        //    transaction.Locktime = 0;

        //    foreach (var utxo in utxos)
        //    {
        //        var account = accountDac.SelectById(utxo.AccountId);

        //        if (account == null || account.WatchedOnly)
        //        {
        //            //TODO: throw exception;
        //            return null;
        //        }

        //        var privateKey = account.PrivateKey;
        //        var inputMsg = new InputMsg();
        //        inputMsg.OutputTransactionHash = utxo.TransactionHash;
        //        inputMsg.OutputIndex = utxo.OutputIndex;
        //        inputMsg.UnlockScript = Script.BuildUnlockScript(utxo.TransactionHash, utxo.OutputIndex, Base16.Decode(privateKey), Base16.Decode(account.PublicKey));
        //        inputMsg.Size = inputMsg.UnlockScript.Length;

        //        transaction.Inputs.Add(inputMsg);
        //        TaskQueue.AddWaitAction(() =>
        //        {
        //            outputDac.UpdateSpentStatus(utxo.TransactionHash, utxo.OutputIndex);
        //        });
        //    }

        //    int index = 0;

        //    foreach (var key in receiverIdAndValues.Keys)
        //    {
        //        var value = receiverIdAndValues[key];

        //        var outputMsg = new OutputMsg();
        //        outputMsg.Index = index;
        //        outputMsg.Amount = value;
        //        outputMsg.LockScript = Script.BuildLockScipt(key);
        //        outputMsg.Size = outputMsg.LockScript.Length;

        //        transaction.Outputs.Add(outputMsg);
        //        index++;
        //    }

        //    transaction.Hash = transaction.GetHash();
        //    return transaction;
        //}

        public void CreateNewTransaction(TransactionMsg transaction, long feeRate)
        {
            TransactionPool.Instance.AddNewTransaction(feeRate, transaction);
        }

        public void AddTransactionToPool(TransactionMsg transaction)
        {
            var isBlacked = BlacklistTxs.Current.IsBlacked(transaction.Hash);
            if (isBlacked)
                return;

            var accountDac = AccountDac.Default;
            var outputDac = OutputDac.Default;
            long feeRate = 0;
            long totalInput = 0;
            long totalOutput = 0;

            //var accounts = new AccountComponent().GetAllAccounts();
            foreach (var input in transaction.Inputs)
            {
                long amount;
                string lockCript;
                long blockHeight;

                if (this.getOutput(input.OutputTransactionHash, input.OutputIndex, out amount, out lockCript, out blockHeight))
                {
                    totalInput += amount;
                }
            }
            foreach (var output in transaction.Outputs)
            {
                totalOutput += output.Amount;
            }

            feeRate = (totalInput - totalOutput) / transaction.Size;

            long fee = 0;
            bool result = false;
            try
            {
                result = this.VerifyTransaction(transaction, out fee);
            }
            catch (CommonException ex)
            {
                LogHelper.Error(transaction.Hash + ":" + ex.ToString());
                //交易出错时，需要添加到黑名单所在的文件夹
                BlacklistTxs.Current.Add(transaction.Hash);
                BlacklistTxs.Current.AddToBlackFile(transaction);
                return;
            }

            try
            {
                if (result)
                {
                    TransactionPool.Instance.AddNewTransaction(feeRate, transaction);
                    //recheck isolate transactions
                    var txList = TransactionPool.Instance.GetIsolateTransactions();
                    //LogHelper.Debug($"this is spent lots of time date time is {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fffffff")}");
                    foreach (var tx in txList)
                    {
                        try
                        {
                            TransactionPool.Instance.MoveIsolateTransactionToMainPool(tx.Hash);
                        }
                        catch { }
                    }
                }
                else
                {
                    TransactionPool.Instance.AddNewTransaction(feeRate, transaction);
                }

                var spentUtxo = transaction.Inputs.Select(x => x.OutputTransactionHash + x.OutputIndex);
                OutputDac.Default.UpdateSpentStatuses(spentUtxo);

                //foreach (var output in transaction.Outputs)
                //{
                //    var accountId = AccountIdHelper.CreateAccountAddressByPublicKeyHash(
                //            Base16.Decode(
                //                Script.GetPublicKeyHashFromLockScript(output.LockScript)
                //            )
                //        );

                //if (accounts.Count(a => a.Id == accountId) > 0)
                //{
                //    UtxoSet.Instance.AddUtxoRecord(new UtxoMsg
                //    {
                //        AccountId = accountId,
                //        BlockHash = null,
                //        TransactionHash = transaction.Hash,
                //        OutputIndex = output.Index,
                //        Amount = output.Amount,
                //        IsConfirmed = false
                //    });
                //}
                //}
            }
            catch (CommonException ex)
            {
                LogHelper.Error(ex.ToString());
                return;
            }
        }

        public void UpdateTxInputToSpent(TransactionMsg tx)
        {
            var outputDac = OutputDac.Default;
            var utxos = tx.Inputs.Select(x => new SimpleUtxo { TransactionHash = x.OutputTransactionHash, Index = x.OutputIndex });
            TaskQueue.AddWaitAction(() =>
            {
                outputDac.UpdateSpentStatus(utxos);
            }, "UpdateTxInputToSpent");
        }

        public bool VerifyTransaction(TransactionMsg transaction, out long txFee, BlockMsg block = null)
        {
            var blockComponent = new BlockComponent();

            //compatible with old node
            if (transaction.Locktime > 0 && transaction.ExpiredTime == transaction.Locktime)
            {
                transaction.ExpiredTime = 0;
            }

            //step 0
            if (transaction.Hash != transaction.GetHash())
            {
                LogHelper.Error("Tx Hash Error:" + transaction.Hash);
                LogHelper.Error("Timestamp:" + transaction.Timestamp);
                LogHelper.Error("Locktime:" + transaction.Locktime);
                LogHelper.Error("ExpiredTime:" + transaction.ExpiredTime);
                LogHelper.Error("InputCount:" + transaction.InputCount);
                LogHelper.Error("OutputCount:" + transaction.OutputCount);

                throw new CommonException(ErrorCode.Engine.Transaction.Verify.TRANSACTION_HASH_ERROR);
            }

            //step 1
            if (transaction.InputCount == 0 || transaction.OutputCount == 0)
            {
                throw new CommonException(ErrorCode.Engine.Transaction.Verify.INPUT_AND_OUTPUT_CANNOT_BE_EMPTY);
            }

            //step 2
            if (transaction.Hash == Base16.Encode(HashHelper.EmptyHash()))
            {
                throw new CommonException(ErrorCode.Engine.Transaction.Verify.HASH_CANNOT_BE_EMPTY);
            }

            //step 3
            if (transaction.Locktime < 0 || transaction.Locktime > (Time.EpochTime + BlockSetting.LOCK_TIME_MAX))
            {
                throw new CommonException(ErrorCode.Engine.Transaction.Verify.LOCK_TIME_EXCEEDED_THE_LIMIT);
            }

            //step 4
            if (transaction.Serialize().Length < BlockSetting.TRANSACTION_MIN_SIZE)
            {
                throw new CommonException(ErrorCode.Engine.Transaction.Verify.TRANSACTION_SIZE_BELOW_THE_LIMIT);
            }

            //step 5
            if (this.existsInDB(transaction.Hash))
            {
                throw new CommonException(ErrorCode.Engine.Transaction.Verify.TRANSACTION_HAS_BEEN_EXISTED);
            }

            long totalOutput = 0;
            long totalInput = 0;

            foreach (var output in transaction.Outputs)
            {
                if (output.Amount <= 0 || output.Amount > BlockSetting.OUTPUT_AMOUNT_MAX)
                {
                    throw new CommonException(ErrorCode.Engine.Transaction.Verify.OUTPUT_EXCEEDED_THE_LIMIT);
                }

                if (!Script.VerifyLockScriptFormat(output.LockScript))
                {
                    throw new CommonException(ErrorCode.Engine.Transaction.Verify.SCRIPT_FORMAT_ERROR);
                }

                totalOutput += output.Amount;
            }

            foreach (var input in transaction.Inputs)
            {
                if (!Script.VerifyUnlockScriptFormat(input.UnlockScript))
                {
                    throw new CommonException(ErrorCode.Engine.Transaction.Verify.SCRIPT_FORMAT_ERROR);
                }

                long amount;
                string lockScript;
                long blockHeight;
                var result = this.getOutput(input.OutputTransactionHash, input.OutputIndex, out amount, out lockScript, out blockHeight);

                if (result)
                {
                    var utxoTx = this.GetTransactionMsgByHash(input.OutputTransactionHash);

                    if (utxoTx.InputCount == 1 && utxoTx.Inputs[0].OutputTransactionHash == Base16.Encode(HashHelper.EmptyHash()) &&
                        (blockHeight < 0 || (blockComponent.GetLatestHeight() - blockHeight < 100)))
                    {
                        throw new CommonException(ErrorCode.Engine.Transaction.Verify.COINBASE_NEED_100_CONFIRMS);
                    }

                    if (Time.EpochTime < utxoTx.Locktime)
                    {
                        throw new CommonException(ErrorCode.Engine.Transaction.Verify.TRANSACTION_IS_LOCKED);
                    }

                    if (block != null && block.Header.Height > 24696)
                    {
                        //check whether contain two or more same utxo in one block
                        var count = 0;
                        foreach (var tx in block.Transactions)
                        {
                            foreach (var i in tx.Inputs)
                            {
                                if (i.OutputTransactionHash == input.OutputTransactionHash && i.OutputIndex == input.OutputIndex)
                                {
                                    count++;
                                }
                            }
                        }

                        if (count > 1)
                        {
                            //LogHelper.Warn($"transaction.Hash:{transaction.Hash}");
                            //LogHelper.Warn($"input.OutputTransactionHash:{input.OutputTransactionHash}");
                            //LogHelper.Warn($"input.OutputIndex:{input.OutputIndex}");
                            throw new CommonException(ErrorCode.Engine.Transaction.Verify.UTXO_DUPLICATED_IN_ONE_BLOCK);
                        }

                        if (this.checkOutputSpent(transaction.Hash, input.OutputTransactionHash, input.OutputIndex, block.Header.Hash))
                        {
                            //LogHelper.Warn($"transaction.Hash:{transaction.Hash}");
                            //LogHelper.Warn($"input.OutputTransactionHash:{input.OutputTransactionHash}");
                            //LogHelper.Warn($"input.OutputIndex:{input.OutputIndex}");
                            throw new CommonException(ErrorCode.Engine.Transaction.Verify.UTXO_HAS_BEEN_SPENT);
                        }
                    }
                    else if (block == null)
                    {
                        //check whether contain two or more same utxo in one transaction
                        var count = transaction.Inputs.Where(i => i.OutputTransactionHash == input.OutputTransactionHash && i.OutputIndex == input.OutputIndex).Count();

                        if (count > 1)
                        {
                            //LogHelper.Warn($"transaction.Hash:{transaction.Hash}");
                            //LogHelper.Warn($"input.OutputTransactionHash:{input.OutputTransactionHash}");
                            //LogHelper.Warn($"input.OutputIndex:{input.OutputIndex}");
                            throw new CommonException(ErrorCode.Engine.Transaction.Verify.UTXO_DUPLICATED_IN_ONE_BLOCK);
                        }

                        if (this.checkOutputSpent(transaction.Hash, input.OutputTransactionHash, input.OutputIndex, null))
                        {
                            //LogHelper.Warn($"transaction.Hash:{transaction.Hash}");
                            //LogHelper.Warn($"input.OutputTransactionHash:{input.OutputTransactionHash}");
                            //LogHelper.Warn($"input.OutputIndex:{input.OutputIndex}");
                            throw new CommonException(ErrorCode.Engine.Transaction.Verify.UTXO_HAS_BEEN_SPENT);
                        }
                    }

                    if (!Script.VerifyLockScriptByUnlockScript(input.OutputTransactionHash, input.OutputIndex, lockScript, input.UnlockScript))
                    {
                        throw new CommonException(ErrorCode.Engine.Transaction.Verify.UTXO_UNLOCK_FAIL);
                    }

                    totalInput += amount;
                }
                else
                {
                    //not found output, wait for other transactions or blocks;
                    txFee = 0;
                    return false;
                }
            }

            if (totalOutput >= totalInput)
            {
                throw new CommonException(ErrorCode.Engine.Transaction.Verify.OUTPUT_LARGE_THAN_INPUT);
            }

            if ((totalInput - totalOutput) < BlockSetting.TRANSACTION_MIN_FEE)
            {
                throw new CommonException(ErrorCode.Engine.Transaction.Verify.TRANSACTION_FEE_IS_TOO_FEW);
            }

            if (totalInput > BlockSetting.INPUT_AMOUNT_MAX)
            {
                throw new CommonException(ErrorCode.Engine.Transaction.Verify.INPUT_EXCEEDED_THE_LIMIT);
            }

            if (totalOutput > BlockSetting.OUTPUT_AMOUNT_MAX)
            {
                throw new CommonException(ErrorCode.Engine.Transaction.Verify.OUTPUT_EXCEEDED_THE_LIMIT);
            }

            txFee = totalInput - totalOutput;
            return true;
        }

        public bool VerifyTransactionMsg(VerifyTransactionModel model, out Transaction tranx)
        {
            var transaction = model.transaction;

            //校验锁定时间
            if (transaction.Locktime > 0 && transaction.ExpiredTime == transaction.Locktime)
            {
                transaction.ExpiredTime = 0;
            }

            //校验HASH值
            if (transaction.Hash != transaction.GetHash())
            {
                throw new CommonException(ErrorCode.Engine.Transaction.Verify.TRANSACTION_HASH_ERROR);
            }

            //交易必须包含输入和输出
            if (transaction.InputCount == 0 || transaction.OutputCount == 0)
            {
                throw new CommonException(ErrorCode.Engine.Transaction.Verify.INPUT_AND_OUTPUT_CANNOT_BE_EMPTY);
            }

            //校验交易的时间
            if (transaction.Locktime < 0 || transaction.Locktime > (Time.EpochTime + BlockSetting.LOCK_TIME_MAX))
            {
                throw new CommonException(ErrorCode.Engine.Transaction.Verify.LOCK_TIME_EXCEEDED_THE_LIMIT);
            }

            //校验交易量
            if (transaction.Serialize().Length < BlockSetting.TRANSACTION_MIN_SIZE)
            {
                throw new CommonException(ErrorCode.Engine.Transaction.Verify.TRANSACTION_SIZE_BELOW_THE_LIMIT);
            }

            return VerifyTransactionData(model, out tranx);
        }

        readonly long[] Heights = new long[] { 19288, 19306, 19314, 19329 };
        const string COINBASE_INPUT_HASH = "0000000000000000000000000000000000000000000000000000000000000000";

        public bool VerifyTransactionData(VerifyTransactionModel model, out Transaction tranx)
        {
            long totalOutput = 0;
            long totalInput = 0;
            
            var transaction = model.transaction;
            var block = model.block;
            var localHeight = model.localHeight;

            List<Output> outputEntities = new List<Output>();
            List<Input> inputEntities = new List<Input>();
            
            #region 校验和转换 接收地址
            foreach (var output in transaction.Outputs)
            {
                if ((output.Amount <= 0 || output.Amount > BlockSetting.OUTPUT_AMOUNT_MAX) && !Heights.Contains(block.Header.Height))
                {
                    throw new CommonException(ErrorCode.Engine.Transaction.Verify.OUTPUT_EXCEEDED_THE_LIMIT);
                }

                if (!Script.VerifyLockScriptFormat(output.LockScript))
                {
                    throw new CommonException(ErrorCode.Engine.Transaction.Verify.SCRIPT_FORMAT_ERROR);
                }

                var outputEntity = output.ConvertToEntiry(transaction, block);
                outputEntities.Add(outputEntity);

                totalOutput += output.Amount;
            }
            #endregion
            
            #region 判断是不是在一个区块内重复花费（24696之前有错误数据）
            if (block.Header.Height <= 24696)
            {
                var inputsCount = transaction.Inputs.Select(x => x.OutputTransactionHash + x.OutputIndex).Distinct().Count();
                if (inputsCount < transaction.Inputs.Count)
                {
                    throw new CommonException(ErrorCode.Engine.Transaction.Verify.UTXO_NOT_EXISTED);
                }
            }
            #endregion
            
            
            var firstInput = transaction.Inputs[0];
            bool isCoinbase = firstInput.OutputTransactionHash == COINBASE_INPUT_HASH;
            if (isCoinbase)
            {
                #region 如果是Coinbase
                var inputps = new InputExtensionParams();
                inputps.BlockHash = block.Header.Hash;
                inputps.InputAccountId = null;
                inputps.InputAmount = 0;
                inputps.TransactionHash = transaction.Hash;
                var inputEntity = firstInput.ConvertToEntiry(inputps);

                totalInput += 0;
                inputEntities.Add(inputEntity);
                isCoinbase = true;
                #endregion
            }
            else
            {
                foreach (var input in transaction.Inputs)
                {
                    if (!Script.VerifyUnlockScriptFormat(input.UnlockScript))
                    {
                        throw new CommonException(ErrorCode.Engine.Transaction.Verify.SCRIPT_FORMAT_ERROR);
                    }

                    var output = model.outputs.FirstOrDefault(x => x.TransactionHash == input.OutputTransactionHash && x.Index == input.OutputIndex);
                    if (output == null)
                    {
                        throw new CommonException(ErrorCode.Engine.Transaction.Verify.UTXO_NOT_EXISTED);
                    }

                    //是否已经上链了
                    if (output.Spent && block.Header.Height > 24696)
                    {
                        throw new CommonException(ErrorCode.Engine.Transaction.Verify.UTXO_HAS_BEEN_SPENT);
                    }

                    var constraint = model.constraints.FirstOrDefault(x => x.TransactionHash == output.TransactionHash);
                    //判断是不是转账得交易
                    if (constraint != null)
                    {
                        long blockHeight = constraint.Height;
                        //判断挖矿的区块等待100个确认
                        if (constraint.IsCoinBase && (blockHeight < 0 || (localHeight - blockHeight < 100)))
                        {
                            throw new CommonException(ErrorCode.Engine.Transaction.Verify.COINBASE_NEED_100_CONFIRMS);
                        }
                        //判断余额是否已经解锁
                        if (Time.EpochTime < constraint.LockTime)
                        {
                            throw new CommonException(ErrorCode.Engine.Transaction.Verify.TRANSACTION_IS_LOCKED);
                        }
                    }

                    string lockScript = output.LockScript;

                    if (!Script.VerifyLockScriptByUnlockScript(input.OutputTransactionHash, input.OutputIndex, lockScript, input.UnlockScript))
                    {
                        throw new CommonException(ErrorCode.Engine.Transaction.Verify.UTXO_UNLOCK_FAIL);
                    }
                    var inputps = new InputExtensionParams();
                    inputps.BlockHash = block.Header.Hash;
                    inputps.InputAccountId = output.ReceiverId;
                    inputps.InputAmount = output.Amount;
                    inputps.TransactionHash = transaction.Hash;
                    var inputEntity = input.ConvertToEntiry(inputps);

                    totalInput += output.Amount;

                    inputEntities.Add(inputEntity);
                }

                if (totalOutput >= totalInput)
                {
                    throw new CommonException(ErrorCode.Engine.Transaction.Verify.OUTPUT_LARGE_THAN_INPUT);
                }

                if ((totalInput - totalOutput) < BlockSetting.TRANSACTION_MIN_FEE)
                {
                    throw new CommonException(ErrorCode.Engine.Transaction.Verify.TRANSACTION_FEE_IS_TOO_FEW);
                }
            }

            if (totalInput > BlockSetting.INPUT_AMOUNT_MAX)
            {
                throw new CommonException(ErrorCode.Engine.Transaction.Verify.INPUT_EXCEEDED_THE_LIMIT);
            }

            if (totalOutput > BlockSetting.OUTPUT_AMOUNT_MAX)
            {
                throw new CommonException(ErrorCode.Engine.Transaction.Verify.OUTPUT_EXCEEDED_THE_LIMIT);
            }

            var ps = new TransExtensionParams();
            ps.BlockHash = block.Header.Hash;
            ps.Inputs = inputEntities;
            ps.Outputs = outputEntities;
            ps.TotalInput = totalInput;
            ps.TotalOutput = totalOutput;
            ps.Height = block.Header.Height;
            tranx = transaction.ConvertToEntity(ps, isCoinbase);
            return true;
        }

        public List<string> GetAllHashesFromPool()
        {
            return TransactionPool.Instance.GetAllTransactionHashes();
        }

        public List<string> GetAllHashesRelevantWithCurrentWalletFromPool()
        {
            var accountIds = new AccountComponent().GetAllAccounts().Select(a => a.Id).ToList();
            var txs = TransactionPool.Instance.GetAllTransactions();
            var result = new List<string>();

            foreach(var tx in txs)
            {
                bool isOK = false;
                var entity = tx.ConvertTxMsgToEntity(false);

                foreach(var input in entity.Inputs)
                {
                    if(accountIds.Contains(input.AccountId))
                    {
                        result.Add(tx.Hash);
                        isOK = true;
                    }
                }

                if(!isOK)
                {
                    foreach(var output in entity.Outputs)
                    {
                        result.Add(tx.Hash);
                        isOK = true;
                    }
                }
            }

            return result;
        }

        public bool CheckTxExisted(string txHash, bool checkPool = true)
        {
            var dac = TransactionDac.Default;
                        
            if(checkPool)
            {
                var txMsg =  TransactionPool.Instance.SearchByTransactionHash(txHash);
                if(txMsg != null)
                {
                    return true;
                }                
            }            
            return dac.TransactionHashExist(txHash);
        }

        public bool CheckBlackTxExisted(string txHash)
        {
            return BlacklistTxs.Current.IsBlacked(txHash);
        }

        public TransactionMsg GetTransactionMsgByHash(string txHash)
        {
            var txDac = TransactionDac.Default;
            var entity = txDac.SelectByHash(txHash);

            if (entity != null)
            {
                return this.ConvertTxEntityToMsg(entity);
            }
            else
            {
                return GetTransactionMsgFromPool(txHash);
            }
        }

        public List<Transaction> GetTransactionEntitiesByBlockHash(string blockHash)
        {
            var txDac = TransactionDac.Default;
            return txDac.SelectByBlockHash(blockHash);
        }

        public TransactionMsg GetTransactionMsgFromDB(string txHash, out string blockHash)
        {
            var txDac = TransactionDac.Default;
            var entity = txDac.SelectByHash(txHash);
            blockHash = null;

            if(entity != null)
            {
                blockHash = entity.BlockHash;
                return this.ConvertTxEntityToMsg(entity);
            }
            else
            {
                return null;
            }
        }

        public TransactionMsg GetTransactionMsgFromPool(string txHash)
        {
            return TransactionPool.Instance.GetTransactionByHashes(new string[] { txHash }).FirstOrDefault();
        }

        public IEnumerable<TransactionMsg> GetTransactionMsgsFromPool(IEnumerable<string> txHashes)
        {
            return TransactionPool.Instance.GetTransactionByHashes(txHashes);
        }

        public List<Transaction> GetTransactionEntitiesContainUnspentUTXO()
        {
            var items = TransactionDac.Default.SelectTransactionsContainUnspentUTXO();
            return items;
            //var result = new List<TransactionMsg>();

            //foreach(var item in items)
            //{
            //    result.Add(this.convertTxEntityToMsg(item));
            //}

            //return result;
        }

        public List<Output> GetOutputEntitesByTxHash(string txHash)
        {
            return OutputDac.Default.SelectByTransactionHash(txHash);
        }

        public List<Output> GetOutputEntitesContainUnspentUTXOByTxHash(long lockTime)
        {
            return OutputDac.Default.GetOutputEntitesContainUnspentUTXOByTxHash(lockTime);
        }

        public PageUnspent GetOutputEntitesContainUnspentUTXOByTxHash(long minAmount, long maxAmount, int currentPage, int pageSize, bool isDesc, long lockTime, long minConfirmations, long maxConfirmations, long latestHeight)
        {
            OutputDac dac = OutputDac.Default;
            List<OutputConfirmInfo> outputs = dac.GetOutputEntitesContainUnspentUTXOByTxHash(minAmount, maxAmount, currentPage, pageSize, isDesc, lockTime, minConfirmations, maxConfirmations, latestHeight);
            long count = dac.GetOutputEntitesContainUnspentUTXOCount(minAmount, maxAmount, lockTime);
            return new PageUnspent { Outputs = outputs, Count = count };
        }

        public PageUnspent GetOutputEntitesContainUnspentUTXOByTxHash(int currentPage, int pageSize, long lockTime, long latestHeight)
        {
            OutputDac dac = OutputDac.Default;
            List<OutputConfirmInfo> outputs = dac.GetOutputEntitesContainUnspentUTXOByTxHash(currentPage, pageSize, lockTime, latestHeight);
            long count = dac.GetOutputEntitesContainUnspentUTXOCount(0, Int64.MaxValue, lockTime);
            return new PageUnspent { Outputs = outputs, Count = count };
        }

        /// <summary>
        /// UTXO合并拆分专用
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="lockTime"></param>
        /// <param name="latestHeight"></param>
        /// <param name="maxAmount"></param>
        /// <param name="minAmount"></param>
        /// <returns></returns>
        public PageUnspent GetOutputEntitesContainUnspentUTXOByTxHash(int currentPage, int pageSize, long lockTime, long latestHeight, long maxAmount, long minAmount)
        {
            OutputDac dac = OutputDac.Default;
            List<OutputConfirmInfo> outputs = dac.GetOutputEntitesContainUnspentUTXOByTxHash(currentPage, pageSize, lockTime, latestHeight, maxAmount, minAmount);
            long count = dac.GetOutputEntitesContainUnspentUTXOCount(0, Int64.MaxValue, lockTime);
            return new PageUnspent { Outputs = outputs, Count = count };
        }

        public Output GetOutputEntiyByIndexAndTxHash(string txHash, int index)
        {
            return OutputDac.Default.SelectByHashAndIndex(txHash, index);
        }

        public Transaction GetTransactionEntityByHash(string hash)
        {
            var inputDac = InputDac.Default;
            var outputDac = OutputDac.Default;
            var item = TransactionDac.Default.SelectByHash(hash);

            if (item != null)
            {
                TaskQueue.AddWaitAction(() =>
                {
                    item.Inputs = inputDac.SelectByTransactionHash(item.Hash);
                    item.Outputs = outputDac.SelectByTransactionHash(item.Hash);
                }, "GetTransactionEntityByHash");
            }
            else
            {
                var msg = GetTransactionMsgFromPool(hash);

                if (msg != null)
                {
                    item = msg.ConvertTxMsgToEntity(false);
                }
            }

            return item;
        }

        public TransactionMsg ConvertTxEntityToMsg(Transaction tx)
        {
            var inputDac = InputDac.Default;
            var outputDac = OutputDac.Default;

            var txMsg = new TransactionMsg();
            txMsg.Version = tx.Version;
            txMsg.Hash = tx.Hash;
            txMsg.Timestamp = tx.Timestamp;
            txMsg.Locktime = tx.LockTime;

            var inputs = inputDac.SelectByTransactionHash(tx.Hash);
            var outputs = outputDac.SelectByTransactionHash(tx.Hash);

            foreach (var input in inputs)
            {
                txMsg.Inputs.Add(new InputMsg
                {
                    OutputTransactionHash = input.OutputTransactionHash,
                    OutputIndex = input.OutputIndex,
                    Size = input.Size,
                    UnlockScript = input.UnlockScript
                });
            }

            foreach (var output in outputs)
            {
                txMsg.Outputs.Add(new OutputMsg
                {
                    Index = output.Index,
                    Amount = output.Amount,
                    Size = output.Size,
                    LockScript = output.LockScript
                });
            }

            return txMsg;
        }

        public string GetTransactionHashByInputTx(string txHash, int index)
        {
            var txMsg = TransactionPool.Instance.GetTransactionByInputTx(txHash, index);

            if(txMsg == null)
            {
                var inputDac = InputDac.Default;
                var items = inputDac.SelectByOutputHash(txHash, index);

                if(items.Count > 0)
                {
                    return items[0].TransactionHash;
                }
            }
            else
            {
                return txMsg.Hash;
            }

            return null;
        }

        public void AddBlockedTxHash(string txHash)
        {
            BlacklistTxs.Current.Add(txHash);
        }

        //check whether output is existed. get amount and lockscript from output.
        private bool getOutput(string transactionHash, int outputIndex, out long outputAmount, out string lockScript, out long blockHeight)
        {
            var outputDac = OutputDac.Default;
            var outputEntity = outputDac.SelectByHashAndIndex(transactionHash, outputIndex);

            if (outputEntity == null)
            {
                long height = -1;
                var outputMsg = TransactionPool.Instance.GetOutputMsg(transactionHash, outputIndex);

                if (outputMsg != null)
                {
                    outputAmount = outputMsg.Amount;
                    lockScript = outputMsg.LockScript;
                    blockHeight = height;
                    return true;
                }
            }
            else
            {
                outputAmount = outputEntity.Amount;
                lockScript = outputEntity.LockScript;
                var blockEntity = BlockDac.Default.SelectByHash(outputEntity.BlockHash);

                if(blockEntity != null)
                {
                    blockHeight = blockEntity.Height;
                    return true;
                }
            }

            outputAmount = 0;
            lockScript = null;
            blockHeight = -1;
            return false;
        }

        //Check whether output has been spent or contained in another transaction, true = spent, false = unspent
        private bool checkOutputSpent(string currentTxHash, string outputTxHash, int outputIndex, string blockHash = null)
        {
            var outputDac = OutputDac.Default;
            var inputDac = InputDac.Default;
            var txDac = TransactionDac.Default;
            var blockDac = BlockDac.Default;

            var items = inputDac.SelectByOutputHash(outputTxHash, outputIndex);
            var tips = blockDac.SelectTipBlocks();

            foreach(var inputEntity in items)
            {
                if(inputEntity.TransactionHash != currentTxHash)
                {
                    if (tips.Count <= 1)
                    {
                        return true;
                    }
                    else
                    {
                        if(blockHash != null)
                        {
                            var prevBlocks = blockDac.SelectPreviousBlocks(blockHash, 6);

                            if(prevBlocks.Any(b=>b.Hash == inputEntity.BlockHash))
                            {
                                return true;
                            }

                            var blockEntity = blockDac.SelectByHash(inputEntity.BlockHash);

                            if (blockEntity != null && blockEntity.IsVerified)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool existsInDB(string transactionHash)
        {
            var transactionDac = TransactionDac.Default;

            if(transactionDac.HasTransactionByHash(transactionHash))
            {
                return true;
            }

            return false;
        }

        private bool existsInTransactionPool(string transactionHash)
        {
            if (TransactionPool.Instance.SearchByTransactionHash(transactionHash) != null)
            {
                return true;
            }

            return false;
        }

    }
}
