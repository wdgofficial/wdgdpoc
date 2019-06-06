// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.


namespace WDG.DTO
{
    public class BlockChainStatusOM
    {
        /// <summary>
        /// three status: Running, Stoping, Stoped
        /// </summary>
        public string ChainService { get; set; }

        public string P2pService { get; set; }

        public string BlockService { get; set; }

        public string RpcService { get; set; }

        /// <summary>
        /// Mainnet or Testnet
        /// </summary>
        public string ChainNetwork { get; set; }

        public long Height { get; set; }
    }
}
