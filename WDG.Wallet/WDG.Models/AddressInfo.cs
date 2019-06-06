// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.


namespace WDG.Models
{
    public class AddressInfo :BaseModel
    {
        private bool isValid;
        public bool IsValid
        {
            get { return isValid; }
            set
            {
                isValid = value;
                OnChanged("IsValid");
            }
        }

        private string address;
        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                OnChanged("address");
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

        private bool isMine;
        public bool IsMine
        {
            get { return isMine; }
            set
            {
                isMine = value;
                OnChanged("IsMine");
            }
        }

        private bool isWatchOnly;
        public bool IsWatchOnly
        {
            get { return isWatchOnly; }
            set
            {
                isWatchOnly = value;
                OnChanged("IsWatchOnly");
            }
        }

        private bool isScript;
        public bool IsScript
        {
            get { return isScript; }
            set
            {
                isScript = value;
                OnChanged("IsScript");
            }
        }

        private string script;
        public string Script
        {
            get { return script; }
            set
            {
                script = value;
                OnChanged("Script");
            }
        }

        private string hex;
        public string Hex
        {
            get { return hex; }
            set
            {
                hex = value;
                OnChanged("Hex");
            }
        }

        private string[] addresses;
        public string[] Addresses
        {
            get { return addresses; }
            set
            {
                addresses = value;
                OnChanged("Addresses");
            }
        }

        private long sigrequired;
        public long Sigrequired
        {
            get { return sigrequired; }
            set
            {
                sigrequired = value;
                OnChanged("Sigrequired");
            }
        }

        private string pubKey;
        public string PubKey
        {
            get { return pubKey; }
            set
            {
                pubKey = value;
                OnChanged("PubKey");
            }
        }

        private bool isCompressed;
        public bool IsCompressed
        {
            get { return isCompressed; }
            set
            {
                isCompressed = value;
                OnChanged("IsCompressed");
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

        private string hdkeypath;
        public string Hdkeypath
        {
            get { return hdkeypath; }
            set
            {
                hdkeypath = value;
                OnChanged("Hdkeypath");
            }
        }

        private string hdmasterkeyid;
        public string Hdmasterkeyid
        {
            get { return hdmasterkeyid; }
            set
            {
                hdmasterkeyid = value;
                OnChanged("Hdmasterkeyid");
            }
        }
    }
}
