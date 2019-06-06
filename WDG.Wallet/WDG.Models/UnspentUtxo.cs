// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

namespace WDG.Models
{
    public class UnspentUtxo : BaseModel
    {
        private string txid;
        public string TXID
        {
            get { return txid; }
            set
            {
                txid = value;
                OnChanged("TXID");
            }
        }


        private long vout;
        public long Vout
        {
            get { return vout; }
            set
            {
                vout = value;
                OnChanged("Vout");
            }
        }

        private string address;
        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                OnChanged("Address");
            }
        }

        private string account;
        public string Account
        {
            get { return account; }
            set
            {
                account = value;
                OnChanged("Account");
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

        private string redeemScript;
        public string RedeemScript
        {
            get { return redeemScript; }
            set
            {
                redeemScript = value;
                OnChanged("RedeemScript");
            }
        }

        private long amount;
        public long Amount
        {
            get { return amount; }
            set
            {
                amount = value;
                OnChanged("Amount");
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

        private bool spendable;
        public bool Spendable
        {
            get { return spendable; }
            set
            {
                spendable = value;
                OnChanged("Spendable");
            }
        }

        private bool solvable;
        public bool Solvable
        {
            get { return solvable; }
            set
            {
                solvable = value;
                OnChanged("TXID");
            }
        }
    }
}
