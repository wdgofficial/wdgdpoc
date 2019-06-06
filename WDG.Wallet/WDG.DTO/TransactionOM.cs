using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDG.DTO
{
    public class TransactionOM
    {
        public long Id { get; set; }

        public string Hash { get; set; }

        public string BlockHash { get; set; }

        public int Version { get; set; }

        public long Timestamp { get; set; }

        public long LockTime { get; set; }

        public long TotalInput { get; set; }

        public long TotalOutput { get; set; }

        public int Size { get; set; }

        public long Fee { get; set; }

        public long ExpiredTime { get; set; }

        public bool IsDiscarded { get; set; }

        public List<Input> Inputs { get; set; }

        public List<Output> Outputs { get; set; }
    }

    public class Input
    {
        public long Id { get; set; }

        public string TransactionHash { get; set; }

        public string OutputTransactionHash { get; set; }

        public int OutputIndex { get; set; }

        public int Size { get; set; }

        public long Amount { get; set; }

        public string UnlockScript { get; set; }

        public string AccountId { get; set; }

        public bool IsDiscarded { get; set; }

        public string BlockHash { get; set; }
    }

    public class Output
    {
        public long Id { get; set; }

        public int Index { get; set; }

        public string TransactionHash { get; set; }

        public string ReceiverId { get; set; }

        public long Amount { get; set; }

        public int Size { get; set; }

        public string LockScript { get; set; }

        public bool Spent { get; set; }

        public bool IsDiscarded { get; set; }

        public string BlockHash { get; set; }
    }

    public class PageUnspentOM
    {
        public string txid { get; set; }
        public long vout { get; set; }
        public string address { get; set; }
        public string account { get; set; }
        public string scriptPubKey { get; set; }
        public string redeemScript { get; set; }
        public long amount { get; set; }
        public long confirmations { get; set; }
        public bool spendable { get; set; }
        public bool solvable { get; set; }
    }
}
