// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

namespace WDG.Models
{
    public class BlockChainInfo : BaseModel
    {
        private bool isRunning;
        public bool IsRunning
        {
            get { return isRunning; }
            set
            {
                isRunning = value;
                OnChanged("IsRunning");
            }
        }
        private long connections;
        public long Connections
        {
            get { return connections; }
            set
            {
                connections = value;
                OnChanged("Connections");
            }
        }

        private long localLastBlockHeight;
        public long LocalLastBlockHeight
        {
            get { return localLastBlockHeight; }
            set
            {
                localLastBlockHeight = value;
                OnChanged("LocalLastBlockHeight");
            }
        }
        private long remoteLatestBlockHeight;
        public long RemoteLatestBlockHeight
        {
            get { return remoteLatestBlockHeight; }
            set
            {
                remoteLatestBlockHeight = value;
                OnChanged("RemoteLatestBlockHeight");
            }
        }

        private long timeOffset;
        public long TimeOffset
        {
            get { return timeOffset; }
            set
            {
                timeOffset = value;
                OnChanged("TimeOffset");
            }
        }

        private long localLastBlockTime;
        public long LocalLastBlockTime
        {
            get { return localLastBlockTime; }
            set
            {
                localLastBlockTime = value;
                OnChanged("LocalLastBlockTime");
            }
        }

        private long tempBlockCount;
        public long TempBlockCount
        {
            get { return tempBlockCount; }
            set
            {
                tempBlockCount = value;
                OnChanged("TempBlockCount");
            }
        }
    }
}
