// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDG.Wallet.Win.Models
{
    public class ReceiveInfo : NotifyBase
    {
        private string _tag;

        public string Tag
        {
            get { return _tag; }
            set { _tag = value; RaisePropertyChanged("Tag"); }
        }

        private string _comment;

        public string Comment
        {
            get { return _comment; }
            set { _comment = value; RaisePropertyChanged("Comment"); }
        }

        private long _amount;

        public long Amount
        {
            get { return _amount; }
            set
            {
                _amount = value;
                _amount_Double = _amount / Math.Pow(10, 8);
                RaisePropertyChanged("Amount_Double");
                RaisePropertyChanged("Amount");
            }
        }

        private string _account;

        public string Account
        {
            get { return _account; }
            set { _account = value; RaisePropertyChanged("Account"); }
        }

        private double _amount_Double;

        public double Amount_Double
        {
            get { return _amount_Double; }
            set
            {
                if (_amount_Double == value)
                    return;
                _amount_Double = value;
                _amount = Convert.ToInt64(_amount_Double * Math.Pow(10, 8));
                RaisePropertyChanged("Amount_Double");
                RaisePropertyChanged("Amount");
            }
        }

        private string _amount_Str;

        public string Amount_Str
        {
            get { return _amount_Str; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    Amount_Double = double.Parse(value);
                }
                else
                {
                    Amount_Double = 0;
                }
                _amount_Str = value;
                
                RaisePropertyChanged("Amount_Str");
                RaisePropertyChanged("Amount_Double");
                RaisePropertyChanged("Amount");
            }
        }

    }

    public enum ReceiveClearType
    {
        All,
        Tag,
        Comment
    }

}
