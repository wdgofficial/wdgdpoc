// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDG.Wallet.Win.Models
{
    public class SendMsgData<Token_T1, Ref_T2> : SendMsgData<Token_T1>
    {
        public Ref_T2 RefData { get; set; }
    }

    public class SendMsgData<T1>
    {
        public T1 Token { get; set; }
        public object CallBackParams { get; set; }

        private Action _callBack { get; set; }

        public void SetCallBack(Action action)
        {
            _callBack = action;
        }

        public void CallBack()
        {
            if (_callBack != null)
                _callBack.Invoke();
        }
    }
}
