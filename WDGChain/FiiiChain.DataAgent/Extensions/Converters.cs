// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.Consensus;
using FiiiChain.Data;
using FiiiChain.DataAgent;
using FiiiChain.Entities;
using FiiiChain.Entities.CacheModel;
using FiiiChain.Framework;
using FiiiChain.IModules;
using FiiiChain.Messages;
using System.Collections.Generic;
using System.Linq;

namespace FiiiChain.DataAgent
{
    public static class Converters
    {
        public static Transaction ConvertTxMsgToEntity(this TransactionMsg txMsg,bool isSelf = false)
        {
            var entity = new Transaction();

            entity.Hash = txMsg.Hash;
            entity.BlockHash = null;
            entity.Version = txMsg.Version;
            entity.Timestamp = txMsg.Timestamp;
            entity.LockTime = txMsg.Locktime;
            entity.Inputs = new List<Input>();
            entity.Outputs = new List<Output>();

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

                Output output = null;
                if (isSelf)
                {
                    var outputKey = $"{input.OutputTransactionHash}_{input.OutputIndex}";
                    output = CacheManager.Default.Get<Output>(DataCatelog.Output, outputKey);
                }
                else
                {
                    output = OutputDac.Default.SelectByHashAndIndex(input.OutputTransactionHash, input.OutputIndex);
                }

                if (output != null)
                {
                    input.Amount = output.Amount;
                    input.AccountId = output.ReceiverId;
                }
                else
                {
                    input.Amount = 0;
                    input.AccountId = "";
                }
                
                entity.Inputs.Add(input);
                totalInput += input.Amount;
            }

            foreach (var outputMsg in txMsg.Outputs)
            {
                var output = new Output();
                output.Index = outputMsg.Index;
                output.TransactionHash = entity.Hash;
                var address = AccountIdHelper.CreateAccountAddressByPublicKeyHash(
                    Base16.Decode(
                        Script.GetPublicKeyHashFromLockScript(outputMsg.LockScript)
                    ));
                output.ReceiverId = address;
                output.Amount = outputMsg.Amount;
                output.Size = outputMsg.Size;
                output.LockScript = outputMsg.LockScript;
                entity.Outputs.Add(output);
                totalOutput += output.Amount;
            }

            entity.TotalInput = totalInput;
            entity.TotalOutput = totalOutput;
            entity.Fee = totalInput - totalOutput;
            entity.Size = txMsg.Size;

            if (txMsg.Inputs.Count == 1 &&
                txMsg.Outputs.Count == 1 &&
                txMsg.Inputs[0].OutputTransactionHash == Base16.Encode(HashHelper.EmptyHash()))
            {
                entity.Fee = 0;
            }

            return entity;
        }

        public static Transaction ConvertToEntity(this TransactionPoolItem item)
        {
            return item.Transaction.ConvertTxMsgToEntity();
        }

        public static List<PaymentCache> ConvertToPayment(TransactionPoolItem transactionpoolitem, bool isSelf = true)
        {
            List<PaymentCache> result = new List<PaymentCache>();

            var tx = transactionpoolitem.ConvertToEntity();

            var ids = CacheManager.Default.GetAllKeys(DataCatelog.Accounts);

            if (isSelf)
            {
                if (tx.Outputs.Any(x => ids.Contains(x.ReceiverId)) || tx.Inputs.Any(x => ids.Contains(x.AccountId)))
                {
                    return ConvertToSelfPayment(tx);
                }
                else
                {
                    return result;
                }
            }

            return ConvertToSelfPayment(tx);
        }

        static readonly byte[] EMPTYHASH = HashHelper.EmptyHash();

        public static List<PaymentCache> ConvertToSelfPayment(Transaction tx)
        {
            var result = new List<PaymentCache>();
            var accounts = CacheManager.Default.GetAllKeys(DataCatelog.Accounts);
            if (accounts == null || !accounts.Any())
                return result;

            var ids = tx.Inputs.Select(x => x.AccountId).ToList() ;
            ids.AddRange(tx.Outputs.Select(x => x.ReceiverId));

            if (!ids.Any(x => accounts.Contains(x)))
                return result;

            bool isCoinBase = tx.Inputs.Count == 1 && tx.Outputs.Count == 1 && tx.Inputs[0].OutputTransactionHash == Base16.Encode(EMPTYHASH);

            if (isCoinBase)
            {
                return tx.ConvertToGeneratePayment();
            }

            var hasmyInput = tx.Inputs.Any(x => accounts.Contains(x.AccountId));
            var hasOtherOuput = tx.Outputs.Any(x => !accounts.Contains(x.ReceiverId));

            if (hasmyInput)
            {
                if (!hasOtherOuput)
                {
                    result = tx.ConvertToSelf();
                }
                else
                {
                    result = tx.ConvertToSend(accounts);
                }
            }
            else
            {
                result = tx.ConvertToReceive(accounts);
            }

            return result;
        }


