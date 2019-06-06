// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDG.Wallet.Win.Models.CommonParams
{
    public class TitleWithParams<T> : NotifyBase
    {
        private string _title;

        public string Title
        {
            get { return _title; }
            set { _title = value; RaisePropertyChanged("Title"); }
        }

        private T _params;

        public T Params
        {
            get { return _params; }
            set { _params = value;  RaisePropertyChanged("Params"); }
        }


    }
}
