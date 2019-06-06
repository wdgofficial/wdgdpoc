// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.


namespace WDG.Models
{
    public class AccountInfo : BaseModel
    {
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

        private string publicKey;
        public string PublicKey
        {
            get { return publicKey; }
            set
            {
                publicKey = value;
                OnChanged("PublicKey");
            }
        }

        private long balance;
        public long Balance
        {
            get { return balance; }
            set
            {
                balance = value;
                OnChanged("Balance");
            }
        }

        private bool isDefault;
        public bool IsDefault
        {
            get { return isDefault; }
            set
            {
                isDefault = value;
                OnChanged("IsDefault");
            }
        }

        private bool watchOnly;
        public bool WatchOnly
        {
            get { return watchOnly; }
            set
            {
                watchOnly = value;
                OnChanged("WatchOnly");
            }
        }

        private string tag;
        public string Tag
        {
            get { return tag; }
            set
            {
                tag = value;
                OnChanged("Tag");
            }
        }
    }
}
