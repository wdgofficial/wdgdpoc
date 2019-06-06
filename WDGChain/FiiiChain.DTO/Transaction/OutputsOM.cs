using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.DTO.Transaction
{
    public class OutputsOM
    {
        public int Vout { get; set; }

        public string Txid { get; set; }

        public string ReceiverId { get; set; }

        public long Amount { get; set; }

        public int Size { get; set; }

        public string LockScript { get; set; }

        public bool Spent { get; set; }

        public bool IsDiscarded { get; set; }
    }
}
