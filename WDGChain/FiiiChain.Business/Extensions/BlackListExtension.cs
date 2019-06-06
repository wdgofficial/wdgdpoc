// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.Data;
using FiiiChain.DataAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiiiChain.Business.Extensions
{
    internal enum TxType
    {
        UnPackge,
        RepeatedCost,
        NoFoundUtxo
    }

    internal static class BlackListExtension
    {
        public static void BlackTxPoolItems()
        {
            var items = TransactionPool.Instance.MainPool.ToList();
            items.AddRange(TransactionPool.Instance.IsolateTransactionPool);
            List<string> hashes = new List<string>();

            foreach (TransactionPoolItem item in items)
            {
                var type = Check(item);
                if (type != TxType.UnPackge)
                {
                    hashes.Add(item.Transaction.Hash);
                    TransactionPool.Instance.RemoveTransaction(item.Transaction.Hash);
                }
            }
            BlacklistTxs.Current.Add(hashes);
        }

        static TransactionDac txDac = TransactionDac.Default;

        public static TxType Check(TransactionPoolItem poolItem)
        {
            var inputs = poolItem.Transaction.Inputs;
            foreach (var input in inputs)
            {
                if (!txDac.HasTransaction(input.OutputTransactionHash))
                    return TxType.NoFoundUtxo;
                if (txDac.HasCost(input.OutputTransactionHash, input.OutputIndex))
                    return TxType.RepeatedCost;
            }
            return TxType.UnPackge;
        }

        static void CheckTxItem(List<TransactionPoolItem> items)
        {
            var sb = new StringBuilder();
            int RepeatedCost = 0;
            int NoFoundUtxo = 0;
            foreach (var item in items)
            {
                var result = Check(item);
                if (result != TxType.UnPackge)
                {
                    sb.AppendLine($"{result.ToString()}:{item.Transaction.Hash}");
                    if (result == TxType.NoFoundUtxo)
                        NoFoundUtxo++;
                    else
                        RepeatedCost++;
                }
            }
        }
    }
}
