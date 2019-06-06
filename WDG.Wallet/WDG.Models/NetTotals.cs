// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.


namespace WDG.Models
{
    public class NetTotals: BaseModel
    {
        private long totalBytesRecv;
        public long TotalBytesRecv
        {
            get { return totalBytesRecv; }
            set
            {
                totalBytesRecv = value;
                OnChanged("TotalBytesRecv");
            }
        }

        private long totalBytesSent;
        public long TotalBytesSent
        {
            get { return totalBytesSent; }
            set
            {
                totalBytesSent = value;
                OnChanged("TotalBytesSent");
            }
        }

        private long timeMillis;
        public long TimeMillis
        {
            get { return timeMillis; }
            set
            {
                timeMillis = value;
                OnChanged("TimeMillis");
            }
        }
    }
}
