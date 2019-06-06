// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Models;
using System;

namespace WDG.Wallet.Win.Models
{
    public class SendItemInfo : SendManyModel
    {
        private double _payAmount;
        public double PayAmount
        {
            get
            {
                return _payAmount;
            }
            set
            {
                if (_payAmount == value)
                    return;
                _payAmount = value;
                
                UpdateAmount();
                OnChanged("PayAmount");
                OnChanged("Amount");
            }
        }

        private bool _isContainFee;
        public bool IsContainFee
        {
            get
            {
                return _isContainFee;
            }
            set
            {
                _isContainFee = value;
                OnChanged("IsContainFee");
            }
        }
        
        double _divisor = Math.Pow(10, 8);

        private void UpdateAmount()
        {
            Amount = Convert.ToInt64(_payAmount * _divisor);
        }


        private string _payAmountStr;
        public string PayAmountStr
        {
            get
            {
                return _payAmountStr;
            }
            set
            {
                if (_payAmountStr == value)
                    return;
                _payAmountStr = value;

                if (string.IsNullOrEmpty(value))
                    PayAmount = 0;
                else
                    PayAmount = double.Parse(value);

                OnChanged("PayAmountStr");
                OnChanged("PayAmount");
                OnChanged("Amount");
            }
        }
    }
}
