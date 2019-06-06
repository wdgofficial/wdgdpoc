using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.DTO.Transaction
{
    public class InputsOM
    {
        public string Txid { get; set; }

        public int Vout { get; set; }

        public int Size { get; set; }

        public long Amount { get; set; }

        public string UnlockScript { get; set; }

        public string AccountId { get; set; }

        public bool IsDiscarded { get; set; }
    }
}
