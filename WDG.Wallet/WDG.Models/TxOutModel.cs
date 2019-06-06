// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

namespace WDG.Models
{
    public class TxOutModel : BaseModel
    {
        private string bestBlock;
        public string BestBlock
        {
            get { return bestBlock; }
            set
            {
                bestBlock = value;
                OnChanged("BestBlock");
            }
        }

        private long confirmations;
        public long Confirmations
        {
            get { return confirmations; }
            set
            {
                confirmations = value;
                OnChanged("Confirmations");
            }
        }

        private long txValue;
        public long TxValue
        {
            get { return txValue; }
            set
            {
                txValue = value;
                OnChanged("TxValue");
            }
        }

        private string scriptPubKey;
        public string ScriptPubKey
        {
            get { return scriptPubKey; }
            set
            {
                scriptPubKey = value;
                OnChanged("ScriptPubKey");
            }
        }

        private int version;
        public int Version
        {
            get { return version; }
            set
            {
                version = value;
                OnChanged("Version");
            }
        }

        private bool coinBase;
        public bool CoinBase
        {
            get { return coinBase; }
            set
            {
                coinBase = value;
                OnChanged("CoinBase");
            }
        }
    }
}
