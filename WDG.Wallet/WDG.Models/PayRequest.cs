// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.


namespace WDG.Models
{
    public class PayRequest : BaseModel
    {
        private long id;
        public long Id
        {
            get { return id; }
            set
            {
                id = value;
                OnChanged("Id");
            }
        }

        private string accountId;
        public string AccountId
        {
            get { return accountId; }
            set
            {
                accountId = value;
                OnChanged("AccountId");
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

        private string comment;
        public string Comment
        {
            get { return comment; }
            set
            {
                comment = value;
                OnChanged("Comment");
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

        private long timestamp;
        public long Timestamp
        {
            get { return timestamp; }
            set
            {
                timestamp = value;
                OnChanged("Timestamp");
            }
        }
    }
}
