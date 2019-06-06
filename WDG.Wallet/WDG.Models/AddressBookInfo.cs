// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.


namespace WDG.Models
{
    public class AddressBookInfo : BaseModel
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
