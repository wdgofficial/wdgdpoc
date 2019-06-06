// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

namespace WDG.DTO
{
    /// <summary>
    /// Information about the UTXO set
    /// </summary>
    public class TxOutSetOM
    {
        /// <summary>
        /// The height of the local best block chain. 
        /// A new node with only the hardcoded genesis block will have a height of 0
        /// </summary>
        public long Height { get; set; }

        /// <summary>
        /// The hash of the header of the highest block on the local best block chain, encoded as hex in RPC byte order
        /// </summary>
        public string BestBlock { get; set; }

        /// <summary>
        /// The number of transactions with unspent outputs
        /// </summary>
        public long Transactions { get; set; }

        /// <summary>
        /// The number of unspent transaction outputs
        /// </summary>
        public long Txouts { get; set; }

        /// <summary>
        /// The total number of fiiicoins in the UTXO set
        /// </summary>
        public long Total_amount { get; set; }
    }
}
