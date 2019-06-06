// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.Data;
using FiiiChain.Data.Accesses;
using FiiiChain.Data.Dacs;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.Business
{
    public class UserSettingComponent
    {
        public string GetDefaultAccount()
        {
            var dac = UserSettingDac.Default;
            var setting = dac.SelectByType(Entities.UserSettingType.DefaultAccount);
            if (setting == null)
            {
                var accpuntDac = AccountDac.Default;
                var account = accpuntDac.SelectFirstDefaultAccount().Id;
                SetDefaultAccount(account);
                return account;
            }
            return setting.Value;
        }

        public void SetDefaultAccount(string id)
        {
            var dac = UserSettingDac.Default;
            dac.Upsert(new Entities.UserSetting { Type = Entities.UserSettingType.DefaultAccount, Value = id });
        }

        public void SetEnableAutoAccount(bool enable)
        {
            var dac = UserSettingDac.Default;
            dac.Upsert(new Entities.UserSetting { Type = Entities.UserSettingType.EnableAutoAccount, Value = enable.ToString() });
        }
    }
}
