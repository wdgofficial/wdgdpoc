// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDG.Wallet.Win.Common
{
    public abstract class InstanceBase<T> where T : class
    {
        private static T instance = null;
        public static T Default
        {
            get
            {
                if (instance == null)
                {
                    try
                    {
                        instance = Activator.CreateInstance<T>();
                    }
                    catch
                    {
                        throw new Exception("InstanceBase Create a Default Value Fail");
                    }
                }
                return instance;
            }
            protected set
            {
                instance = value;
            }
        }
    }
}
