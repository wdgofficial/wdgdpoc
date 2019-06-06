// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.


namespace WDG.Models
{
    public class AddNodeInfo : BaseModel
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

        private string connected;
        public string Connected
        {
            get { return connected; }
            set
            {
                connected = value;
                OnChanged("Connected");
            }
        }

        private long connectedTime;
        public long ConnectedTime
        {
            get { return connectedTime; }
            set
            {
                connectedTime = value;
                OnChanged("ConnectedTime");
            }
        }
    }
}
