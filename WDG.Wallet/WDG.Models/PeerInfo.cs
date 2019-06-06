// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.


namespace WDG.Models
{
    public class PeerInfo : BaseModel
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
        private bool isTracker;
        public bool IsTracker
        {
            get { return isTracker; }
            set
            {
                isTracker = value;
                OnChanged("IsTracker");
            }
        }

        private string addr;
        public string Addr
        {
            get { return addr; }
            set
            {
                addr = value;
                OnChanged("Addr");
            }
        }
        private long lastSend;
        public long LastSend
        {
            get { return lastSend; }
            set
            {
                lastSend = value;
                OnChanged("LastSend");
            }
        }
        private long lastRecv;
        public long LastRecv
        {
            get { return lastRecv; }
            set
            {
                lastRecv = value;
                OnChanged("LastRecv");
            }
        }
        private long lastHeartBeat;
        public long LastHeartBeat
        {
            get { return lastHeartBeat; }
            set
            {
                lastHeartBeat = value;
                OnChanged("LastHeartBeat");
            }
        }
        private long bytesSent;
        public long BytesSent
        {
            get { return bytesSent; }
            set
            {
                bytesSent = value;
                OnChanged("BytesSent");
            }
        }
        private long bytesRecv;
        public long BytesRecv
        {
            get { return bytesRecv; }
            set
            {
                bytesRecv = value;
                OnChanged("BytesRecv");
            }
        }
        private long connTime;
        public long ConnTime
        {
            get { return connTime; }
            set
            {
                connTime = value;
                OnChanged("ConnTime");
            }
        }
        private long version;
        public long Version
        {
            get { return version; }
            set
            {
                version = value;
                OnChanged("Version");
            }
        }
        private bool inbound;
        public bool Inbound
        {
            get { return inbound; }
            set
            {
                inbound = value;
                OnChanged("Inbound");
            }
        }
        private long latestHeight;
        public long LatestHeight
        {
            get { return latestHeight; }
            set
            {
                latestHeight = value;
                OnChanged("LatestHeight");
            }
        }
    }
}
