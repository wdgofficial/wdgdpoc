// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.


namespace WDG.Models
{
    public class BlockChainStatus : BaseModel
    {
        private string chainService;
        public string ChainService
        {
            get { return chainService; }
            set
            {
                chainService = value;
                OnChanged("ChainService");
            }
        }

        private string p2pService;
        public string P2pService
        {
            get { return p2pService; }
            set
            {
                p2pService = value;
                OnChanged("P2pService");
            }
        }
        private string blockService;
        public string BlockService
        {
            get { return blockService; }
            set
            {
                blockService = value;
                OnChanged("BlockService");
            }
        }

        private string rpcService;
        public string RpcService
        {
            get { return rpcService; }
            set
            {
                rpcService = value;
                OnChanged("RpcService");
            }
        }

        private string chainNetwork;
        public string ChainNetwork
        {
            get { return chainNetwork; }
            set
            {
                chainNetwork = value;
                OnChanged("ChainNetwork");
            }
        }

        private long height;
        public long Height
        {
            get { return height; }
            set
            {
                height = value;
                OnChanged("Height");
            }
        }
    }
}
