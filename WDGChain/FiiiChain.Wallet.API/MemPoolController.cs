// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using EdjCase.JsonRpc.Router;
using EdjCase.JsonRpc.Router.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FiiiChain.DTO;
using FiiiChain.Framework;
using FiiiChain.Business;
using FiiiChain.Messages;
using FiiiChain.Consensus;
using FiiiChain.Entities;
using FiiiChain.Entities.CacheModel;
using FiiiChain.DataAgent;
using FiiiChain.IModules;

namespace FiiiChain.Wallet.API
{
    public class MemPoolController : BaseRpcController
    {
        public IRpcMethodResult GetAllTxInMemPool()
        {
            try
            {
                var result = new TransactionComponent().GetAllHashesRelevantWithCurrentWalletFromPool();
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

        public IRpcMethodResult GetPaymentInfoInMemPool(string txHash)
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
                var accounts = accountComponent.GetAllAccounts();
                var paymentAccountIds = accounts.Where(a => !string.IsNullOrWhiteSpace(a.PrivateKey)).Select(a => a.Id).ToList();
                var allAccountIds = accounts.Select(a => a.Id).ToList();
                var addressBook = CacheManager.Default.Get<AddressBookItem>(DataCatelog.AddressBook);
                var latestHeight = blockComponent.GetLatestHeight();

                var tx = txComponent.GetTransactionEntityByHash(txHash);

                if (tx != null)
                {
                    long totalInput = 0;
                    long selfTotalOutput = 0;
                    long otherUserTotalOutput = 0;
                    bool coibase = false;

                    if (tx.Inputs.Count == 1 && tx.Outputs.Count == 1 && tx.Inputs[0].OutputTransactionHash == Base16.Encode(HashHelper.EmptyHash()))
                    {
                        coibase = true;
                    }

                    if (!coibase)
                    {
                        foreach (var input in tx.Inputs)
                        {
                            var oldOutput = txComponent.GetOutputEntiyByIndexAndTxHash(input.OutputTransactionHash, input.OutputIndex);

                            if (oldOutput != null && paymentAccountIds.Contains(oldOutput.ReceiverId))
                            {
                                totalInput += input.Amount;
                            }
                            else
                            {
                                totalInput = 0;
                                break;
                            }
                        }
                    }

                    foreach (var output in tx.Outputs)
                    {
                        if (allAccountIds.Contains(output.ReceiverId))
                        {
                            selfTotalOutput += output.Amount;
                        }
                        else
                        {
                            otherUserTotalOutput += output.Amount;
                        }
                    }

                    BlockMsg block = null;

                    if (tx.BlockHash != null)
                    {
                        block = blockComponent.GetBlockMsgByHash(tx.BlockHash);
                    }

                    if (coibase)
                    {
                        var payment = new PaymentOM();
                        payment.address = tx.Outputs[0].ReceiverId;
                        payment.account = accounts.Where(a => a.Id == payment.address).Select(a => a.Tag).FirstOrDefault();
                        payment.category = PaymentCatelog.Generate;
                        payment.totalInput = totalInput;
                        payment.totalOutput = selfTotalOutput;
                        payment.amount = selfTotalOutput;
                        payment.fee = 0;
                        payment.txId = tx.Hash;
                        payment.vout = 0;
                        payment.time = tx.Timestamp;
                        payment.size = tx.Size;

                        var txComment = transactionCommentComponent.GetByTransactionHashAndIndex(tx.Hash, 0);
                        if (txComment != null)
                        {
                            payment.comment = txComment.Comment;
                        }

                        if (block != null)
                        {
                            payment.blockHash = tx.BlockHash;
                            payment.blockIndex = 0;// block.Transactions.FindIndex(t=>t.Hash == tx.Hash);
                            payment.blockTime = block.Header.Timestamp;
                            payment.confirmations = latestHeight - block.Header.Height + 1;
                        }
                        else
                        {
                            payment.confirmations = 0;
                        }

                        result.Add(payment);
                    }
                    else if (totalInput > 0 && otherUserTotalOutput == 0)
                    {
                        var payment = new PaymentOM();
                        payment.address = null;
                        payment.account = null;
                        payment.category = PaymentCatelog.Self;
                        payment.totalInput = totalInput;
                        payment.totalOutput = selfTotalOutput;
                        payment.fee = totalInput - selfTotalOutput;
                        payment.amount = payment.fee;
                        payment.txId = tx.Hash;
                        payment.vout = 0;
                        payment.time = tx.Timestamp;
                        payment.size = tx.Size;

                        var txComments = transactionCommentComponent.GetByTransactionHash(tx.Hash);
                        if (txComments.Count > 0)
                        {
                            payment.comment = "";
                            foreach (var item in txComments)
                            {
                                if (!string.IsNullOrWhiteSpace(item.Comment))
                                {
                                    payment.comment += item.Comment + ";";
                                }
                            }
                        }

                        if (block != null)
                        {
                            payment.blockHash = tx.BlockHash;
                            payment.blockIndex = block.Transactions.FindIndex(t => t.Hash == tx.Hash);
                            payment.blockTime = block.Header.Timestamp;
                            payment.confirmations = latestHeight - block.Header.Height + 1;
                        }
                        else
                        {
                            payment.confirmations = 0;
                        }

                        result.Add(payment);
                    }
                    else if (totalInput > 0)
                    {
                        for (int i = 0; i < tx.Outputs.Count; i++)
                        {
                            if (!allAccountIds.Contains(tx.Outputs[i].ReceiverId))
                            {
                                var payment = new PaymentOM();
                                payment.address = tx.Outputs[i].ReceiverId;
                                payment.account = addressBook.Where(a => a.Address == payment.address && !string.IsNullOrWhiteSpace(a.Tag)).Select(a => a.Tag).FirstOrDefault();
                                payment.category = PaymentCatelog.Send;
                                payment.totalInput = totalInput;
                                payment.totalOutput = tx.Outputs[i].Amount;
                                payment.fee = totalInput - (selfTotalOutput + otherUserTotalOutput);
                                payment.amount = (i == 0 ? tx.Outputs[i].Amount + payment.fee : tx.Outputs[i].Amount);
                                payment.txId = tx.Hash;
                                payment.vout = i;
                                payment.time = tx.Timestamp;
                                payment.size = tx.Size;

                                var txComment = transactionCommentComponent.GetByTransactionHashAndIndex(tx.Hash, i);
                                if (txComment != null)
                                {
                                    payment.comment = txComment.Comment;
                                }

                                if (block != null)
                                {
                                    payment.blockHash = tx.BlockHash;
                                    payment.blockIndex = block.Transactions.FindIndex(t => t.Hash == tx.Hash);
                                    payment.blockTime = block.Header.Timestamp;
                                    payment.confirmations = latestHeight - block.Header.Height + 1;
                                }
                                else
                                {
                                    payment.confirmations = 0;
                                }

                                result.Add(payment);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < tx.Outputs.Count; i++)
                        {
                            if (allAccountIds.Contains(tx.Outputs[i].ReceiverId))
                            {
                                var payment = new PaymentOM();
                                payment.address = tx.Outputs[i].ReceiverId;
                                payment.account = accounts.Where(a => a.Id == payment.address).Select(a => a.Tag).FirstOrDefault(); ;
                                payment.category = PaymentCatelog.Receive;
                                payment.totalInput = totalInput;
                                payment.totalOutput = tx.Outputs[i].Amount;
                                payment.fee = totalInput - (selfTotalOutput + otherUserTotalOutput);
                                payment.amount = tx.Outputs[i].Amount;
                                payment.txId = tx.Hash;
                                payment.vout = i;
                                payment.time = tx.Timestamp;
                                payment.size = tx.Size;

                                var txComment = transactionCommentComponent.GetByTransactionHashAndIndex(tx.Hash, i);
                                if (txComment != null)
                                {
                                    payment.comment = txComment.Comment;
                                }

                                if (block != null)
                                {
                                    payment.blockHash = tx.BlockHash;
                                    payment.blockIndex = block.Transactions.FindIndex(t => t.Hash == tx.Hash);
                                    payment.blockTime = block.Header.Timestamp;
                                    payment.confirmations = latestHeight - block.Header.Height + 1;
                                }
                                else
                                {
                                    payment.confirmations = 0;
                                }

                                result.Add(payment);
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

    }
}
