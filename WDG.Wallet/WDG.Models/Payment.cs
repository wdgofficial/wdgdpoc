// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.


namespace WDG.Models
{
    public class Payment : BaseModel
    {
        private string account;
        public string Account
        {
            get { return account; }
            set
            {
                account = value;
                OnChanged("Account");
            }
        }


        private string address;
        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                OnChanged("Address");
            }
        }
        /// <summary>
        /// set to one of the following values: send, receive, generate, immature, orphan, move
        /// </summary>
        private string category;
        public string Category
        {
            get { return category; }
            set
            {
                category = value;
                OnChanged("Category");
            }
        }

        private long amount;
        public long Amount
        {
            get { return amount; }
            set
            {
                amount = value;
                OnChanged("Amount");
            }
        }

        private long totalInput;
        public long TotalInput
        {
            get { return totalInput; }
            set
            {
                totalInput = value;
                OnChanged("TotalInput");
            }
        }

        private long totalOutput;
        public long TotalOutput
        {
            get { return totalOutput; }
            set
            {
                totalOutput = value;
                OnChanged("TotalOutput");
            }
        }

        private long fee;
        public long Fee
        {
            get { return fee; }
            set
            {
                fee = value;
                OnChanged("Fee");
            }
        }

        private string txId;
        public string TxId
        {
            get { return txId; }
            set
            {
                txId = value;
                OnChanged("TxId");
            }
        }

        private long vout;
        public long Vout
        {
            get { return vout; }
            set
            {
                vout = value;
                OnChanged("Vout");
            }
        }

        private long time;
        public long Time
        {
            get { return time; }
            set
            {
                time = value;
                OnChanged("Time");
            }
        }
        private long size;
        public long Size
        {
            get { return size; }
            set
            {
                size = value;
                OnChanged("Size");
            }
        }

        private string comment;
        public string Comment
        {
            get { return comment; }
            set
            {
                comment = value;
                OnChanged("Comment");
            }
        }

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

        private string blockHash;
        public string BlockHash
        {
            get { return blockHash; }
            set
            {
                blockHash = value;
                OnChanged("BlockHash");
            }
        }

        private long blockIndex;
        public long BlockIndex
        {
            get { return blockIndex; }
            set
            {
                blockIndex = value;
                OnChanged("BlockIndex");
            }
        }

        private long blockTime;
        public long BlockTime
        {
            get { return blockTime; }
            set
            {
                blockTime = value;
                OnChanged("BlockTime");
            }
        } 
    }
}
