// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

namespace WDG.DTO
{
    public class TransactionFeeSettingOM
    {
        public long Confirmations { get; set; }

        public long FeePerKB { get; set; }

        public bool Encrypt { get; set; }
    }
}
