// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.


namespace WDG.DTO
{
    public class PaymentOM
    {
        public string Account { get; set; }

        public string Address { get; set; }

        /// <summary>
        /// set to one of the following values: send, receive, generate, immature, orphan, move
        /// </summary>
        public string Category { get; set; }

        public long Amount { get; set; }

        public long TotalInput { get; set; }

        public long TotalOutput { get; set; }

        public long Fee { get; set; }

        public string TxId { get; set; }

        public long Vout { get; set; }

        public long Time { get; set; }

        public long Size { get; set; }

        public string Comment { get; set; }

        public long Confirmations { get; set; }

        public string BlockHash { get; set; }

        public long BlockIndex { get; set; }

        public long BlockTime { get; set; }

    }
}
