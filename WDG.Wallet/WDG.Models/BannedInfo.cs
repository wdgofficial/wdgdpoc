// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

namespace WDG.Models
{
    public class BannedInfo:BaseModel
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

        private string expired;
        public string Expired
        {
            get { return expired; }
            set
            {
                expired = value;
                OnChanged("Expired");
            }
        }
    }
}
