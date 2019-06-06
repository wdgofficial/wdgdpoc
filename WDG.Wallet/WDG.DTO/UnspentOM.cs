// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

namespace WDG.DTO
{
    public class UnspentOM
    {
        /// <summary>
        /// The TXID of the transaction containing the output, encoded as hex in RPC byte order
        /// </summary>
        public string TXID { get; set; }

        /// <summary>
        /// The output index number (vout) of the output within its containing transaction
        /// </summary>
        public long Vout { get; set; }

        /// <summary>
        /// The P2PKH or P2SH address the output paid. Only returned for P2PKH or P2SH output scripts
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// If the address returned belongs to an account, this is the account. Otherwise not returned
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// The output script paid, encoded as hex
        /// </summary>
        public string ScriptPubKey { get; set; }

        /// <summary>
        /// If the output is a P2SH whose script belongs to this wallet, this is the redeem script
        /// </summary>
        public string RedeemScript { get; set; }

        /// <summary>
        /// The amount paid to the output in fiiicoins
        /// </summary>
        public long Amount { get; set; }

        /// <summary>
        /// The number of confirmations received for the transaction containing this output
        /// </summary>
        public long Confirmations { get; set; }

        /// <summary>
        /// Set to true if the private key or keys needed to spend this output are part of the wallet. 
        /// Set to false if not (such as for watch-only addresses)
        /// </summary>
        public bool Spendable { get; set; }

        /// <summary>
        /// Set to true if the wallet knows how to spend this output. 
        /// Set to false if the wallet does not know how to spend the output. 
        /// It is ignored if the private keysare available
        /// </summary>
        public bool Solvable { get; set; }
    }
}
