// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.


namespace WDG.DTO
{
    public class PeerInfoOM
    {
        public long Id { get; set; }

        public bool IsTracker { get; set; }

        public string Addr { get; set; }

        public long LastSend { get; set; }

        public long LastRecv { get; set; }

        public long LastHeartBeat { get; set; }

        public long BytesSent { get; set; }

        public long BytesRecv { get; set; }

        public long ConnTime { get; set; }

        public long Version { get; set; }

        public bool Inbound { get; set; }

        public long LatestHeight { get; set; }
    }
}
