// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

using System;

namespace WDG.Models
{
    public class SendManyModel : BaseModel
    {
        private string address;
        public String Address
        {
            get { return address; }
            set
            {
                address = value;
                OnChanged("Address");
            }
        }

        private string tag;
        public string Tag
        {
            get { return tag; }
            set
            {
                tag = value;
                OnChanged("Tag");
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
    }
}