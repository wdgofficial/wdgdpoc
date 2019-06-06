// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

namespace WDG.Models
{
    public class TransactionFeeSetting :BaseModel
    {
        private long confirmations;
        public long Confirmations
        {
            get { return confirmations; }
            set
            {
                confirmations = value;
                OnChanged("Confirmations");
            }
        }

        private long feePerKB;
        public long FeePerKB
        {
            get { return feePerKB; }
            set
            {
                feePerKB = value;
                OnChanged("FeePerKB");
            }
        }

        private bool encrypt;
        public bool Encrypt
        {
            get { return encrypt; }
            set
            {
                encrypt = value;
                OnChanged("Encrypt");
            }
        }
    }
}
