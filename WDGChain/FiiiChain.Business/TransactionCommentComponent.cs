// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.Consensus;
using FiiiChain.Data;
using FiiiChain.Data.Accesses;
using FiiiChain.DataAgent;
using FiiiChain.Entities;
using FiiiChain.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiiiChain.Business
{
    public class TransactionCommentComponent
    {
        public long Add(string txHash, int outputIndex, string comment)
        {
            var item = new TransactionComment();
            item.TransactionHash = txHash;
            item.OutputIndex = outputIndex;
            item.Comment = comment;
            item.Timestamp = Time.EpochTime;
            var result = 0L;
            result = TransactionCommentDac.Default.Insert(item);
            return result;
        }

        public TransactionComment GetByTransactionHashAndIndex(string txHash, int outputIndex)
        {
            return TransactionCommentDac.Default.SelectByTransactionHashAndIndex(txHash, outputIndex);
        }

        public List<TransactionComment> GetByTransactionHash(string txHash)
        {
            return TransactionCommentDac.Default.SelectByTransactionHash(txHash);
        }
    }
}