        static TransactionCommentDac txComent = TransactionCommentDac.Default;

        private static List<PaymentCache> ConvertToGeneratePayment(this Transaction tx)
        {
            List<PaymentCache> result = new List<PaymentCache>();
            PaymentCache payment = new PaymentCache();

            payment.address = tx.Outputs[0].ReceiverId;
            payment.account = "";
            payment.category = PaymentCatelog.Generate;
            payment.totalInput = 0;
            payment.totalOutput = tx.Outputs[0].Amount;
            payment.amount = tx.Outputs[0].Amount;
            payment.fee = 0;
            payment.txId = tx.Hash;
            payment.vout = 0;
            payment.time = tx.Timestamp;
            payment.size = tx.Size;

            result.Add(payment);
            return result;
        }

        private static List<PaymentCache> ConvertToSend(this Transaction tx, List<string> accounts)
        {
            List<PaymentCache> result = new List<PaymentCache>();

            var totalInput = tx.Inputs.Sum(x => x.Amount);
            var totalOuput = tx.Outputs.Sum(x => x.Amount);
            var totalfee = totalInput - totalOuput;

            var outputCount = tx.Outputs.Count;
            bool useFee = false;
            for (int i = 0; i < outputCount; i++)
            {
                var output = tx.Outputs[i];
                if (accounts.Contains(output.ReceiverId))
                    continue;

                PaymentCache payment = new PaymentCache();
                payment.category = PaymentCatelog.Send;

                payment.address = output.ReceiverId;
                payment.account = "";
                payment.comment = txComent.SelectComment(tx.Hash, i);
                if (!useFee)
                {
                    payment.fee = totalfee;
                    useFee = true;
                }
                payment.amount = output.Amount + payment.fee;
                payment.size = output.Size;
                payment.time = tx.Timestamp;
                payment.totalInput = totalInput;
                payment.totalOutput = output.Amount;
                payment.txId = tx.Hash;
                payment.vout = i;

                result.Add(payment);
            }

            return result;
        }

        private static List<PaymentCache> ConvertToSelf(this Transaction tx)
        {
            List<PaymentCache> result = new List<PaymentCache>();

            var totalInput = tx.Inputs.Sum(x => x.Amount);
            var totalOuput = tx.Outputs.Sum(x => x.Amount);
            var totalfee = totalInput - totalOuput;

            var output = tx.Outputs[0];
            PaymentCache payment = new PaymentCache();

            payment.address = output.ReceiverId;
            payment.account = "";
            payment.category = PaymentCatelog.Self;
            payment.amount = output.Amount;
            payment.fee = totalfee;
            payment.size = output.Size;
            payment.time = tx.Timestamp;
            payment.totalInput = totalInput;
            payment.totalOutput = output.Amount;
            payment.txId = tx.Hash;
            payment.vout = 0;

            result.Add(payment);

            return result;
        }

        private static List<PaymentCache> ConvertToReceive(this Transaction tx, List<string> accounts)
        {
            List<PaymentCache> result = new List<PaymentCache>();

            var totalInput = tx.Inputs.Sum(x => x.Amount);
            var totalOuput = tx.Outputs.Sum(x => x.Amount);
            var totalfee = totalInput - totalOuput;

            for (int i = 0; i < tx.Outputs.Count; i++)
            {
                var output = tx.Outputs[i];
                PaymentCache payment = new PaymentCache();
                if (!accounts.Contains(output.ReceiverId))
                    continue;
                payment.address = output.ReceiverId;
                payment.account = "";
                payment.category = PaymentCatelog.Receive;
                payment.amount = output.Amount;
                if (i == 0)
                    payment.fee = totalfee;
                payment.size = output.Size;
                payment.time = tx.Timestamp;
                payment.totalInput = totalInput;
                payment.totalOutput = output.Amount;
                payment.txId = tx.Hash;
                payment.vout = i;

                result.Add(payment);
            }

            return result;
        }
    }
}