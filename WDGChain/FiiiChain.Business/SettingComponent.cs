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
    public class SettingComponent
    {
        public void SaveSetting(Setting setting)
        {
            SettingDac.Default.SaveSetting(setting);
        }

        public Setting GetSetting()
        {
            var setting = SettingDac.Default.GetSetting();

            if(setting == null)
            {
                setting = new Setting();
                setting.Confirmations = 1;
                setting.FeePerKB = 100000;
            }

            return setting;
        }
    }
}
