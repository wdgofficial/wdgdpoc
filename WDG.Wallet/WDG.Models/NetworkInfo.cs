// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.


namespace WDG.Models
{
    public class NetworkInfo : BaseModel
    {
        private int version;
        public int Version
        {
            get { return version; }
            set
            {
                version = value;
                OnChanged("Version");
            }
        }

        private int minimumSupportedVersion;
        public int MinimumSupportedVersion
        {
            get { return minimumSupportedVersion; }
            set
            {
                version = minimumSupportedVersion;
                OnChanged("MinimumSupportedVersion");
            }
        }

        private int protocolVersion;
        public int ProtocolVersion
        {
            get { return protocolVersion; }
            set
            {
                protocolVersion = value;
                OnChanged("ProtocolVersion");
            }
        }

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

        private int connections;
        public int Connections
        {
            get { return connections; }
            set
            {
                connections = value;
                OnChanged("Connections");
            }
        }
    }
}
