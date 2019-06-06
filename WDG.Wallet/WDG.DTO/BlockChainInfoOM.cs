// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

namespace WDG.DTO
{
    public class BlockChainInfoOM
    {
        public bool IsRunning { get; set; }

        public long Connections { get; set; }

        public long LocalLastBlockHeight { get; set; }

        public long RemoteLatestBlockHeight { get; set; }

        public long TimeOffset { get; set; }

        public long LocalLastBlockTime { get; set; }

        public long TempBlockCount { get; set; }
    }
}
