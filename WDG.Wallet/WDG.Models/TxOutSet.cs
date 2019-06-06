// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

namespace WDG.Models
{
    public class TxOutSet : BaseModel
    {
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

        private string bestBlock;
        public string BestBlock
        {
            get { return bestBlock; }
            set
            {
                bestBlock = value;
                OnChanged("BestBlock");
            }
        }

        private long transactions;
        public long Transactions
        {
            get { return transactions; }
            set
            {
                transactions = value;
                OnChanged("Transactions");
            }
        }

        private long txous;
        public long Txouts
        {
            get { return txous; }
            set
            {
                txous = value;
                OnChanged("Txouts");
            }
        }

        private long total_amount;
        public long Total_amount
        {
            get { return total_amount; }
            set
            {
                total_amount = value;
                OnChanged("Total_amount");
            }
        }
    }
}
