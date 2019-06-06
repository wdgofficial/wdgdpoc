// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

namespace WDG.DTO
{
    public class TxOutOM
    {
        public string BestBlock { get; set; }

        public long Confirmations { get; set; }

        public long Value { get; set; }

        public string ScriptPubKey { get; set; }

        public int Version { get; set; }

        public bool CoinBase { get; set; }
    }
}
