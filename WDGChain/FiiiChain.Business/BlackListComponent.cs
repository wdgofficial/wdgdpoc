// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.Messages;
using FiiiChain.Entities;
using FiiiChain.DataAgent;
using System;
using System.Collections.Generic;
using System.Text;
using FiiiChain.Data;
using FiiiChain.Framework;
using FiiiChain.Consensus;
using System.Linq;


namespace FiiiChain.Business
{
    public class BlackListComponent
    {
        public bool Add(string address, long? expiredTime)
        {
            var dac = BlackListDac.Default;

            if(!dac.CheckExists(address))
            {
                dac.Save(address, expiredTime);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Delete(string address)
        {
            var dac = BlackListDac.Default;

            if (!dac.CheckExists(address))
            {
                return false;
            }
            else
            {
                dac.Delete(address);
                return true;
            }
        }

        public void Clear()
        {
            BlackListDac.Default.DeleteAll();
        }

        public bool Exists(string address)
        {
            return BlackListDac.Default.CheckExists(address);
        }
    }
}
