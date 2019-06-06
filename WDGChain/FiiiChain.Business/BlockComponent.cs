// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.Business.Extensions;
using FiiiChain.Business.ParamsModel;
using FiiiChain.Consensus;
using FiiiChain.Data;
using FiiiChain.Data.Accesses;
using FiiiChain.DataAgent;
using FiiiChain.Entities;
using FiiiChain.Entities.CacheModel;
using FiiiChain.Entities.ExtensionModels;
using FiiiChain.Framework;
using FiiiChain.IModules;
using FiiiChain.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FiiiChain.Business
{
    public class BlockComponent
    {
        public void BlockPoolInitialize()
        {
            //var blockDac = BlockDac.Default;

            //long lastBlockHeight = -1;
            //string lastBlockHash = Base16.Encode(HashHelper.EmptyHash());
            //var blockEntity = blockDac.SelectLast();

            //if (blockEntity != null)
            //{
            //    lastBlockHeight = blockEntity.Height;
            //    lastBlockHash = blockEntity.Hash;
            //}
        }

        public void RemoveInvalidTx()
        {
            BlackListExtension.BlackTxPoolItems();
        }

        /// <summary>
        /// 创建新的区块
        /// </summary>
        /// <param name="minerName"></param>
        /// <param name="generatorId"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public BlockMsg CreateNewBlock(string minerName, string generatorId, string remark = null, string accountId = null)
        {
            var accountDac = new AccountDac();
            var blockDac = BlockDac.Default;
            var outputDac = OutputDac.Default;
            var txDac = TransactionDac.Default;
            var txPool = TransactionPool.Instance;
            var txComponent = new TransactionComponent();
            var transactionMsgs = new List<TransactionMsg>();

            long lastBlockHeight = -1;
            string lastBlockHash = Base16.Encode(HashHelper.EmptyHash());
            long lastBlockBits = -1;
            string lastBlockGenerator = null;

            //获取最后一个区块
            var blockEntity = blockDac.SelectLast();

            if (blockEntity != null)
            {
                lastBlockHeight = blockEntity.Height;
                lastBlockHash = blockEntity.Hash;
                lastBlockBits = blockEntity.Bits;
                lastBlockGenerator = blockEntity.GeneratorId;
            }

            long totalSize = 0;
            long totalInput = 0;
            long totalOutput = 0;
            long totalAmount = 0;
            long totalFee = 0;

            long maxSize = BlockSetting.MAX_BLOCK_SIZE - (1 * 1024);

            //获取待打包的交易
            var txs = txPool.GetTxsWithoutRepeatCost(10, maxSize);

            var hashIndexs = new List<string>();
            foreach (var tx in txs)
            {
                totalSize += tx.Size;
                totalInput += tx.InputCount;
                totalOutput += tx.OutputCount;
                hashIndexs.AddRange(tx.Inputs.Select(x => x.OutputTransactionHash + x.OutputIndex));
                long totalOutputAmount = tx.Outputs.Sum(x => x.Amount);
                totalAmount += totalOutputAmount;
            }

            var totalInputAmount = InputDac.Default.SelectTotalAmount(hashIndexs);
            totalFee = totalInputAmount - totalAmount;

            transactionMsgs.AddRange(txs);

            var accounts = CacheManager.Default.Get<Account>(DataCatelog.Accounts);

            var minerAccount = accounts.OrderBy(x => x.Timestamp).FirstOrDefault();

            if (accountId != null)
            {
                var account = accounts.FirstOrDefault(x => x.Id == accountId);

                if (account != null && !string.IsNullOrWhiteSpace(account.PrivateKey))
                {
                    minerAccount = account;
                }
            }

            var minerAccountId = minerAccount.Id;
            BlockMsg newBlockMsg = new BlockMsg();
            BlockHeaderMsg headerMsg = new BlockHeaderMsg();
            headerMsg.Hash = Base16.Encode(HashHelper.EmptyHash());
            headerMsg.GeneratorId = generatorId;
            newBlockMsg.Header = headerMsg;
            headerMsg.Height = lastBlockHeight + 1;
            headerMsg.PreviousBlockHash = lastBlockHash;

            if (headerMsg.Height == 0)
            {
                minerAccountId = BlockSetting.GenesisBlockReceiver;
                remark = BlockSetting.GenesisBlockRemark;
            }
            
            Block prevBlockMsg = null;
            Block prevStepBlockMsg = null;

            if (blockEntity != null)
            {
                prevBlockMsg = blockEntity;
            }

            if (headerMsg.Height >= POC.DIFFIUCLTY_ADJUST_STEP)
            {
                prevStepBlockMsg = this.GetBlockEntiytByHeight(headerMsg.Height - POC.DIFFIUCLTY_ADJUST_STEP);

                //if (!GlobalParameters.IsTestnet && headerMsg.Height <= POC.DIFFICULTY_CALCULATE_LOGIC_ADJUST_HEIGHT)
                //{
                //    prevStepBlockMsg = this.GetBlockEntiytByHeight(headerMsg.Height - POC.DIFFIUCLTY_ADJUST_STEP - 1);
                //}
                //else
                //{
                //    prevStepBlockMsg = this.GetBlockEntiytByHeight(headerMsg.Height - POC.DIFFIUCLTY_ADJUST_STEP);
                //}
            }

            var newBlockReward = POC.GetNewBlockReward(headerMsg.Height);

            headerMsg.Bits = POC.CalculateBaseTarget(headerMsg.Height, prevBlockMsg, prevStepBlockMsg);
            headerMsg.TotalTransaction = transactionMsgs.Count + 1;

            var coinbaseTxMsg = new TransactionMsg();
            coinbaseTxMsg.Timestamp = Time.EpochTime;
            coinbaseTxMsg.Locktime = 0;

            var coinbaseInputMsg = new InputMsg();
            coinbaseTxMsg.Inputs.Add(coinbaseInputMsg);
            coinbaseInputMsg.OutputIndex = 0;
            coinbaseInputMsg.OutputTransactionHash = Base16.Encode(HashHelper.EmptyHash());
            coinbaseInputMsg.UnlockScript = Script.BuildMinerScript(minerName, remark);
            coinbaseInputMsg.Size = coinbaseInputMsg.UnlockScript.Length;

            var coinbaseOutputMsg = new OutputMsg();
            coinbaseTxMsg.Outputs.Add(coinbaseOutputMsg);
            coinbaseOutputMsg.Amount = newBlockReward + totalFee;
            coinbaseOutputMsg.LockScript = Script.BuildLockScipt(minerAccountId);
            coinbaseOutputMsg.Size = coinbaseOutputMsg.LockScript.Length;
            coinbaseOutputMsg.Index = 0;

            if (newBlockReward < 0 || totalFee < 0 || coinbaseOutputMsg.Amount < 0)
            {
                LogHelper.Warn($"newBlockReward:{newBlockReward}");
                LogHelper.Warn($"totalFee:{totalFee}");
                LogHelper.Warn($"coinbaseOutputMsg.Amount:{coinbaseOutputMsg.Amount}");
                throw new CommonException(ErrorCode.Engine.Transaction.Verify.COINBASE_OUTPUT_AMOUNT_ERROR);
            }

            coinbaseTxMsg.Hash = coinbaseTxMsg.GetHash();

            newBlockMsg.Transactions.Insert(0, coinbaseTxMsg);


            foreach (var tx in transactionMsgs)
            {
                newBlockMsg.Transactions.Add(tx);
            }

            headerMsg.PayloadHash = newBlockMsg.GetPayloadHash();
            headerMsg.BlockSignature = Base16.Encode(HashHelper.EmptyHash());
            headerMsg.BlockSigSize = headerMsg.BlockSignature.Length;
            headerMsg.TotalTransaction = newBlockMsg.Transactions.Count;
            return newBlockMsg;
        }

        /// <summary>
        /// 估算交易费率
        /// </summary>
        /// <returns></returns>
        public long EstimateSmartFee()
        {
            //对象初始化
            var txDac = TransactionDac.Default;
            var transactionMsgs = new List<TransactionMsg>();
            var txPool = TransactionPool.Instance;
            long totalSize = 0;
            long totalFee = 0;
            //设置最大上限
            long maxSize = BlockSetting.MAX_BLOCK_SIZE - (1 * 1024);
            //交易池中的项目按照费率从高到低排列
            List<TransactionPoolItem> poolItemList = txPool.MainPool.OrderByDescending(t => t.FeeRate).ToList();
            var index = 0;

            while (totalSize < maxSize && index < poolItemList.Count)
            {
                //获取totalFee和totalSize
                TransactionMsg tx = poolItemList[index].Transaction;
                //判断交易Hash是否在交易Msg中
                if (tx != null && transactionMsgs.Where(t => t.Hash == tx.Hash).Count() == 0)
                {
                    totalFee += Convert.ToInt64(poolItemList[index].FeeRate * tx.Serialize().LongLength / 1024.0);
                    if (txDac.SelectByHash(tx.Hash) == null)
                    {
                        transactionMsgs.Add(tx);
                        totalSize += tx.Size;
                    }
                    else
                    {
                        txPool.RemoveTransaction(tx.Hash);
                    }
                }
                /*
                else
                {
                    break;
                }
                */
                index++;
            }
            //获取费率
            if (poolItemList.Count == 0)
            {
                return 1024;
            }
            long feeRate = Convert.ToInt64(Math.Ceiling((totalFee / (totalSize / 1024.0)) / poolItemList.Count));
            if (feeRate < 1024)
            {
                feeRate = 1024;
            }
            return feeRate;
        }

        public bool SaveBlockIntoDB(BlockMsg msg, bool needVerify = true)
        {
            try
            {
                Block block = null;
                if (needVerify)
                {
                    ConvertBlock(msg, out block);
                }
                else
                {
                    block = this.convertBlockMsgToEntity(msg);
                    block.IsDiscarded = false;
                    block.IsVerified = false;
                }

                bool result = false;

                if (block == null)
                    return result;

                BlockDac blockDac = BlockDac.Default;
                if (blockDac.SelectByHash(block.Hash) != null)
                    return result;
                var saveResult = blockDac.Save(block);
                if (saveResult < 0)
                    return result;

                GlobalParameters.LocalHeight = block.Height;
                result = true;

                if (block.Transactions.Count > 1)
                {
                    var hashes = block.Transactions.Select(x => x.Hash);
                    TransactionPool.Instance.RemoveTransactions(hashes);
                }

                Task.Run(() =>
                {
                    var accounts = CacheManager.Default.GetAllKeys(DataCatelog.Accounts);

                    List<KeyValuePair<string, PaymentCache>> paymentPairs = new List<KeyValuePair<string, PaymentCache>>();
                    List<KeyValuePair<string, Output>> outputPairs = new List<KeyValuePair<string, Output>>();
                    List<string> spentUtxo = new List<string>();
                    foreach (var tx in block.Transactions)
                    {
                        try
                        {
                            foreach (var output in tx.Outputs)
                            {
                                if (accounts.Contains(output.ReceiverId))
                                {
                                    outputPairs.Add(new KeyValuePair<string, Output>(output.ToString(), output));
                                }
                            }

                            var payments = Converters.ConvertToSelfPayment(tx);
                            var paymentKeyValues = payments
                                .Select(x => new KeyValuePair<string, PaymentCache>(x.ToString(), x));
                            paymentPairs.AddRange(paymentKeyValues);

                            var addAmount = payments.Where(x => x.category == PaymentCatelog.Generate || x.category == PaymentCatelog.Receive).Sum(x => x.amount);
                            var subtractAmount = payments.Where(x => x.category == PaymentCatelog.Self).Sum(x => x.fee);
                            subtractAmount += payments.Where(x => x.category == PaymentCatelog.Send).Sum(x => x.fee + x.amount);

                            GlobalParameters.TotalAmount = GlobalParameters.TotalAmount + addAmount - subtractAmount;

                            spentUtxo.AddRange(tx.Inputs.Select(x => x.OutputTransactionHash + x.OutputIndex));
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error("Error in save cache", ex);
                        }
                    }

                    OutputDac.Default.UpdateSpentStatuses(spentUtxo);

                    CacheManager.Default.Put(DataCatelog.Output, outputPairs);
                    CacheManager.Default.Put(DataCatelog.Payment, paymentPairs);

                    var costItems = block.Transactions.SelectMany(x => x.Inputs)
                        .Select(item => $"{item.OutputTransactionHash}_{item.OutputIndex}").ToList();
                    //将区块信息添加到缓存
                    CacheManager.Default.Put(DataCatelog.Block, msg.ToString(), msg);
                    CacheManager.Default.Put(DataCatelog.BlockSimple, block.Height.ToString(), block.Hash);

                    TransactionPool.Instance.ClearCostUtxo(costItems);
                });

                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message, ex);
                throw ex;
            }
        }

        public bool VerifyBlock(BlockMsg newBlock)
        {
            if (newBlock.Header.Hash != newBlock.Header.GetHash())
            {
                throw new CommonException(ErrorCode.Engine.Block.Verify.BLOCK_HASH_ERROR);
            }

            bool hasPreviousBlock = false;
            long previousBlockTimestamp = -1;
            long previousBlockBits = -1;
            long previousBlockHeight = newBlock.Header.Height - 1;

            var cacheBlockKey = $"{newBlock.Header.Hash}_{newBlock.Header.Height - 1}";

            var previousBlockMsg = CacheManager.Default.Get<BlockMsg>(DataCatelog.Block, cacheBlockKey);
            
            if (previousBlockMsg != null)
            {
                hasPreviousBlock = true;
                previousBlockTimestamp = previousBlockMsg.Header.Timestamp;
                previousBlockBits = previousBlockMsg.Header.Bits;
            }
            else
            {
                var previousBlock = this.GetBlockEntiytByHash(newBlock.Header.PreviousBlockHash);
                hasPreviousBlock = previousBlock != null;
                if (hasPreviousBlock)
                {
                    previousBlockTimestamp = previousBlock.Timestamp;
                    previousBlockBits = previousBlock.Bits;
                }
            }

            if (newBlock.Header.Height > 0 && !hasPreviousBlock)
            {
                throw new CommonException(ErrorCode.Engine.Block.Verify.PREV_BLOCK_NOT_EXISTED);
            }

            if ((newBlock.Header.Timestamp - Time.EpochTime) > 2 * 60 * 60 * 1000 ||
                (hasPreviousBlock && newBlock.Header.Timestamp <= previousBlockTimestamp))
            {
                throw new CommonException(ErrorCode.Engine.Block.Verify.BLOCK_TIME_IS_ERROR);
            }

            if (newBlock.Serialize().Length > BlockSetting.MAX_BLOCK_SIZE)
            {
                throw new CommonException(ErrorCode.Engine.Block.Verify.BLOCK_SIZE_LARGE_THAN_LIMIT);
            }

            Block prevStepBlock = null;
            if (newBlock.Header.Height >= POC.DIFFIUCLTY_ADJUST_STEP)
            {
                prevStepBlock = this.GetBlockEntiytByHeight(newBlock.Header.Height - POC.DIFFIUCLTY_ADJUST_STEP);

                //if (!GlobalParameters.IsTestnet &&
                //    newBlock.Header.Height <= POC.DIFFICULTY_CALCULATE_LOGIC_ADJUST_HEIGHT)
                //{
                //    prevStepBlock =
                //        this.GetBlockEntiytByHeight(newBlock.Header.Height - POC.DIFFIUCLTY_ADJUST_STEP - 1);
                //}
                //else
                //{
                //    prevStepBlock = this.GetBlockEntiytByHeight(newBlock.Header.Height - POC.DIFFIUCLTY_ADJUST_STEP);
                //}
            }

            //区块必须包含交易，否则错误
            if (newBlock.Transactions == null || newBlock.Transactions.Count == 0)
            {
                throw new CommonException(ErrorCode.Engine.Transaction.Verify.NOT_FOUND_COINBASE);
            }

            //第一个一定是coinbase,奖励+手续费
            var coinbase = newBlock.Transactions[0];
            //校验打包区块的矿池是否经过验证
            var minerInfo = Encoding.UTF8.GetString(Base16.Decode(coinbase.Inputs[0].UnlockScript)).Split("`")[0];
            var pool = MiningPoolComponent.CurrentMiningPools.FirstOrDefault(x => x.Name == minerInfo);
            if (pool == null)
            {
                throw new CommonException(ErrorCode.Engine.Block.Verify.MINING_POOL_NOT_EXISTED);
            }

            //校验区块的签名信息
            if (!POC.VerifyBlockSignature(newBlock.Header.PayloadHash, newBlock.Header.BlockSignature, pool.PublicKey))
            {
                throw new CommonException(ErrorCode.Engine.Block.Verify.BLOCK_SIGNATURE_IS_ERROR);
            }

            if (POC.CalculateBaseTarget(newBlock.Header.Height, previousBlockBits, previousBlockTimestamp,
                    prevStepBlock) != newBlock.Header.Bits)
            {
                throw new CommonException(ErrorCode.Engine.Block.Verify.BITS_IS_WRONG);
            }
            
            var targetResult = POC.CalculateTargetResult(newBlock);
            
            if (POC.Verify(newBlock.Header.Bits, targetResult))
            {
                return true;
            }
            else
            {
                throw new CommonException(ErrorCode.Engine.Block.Verify.POC_VERIFY_FAIL);
            }
        }

        public bool ConvertBlock(BlockMsg newBlock, out Block block)
        {
            block = null;
            //校验区块的基本信息
            var result = VerifyBlock(newBlock);

            if (!result)
                return false;

            var txComponent = new TransactionComponent();
            var blockComponent = new BlockComponent();

            //校验交易信息
            var totalFee = 0L;
            List<Transaction> transactions = new List<Transaction>();
            List<Output> outputs = new List<Output>();
            //跳过coinbase,获取所有的输入
            var inputs = newBlock.Transactions.Skip(1).SelectMany(x => x.Inputs);
            #region 初始化需要的信息
            var hashIndexs = inputs.Select(x => x.OutputTransactionHash + x.OutputIndex);
            var hashes = inputs.Select(x => x.OutputTransactionHash).Distinct();
            List<BlockConstraint> constraints = new List<BlockConstraint>();

            if (hashIndexs.Any())
            {
                if (hashIndexs.Count() < hashIndexs.Distinct().Count())
                {
                    throw new CommonException(ErrorCode.Engine.Transaction.Verify.UTXO_HAS_BEEN_SPENT);
                }
                //Action initDataAction = () =>
                //{
                    hashIndexs = hashIndexs.Distinct();
                    outputs = OutputDac.Default.SelectUnspentByHashAndIndexs(hashIndexs);
                    if (outputs == null || outputs.Count() < hashIndexs.Count())
                    {
                        throw new CommonException(ErrorCode.Engine.Transaction.Verify.UTXO_NOT_EXISTED);
                    }

                    constraints = BlockDac.Default.GetConstraintsByBlockHashs(hashes);
                    if (constraints == null || constraints.Count < hashes.Count())
                    {
                        throw new CommonException(ErrorCode.Engine.Transaction.Verify.UTXO_NOT_EXISTED);
                    }
                //};

                //try
                //{
                //    TaskQueue.AddWaitAction(initDataAction, "initDataAction");
                //}
                //catch (Exception ex)
                //{
                //    throw ex;
                //}
            }
            #endregion

            VerifyTransactionModel model = new VerifyTransactionModel();
            model.block = newBlock;
            model.outputs = outputs;
            model.constraints = constraints;
            model.localHeight = blockComponent.GetLatestHeight();
            foreach (var item in newBlock.Transactions)
            {
                Transaction transaction;
                model.transaction = item;
                if (txComponent.VerifyTransactionMsg(model, out transaction) && transaction != null)
                {
                    totalFee += transaction.Fee;
                    transactions.Add(transaction);
                }
                else
                {
                    return false;
                }
            }
            block = newBlock.ConvertToEntity(transactions);

            var newBlockReward = POC.GetNewBlockReward(block.Height);
            var coinbaseAmount = block.Transactions[0].Outputs[0].Amount;
            if (coinbaseAmount < 0 || coinbaseAmount != (block.TotalFee + newBlockReward))
            {
                throw new CommonException(ErrorCode.Engine.Transaction.Verify.COINBASE_OUTPUT_AMOUNT_ERROR);
            }
            return true;
        }

        public long GetLatestHeight()
        {
            if (GlobalParameters.LocalHeight < 0)
            {
                var dac = BlockDac.Default;
                var block = dac.SelectLast();

                if (block != null)
                {
                    GlobalParameters.LocalHeight = block.Height;
                    if (GlobalParameters.LatestBlockTime == 0)
                    { 
                        GlobalParameters.LatestBlockTime = block.Timestamp;
                    }
                }
            }

            return GlobalParameters.LocalHeight;
        }

        public long GetLatestConfirmedHeight()
        {
            if (GlobalParameters.LocalConfirmedHeight < 0)
            {
                var dac = BlockDac.Default;
                var block = dac.SelectLastConfirmed();

                if (block != null)
                {
                    GlobalParameters.LocalConfirmedHeight = block.Height;
                }
            }

            return GlobalParameters.LocalConfirmedHeight;
        }

        public List<BlockHeaderMsg> GetBlockHeaderMsgByHeights(List<long> heights)
        {
            var blockDac = BlockDac.Default;
            var txDac = TransactionDac.Default;
            var headers = new List<BlockHeaderMsg>();

            foreach (var height in heights)
            {
                var blockHash = CacheManager.Default.Get<string>(DataCatelog.BlockSimple, height.ToString());
                string blockKey = string.IsNullOrEmpty(blockHash) ? null : $"{height}_{blockHash}";
                
                if (string.IsNullOrEmpty(blockKey))
                {
                    Func<string, bool> filterKey = key => key.StartsWith(height + "_");
                    var headerHashes = CacheManager.Default.GetAllKeys(DataCatelog.Header, x => filterKey(x));

                    if (headerHashes.Any() && headerHashes.Count() == 1)
                    {
                        blockKey = headerHashes.First();
                    }
                }

                if (!string.IsNullOrEmpty(blockKey))
                {
                    LogHelper.Debug("Load headers from cache");
                    var headerMsg =
                        CacheManager.Default.Get<BlockHeaderMsg>(DataCatelog.Header, blockKey);

                    LogHelper.Warn(
                        $"Find Header Height[{height}] Hash In Cache {(headerMsg == null ? "成功" : "失败")}");

                    if (headerMsg != null)
                    {
                        headers.Add(headerMsg);
                        continue;
                    }
                }


                var items = blockDac.SelectByHeight(height);
                var headerMsgs = new List<BlockHeaderMsg>();

                foreach (var entity in items)
                {
                    var header = new BlockHeaderMsg();
                    header.Version = entity.Version;
                    header.Hash = entity.Hash;
                    header.Height = entity.Height;
                    header.PreviousBlockHash = entity.PreviousBlockHash;
                    header.Bits = entity.Bits;
                    header.Nonce = entity.Nonce;
                    header.Timestamp = entity.Timestamp;
                    header.GeneratorId = entity.GeneratorId;
                    header.PayloadHash = entity.PayloadHash;
                    header.BlockSignature = entity.BlockSignature;
                    header.BlockSigSize = entity.BlockSignature.Length;

                    var transactions = txDac.SelectByBlockHash(entity.Hash);
                    header.TotalTransaction = entity.Transactions == null ? 0 : entity.Transactions.Count;
                    //header.TotalTransaction = entity.Transactions == null ? 0 : entity.Transactions.Count;

                    headerMsgs.Add(header);
                    if (headerMsgs.Count > 0)
                    {
                        Task.Run(() =>
                        {
                            var keyValues = headerMsgs.Select(x =>
                                new KeyValuePair<string, BlockHeaderMsg>(x.GetKey(), x));
                            CacheManager.Default.Put(DataCatelog.Header, keyValues);
                            var hashSimples =
                                headerMsgs.Select(x => new KeyValuePair<string, string>(x.GetKey(), x.Hash));
                            CacheManager.Default.Put(DataCatelog.BlockSimple, hashSimples);

                            LogHelper.Debug("Set headers into cache");
                        });
                        headers.AddRange(headerMsgs);
                    }

                    LogHelper.Debug("Load headers from Sqlite");
                }
            }

            return headers;
        }

        public List<BlockMsg> GetBlockMsgByHeights(List<long> heights)
        {
            var blockDac = BlockDac.Default;
            var blocks = new List<BlockMsg>();
            List<long> notfoundHeights = new List<long>();
            List<BlockMsg> dbBlockMsgs = new List<BlockMsg>();
            foreach (var height in heights)
            {
                var blockHash = CacheManager.Default.Get<string>(DataCatelog.BlockSimple, height.ToString());

                if (string.IsNullOrEmpty(blockHash))
                {
                    notfoundHeights.Add(height);
                }

                var key = $"{blockHash}_{height}";
                var blockMsg = CacheManager.Default.Get<BlockMsg>(DataCatelog.Block, key);
                if (blockMsg != null)
                {
                    blocks.Add(blockMsg);
                }
                else
                {
                    notfoundHeights.Add(height);
                }
            }

            if (notfoundHeights.Any())
            {
                var matches = notfoundHeights.Select(x => "_" + x + ";");
                var blockKeys = CacheManager.Default.GetAllKeys(DataCatelog.Block,
                    key => matches.Any(match => key.Contains(match)));

                Dictionary<string, string> repairKeys = new Dictionary<string, string>();
                foreach (var blockKey in blockKeys)
                {
                    var blockMsg = CacheManager.Default.Get<BlockMsg>(DataCatelog.Block, blockKey);
                    if (blockMsg != null)
                    {
                        blocks.Add(blockMsg);
                        if (!repairKeys.ContainsKey(blockMsg.Header.Height.ToString()))
                        {
                            repairKeys.Add(blockMsg.Header.Height.ToString(), blockMsg.Header.Hash);
                        }

                        notfoundHeights.Remove(blockMsg.Header.Height);
                    }
                }

                if (repairKeys.Any())
                {
                    Task.Run(() => { CacheManager.Default.Put(DataCatelog.BlockSimple, repairKeys); });
                }
            }

            var dbBlocks = blockDac.SelectByHeights(notfoundHeights);

            //LogHelper.Warn($"LoadBlockInCache Height[{(string.Join(",", blocks.Select(x => x.Header.Height)))}]");
            
            foreach (Block dbBlock in dbBlocks)
            {
                dbBlockMsgs.Add(this.ConvertEntityToBlockMsg(dbBlock));
            }
            
            if (dbBlockMsgs.Count > 0)
            {
                Task.Run(() =>
                {
                    //将区块数据写入缓存
                    var blockDics = dbBlockMsgs.Select(x => new KeyValuePair<string, BlockMsg>(x.ToString(), x));
                    CacheManager.Default.Put(DataCatelog.Block, blockDics);
                    CacheManager.Default.Put(DataCatelog.BlockSimple,
                        dbBlockMsgs.Select(x =>
                            new KeyValuePair<string, string>(x.Header.Height.ToString(), x.Header.Hash)));
                    LogHelper.Debug("Set blocks into cache");
                });
                blocks.AddRange(dbBlockMsgs);
            }

            return blocks;
        }

        public BlockMsg GetBlockMsgByHeight(long height)
        {
            var blockDac = BlockDac.Default;
            var txDac = TransactionDac.Default;
            BlockMsg block = null;

            var items = blockDac.SelectByHeight(height);

            if (items.Count > 0)
            {
                block = this.ConvertEntityToBlockMsg(items[0]);
            }

            return block;
        }

        public BlockMsg GetBlockMsgByHash(string hash)
        {
            var blockDac = BlockDac.Default;
            var txDac = TransactionDac.Default;
            BlockMsg block = null;

            var entity = blockDac.SelectByHash(hash);

            if (entity != null)
            {
                block = this.ConvertEntityToBlockMsg(entity);
            }

            return block;
        }
        public BlockHeaderMsg GetBlockHeaderMsgByHash(string hash)
        {
            var blockDac = BlockDac.Default;
            var txDac = TransactionDac.Default;
            BlockHeaderMsg header = null;

            var entity = blockDac.SelectByHash(hash);

            if (entity != null)
            {
                header = new BlockHeaderMsg();
                header.Version = entity.Version;
                header.Hash = entity.Hash;
                header.Height = entity.Height;
                header.PreviousBlockHash = entity.PreviousBlockHash;
                header.Bits = entity.Bits;
                header.Nonce = entity.Nonce;
                header.Timestamp = entity.Timestamp;
                header.GeneratorId = entity.GeneratorId;
                //header.GenerationSignature = entity.GenerationSignature;
                //header.BlockSignature = entity.BlockSignature;
                //header.CumulativeDifficulty = entity.CumulativeDifficulty;
                header.PayloadHash = entity.PayloadHash;
                header.BlockSignature = entity.BlockSignature;
                header.BlockSigSize = entity.BlockSignature.Length;

                var transactions = txDac.SelectByBlockHash(entity.Hash);
                header.TotalTransaction = entity.Transactions == null ? 0 : entity.Transactions.Count;
            }

            return header;
        }

        public List<Block> GetPreviousBlocks(long lastHeight, int blockCount)
        {
            return BlockDac.Default.SelectPreviousBlocks(lastHeight, blockCount);
        }

        public Block GetBlockEntiytByHash(string hash)
        {
            return BlockDac.Default.SelectByHash(hash);
        }

        public Block GetBlockEntiytByHeight(long height)
        {
            var blockDac = BlockDac.Default;
            var items = blockDac.SelectByHeight(height);

            if (items.Count > 0)
            {
                return items[0];
            }

            return null;
        }

        public bool CheckBlockExists(string hash)
        {
            return (BlockDac.Default).BlockHashExist(hash);
        }

        public bool CheckConfirmedBlockExists(long height)
        {
            var result = (BlockDac.Default).SelectByHeight(height);

            if (result.Count > 0 && result[0].IsVerified)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Dictionary<Block, long> GetChainTips()
        {
            var dac = BlockDac.Default;
            var blocks = dac.SelectTipBlocks();

            var dict = new Dictionary<Block, long>();

            if (blocks.Count == 1)
            {
                dict.Add(blocks[0], 0);
            }
            else
            {
                var maxLength = 0;
                Block maxBlock = null;
                foreach (var block in blocks)
                {
                    var len = dac.CountBranchLength(block.Hash);

                    if (len > maxLength)
                    {
                        len = maxLength;
                        maxBlock = block;
                    }

                    dict.Add(block, len);
                }

                if (maxBlock != null && dict.ContainsKey(maxBlock))
                {
                    dict[maxBlock] = 0;
                }
            }

            return dict;
        }

        public string GetMiningWorkResult(BlockMsg block)
        {
            var listBytes = new List<Byte>();
            listBytes.AddRange(Base16.Decode(block.Header.PayloadHash));
            listBytes.AddRange(BitConverter.GetBytes(block.Header.Height));
            var genHash = Sha3Helper.Hash(listBytes.ToArray());
            //POC.CalculateScoopData(block.Header., block.Header.Nonce);




            var blockData = new List<byte>();

            foreach (var tx in block.Transactions)
            {
                blockData.AddRange(tx.Serialize());
            }

            var nonceBytes = BitConverter.GetBytes(block.Header.Nonce);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(nonceBytes);
            }

            blockData.AddRange(nonceBytes);
            var result = Base16.Encode(
                HashHelper.Hash(
                    blockData.ToArray()
                ));

            return result;
        }

        public Block convertBlockMsgToEntity(BlockMsg blockMsg,List<Output> outputDic = null)
        {
            OutputDac outputDac = OutputDac.Default;

            var block = new Block();
            block.Hash = blockMsg.Header.Hash;
            block.Version = blockMsg.Header.Version;
            block.Height = blockMsg.Header.Height;
            block.PreviousBlockHash = blockMsg.Header.PreviousBlockHash;
            block.Bits = blockMsg.Header.Bits;
            block.Nonce = blockMsg.Header.Nonce;
            block.GeneratorId = blockMsg.Header.GeneratorId;
            block.Timestamp = blockMsg.Header.Timestamp;
            block.TotalAmount = 0;
            block.TotalFee = 0;

            block.Transactions = new List<Transaction>();
            
            if (outputDic == null)
            {
                var hashIndexs = blockMsg.Transactions.SelectMany(x => x.Inputs).Select(x => x.OutputTransactionHash + x.OutputIndex);
                outputDic = outputDac.SelectByHashAndIndexs(hashIndexs);
            }

            foreach (var txMsg in blockMsg.Transactions)
            {
                var transaction = new Transaction();

                transaction.Hash = txMsg.Hash;
                transaction.BlockHash = block.Hash;
                transaction.Version = txMsg.Version;
                transaction.Timestamp = txMsg.Timestamp;
                transaction.LockTime = txMsg.Locktime;
                transaction.Inputs = new List<Input>();
                transaction.Outputs = new List<Output>();

                long totalInput = 0L;
                long totalOutput = 0L;

                foreach (var inputMsg in txMsg.Inputs)
                {
                    var input = new Input();
                    input.TransactionHash = txMsg.Hash;
                    input.OutputTransactionHash = inputMsg.OutputTransactionHash;
                    input.OutputIndex = inputMsg.OutputIndex;
                    input.Size = inputMsg.Size;
                    input.UnlockScript = inputMsg.UnlockScript;
                    input.BlockHash = block.Hash;

                    var output = outputDic.FirstOrDefault(x => x.TransactionHash == inputMsg.OutputTransactionHash && x.Index == inputMsg.OutputIndex);

                    if (output != null)
                    {
                        input.Amount = output.Amount;
                        input.AccountId = output.ReceiverId;
                    }

                    transaction.Inputs.Add(input);
                    totalInput += input.Amount;
                }

                foreach (var outputMsg in txMsg.Outputs)
                {
                    var output = new Output();
                    output.Index = outputMsg.Index;
                    output.TransactionHash = transaction.Hash;
                    var address = AccountIdHelper.CreateAccountAddressByPublicKeyHash(
                        Base16.Decode(
                            Script.GetPublicKeyHashFromLockScript(outputMsg.LockScript)
                        ));
                    output.ReceiverId = address;
                    output.Amount = outputMsg.Amount;
                    output.Size = outputMsg.Size;
                    output.LockScript = outputMsg.LockScript;
                    output.BlockHash = block.Hash;
                    transaction.Outputs.Add(output);
                    totalOutput += output.Amount;
                }

                transaction.TotalInput = totalInput;
                transaction.TotalOutput = totalOutput;
                transaction.Fee = totalInput - totalOutput;
                transaction.Size = txMsg.Size;

                if (txMsg.Inputs.Count == 1 &&
                    txMsg.Outputs.Count == 1 &&
                    txMsg.Inputs[0].OutputTransactionHash == Base16.Encode(HashHelper.EmptyHash()))
                {
                    transaction.Fee = 0;
                }

                block.Transactions.Add(transaction);
                block.TotalAmount += transaction.TotalOutput;
                block.TotalFee += transaction.Fee;

            }

            //block.GenerationSignature = blockMsg.Header.GenerationSignature;
            block.BlockSignature = blockMsg.Header.BlockSignature;
            //block.CumulativeDifficulty = blockMsg.Header.CumulativeDifficulty;
            block.PayloadHash = blockMsg.Header.PayloadHash;
            block.IsVerified = false;
            return block;
        }

        public BlockMsg ConvertEntityToBlockMsg(Block entity)
        {
            var txDac = TransactionDac.Default;
            var inputDac = InputDac.Default;
            var outputDac = OutputDac.Default;

            var blockMsg = new BlockMsg();
            var headerMsg = new BlockHeaderMsg();
            headerMsg.Version = entity.Version;
            headerMsg.Hash = entity.Hash;
            headerMsg.Height = entity.Height;
            headerMsg.PreviousBlockHash = entity.PreviousBlockHash;
            headerMsg.Bits = entity.Bits;
            headerMsg.Nonce = entity.Nonce;
            headerMsg.Timestamp = entity.Timestamp;

            var transactions = txDac.SelectByBlockHash(entity.Hash);
            headerMsg.TotalTransaction = transactions == null ? 0 : transactions.Count;

            foreach (var tx in transactions)
            {
                var txMsg = new TransactionMsg();
                txMsg.Version = tx.Version;
                txMsg.Hash = tx.Hash;
                txMsg.Timestamp = tx.Timestamp;
                txMsg.Locktime = tx.LockTime;
                txMsg.ExpiredTime = tx.ExpiredTime;

                var inputs = inputDac.SelectByTransactionHash(tx.Hash);
                var outputs = outputDac.SelectByTransactionHash(tx.Hash);

                bool inputsNeedRepair = false;
                bool outputsNeedRepair = false;

                if (inputs.Count == 0)
                {
                    inputs = inputDac.SelectFirstDiscardedByTransactionHash(tx.Hash);
                    inputsNeedRepair = true;
                }

                if (outputs.Count == 0)
                {
                    outputs = outputDac.SelectFirstDiscardedByTransactionHash(tx.Hash);
                    outputsNeedRepair = true;
                }

                foreach (var input in inputs)
                {
                    if (inputsNeedRepair)
                    {
                        inputDac.UpdateBlockAndDiscardStatus(input.Id, entity.Hash, false);
                    }

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
                    if (outputsNeedRepair)
                    {
                        outputDac.UpdateBlockAndDiscardStatus(output.Id, entity.Hash, false);
                    }

                    txMsg.Outputs.Add(new OutputMsg
                    {
                        Index = output.Index,
                        Amount = output.Amount,
                        Size = output.Size,
                        LockScript = output.LockScript
                    });
                }

                blockMsg.Transactions.Add(txMsg);
            }

            headerMsg.GeneratorId = entity.GeneratorId;
            //headerMsg.GenerationSignature = entity.GenerationSignature;
            //headerMsg.BlockSignature = entity.BlockSignature;
            //headerMsg.CumulativeDifficulty = entity.CumulativeDifficulty;
            headerMsg.PayloadHash = entity.PayloadHash;
            headerMsg.BlockSignature = entity.BlockSignature;
            headerMsg.BlockSigSize = entity.BlockSignature.Length;
            blockMsg.Header = headerMsg;
            return blockMsg;
        }

        public void ProcessUncsonfirmedBlocks()
        {
            var blockDac = BlockDac.Default;
            var txDac = TransactionDac.Default;
            var txComponent = new TransactionComponent();
            var utxoComponent = new UtxoComponent();

            long lastHeight = this.GetLatestHeight();
            long confirmedHeight = -1;
            var confirmedBlock = blockDac.SelectLastConfirmed();
            var confirmedHash = Base16.Encode(HashHelper.EmptyHash());

            if (confirmedBlock != null)
            {
                confirmedHeight = confirmedBlock.Height;
                confirmedHash = confirmedBlock.Hash;
            }

            if (lastHeight - confirmedHeight >= 6)
            {
                var blocks = blockDac.SelectByPreviousHash(confirmedHash);

                if (blocks.Count == 1)
                {
                    BlockAccess.Current.UpdateBlockStatusToConfirmed(blocks[0].Hash, () =>
                    {
                        GlobalParameters.LocalConfirmedHeight = blocks[0].Height;
                        this.ProcessUncsonfirmedBlocks();
                    });
                }
                else if (blocks.Count > 1)
                {
                    var countOfDescendants = new Dictionary<string, long>();
                    foreach (var block in blocks)
                    {
                        var count = blockDac.CountOfDescendants(block.Hash);

                        if (!countOfDescendants.ContainsKey(block.Hash))
                        {
                            countOfDescendants.Add(block.Hash, count);
                        }
                    }

                    var dicSort = countOfDescendants.OrderBy(d => d.Value).ToList();

                    var lastItem = blocks.Where(b => b.Hash == dicSort[dicSort.Count - 1].Key).First();
                    var index = 0;

                    while (index < dicSort.Count - 1)
                    {
                        var currentItem = blocks.Where(b => b.Hash == dicSort[index].Key).First();

                        if (lastHeight - currentItem.Height >= 6)
                        {
                            var txList = txDac.SelectByBlockHash(currentItem.Hash);

                            //Skip coinbase tx
                            for (int i = 1; i < txList.Count; i++)
                            {
                                var tx = txList[i];
                                var txMsg = txComponent.ConvertTxEntityToMsg(tx);

                                try
                                {
                                    txComponent.AddTransactionToPool(txMsg);
                                }
                                catch (Exception ex)
                                {
                                    LogHelper.Error(ex.Message, ex);
                                }
                            }

                            //remove coinbase from utxo
                            UtxoSet.Instance.RemoveUtxoRecord(txList[0].Hash, 0);
                            //解决区块分叉问题
                            BlockAccess.Current.UpdateBlockStatusToDiscarded(currentItem.Hash, null);
                            //删掉分叉区块
                            var key = currentItem.Hash + "_" + currentItem.Height + ";";
                            CacheManager.Default.DeleteByKey(DataCatelog.Block, key);
                            CacheManager.Default.Put(DataCatelog.BlockSimple, currentItem.Height.ToString(), currentItem.Hash);
                            index++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (dicSort.Count - index <= 1)
                    {
                        BlockAccess.Current.UpdateBlockStatusToConfirmed(lastItem.Hash, () => {
                            GlobalParameters.LocalConfirmedHeight = blocks[0].Height;
                        });

                        this.ProcessUncsonfirmedBlocks();
                    }
                }
            }
        }

        public List<Block> GetVerifiedHashes(List<string> hashes)
        {
            BlockDac dac = BlockDac.Default;
            return dac.DistinguishBlockVerifiedByHashes(hashes);
        }

        public ListSinceBlock ListSinceBlock(string blockHash, long heightLength, long confirmations)
        {
            BlockDac dac = BlockDac.Default;
            //确认次数=总区块高度-当前区块高度+1
            return dac.GetSinceBlock(blockHash, heightLength, confirmations);
        }

        public ListSinceBlock ListPageSinceBlock(string blockHash, long heightLength, long confirmations, int currentPage, int pageSize)
        {
            BlockDac dac = BlockDac.Default;
            //确认次数=总区块高度-当前区块高度+1
            return dac.GetPageSinceBlock(blockHash, heightLength, confirmations, currentPage, pageSize);
        }

        /// <summary>
        /// 获取区块总的奖励
        /// </summary>
        /// <param name="blockHash"></param>
        /// <returns></returns>
        public long GetBlockReward(string blockHash)
        {
            //总的奖励分为两部分，一部分挖矿所得，一部分区块中交易手续费
            long totalReward = 0;
            var txList = TransactionDac.Default.SelectByBlockHash(blockHash);

            foreach (var tx in txList)
            {
                if (tx.TotalInput == 0 && tx.Fee == 0)
                {
                    totalReward += tx.TotalOutput;
                }
            }

            return totalReward;
        }
    }
}
