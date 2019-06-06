// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.Entities
{
    public class UserSetting
    {
        public int Id;
        public UserSettingType Type;
        public string Value;
    }

    public enum UserSettingType
    {
        DefaultAccount = 0,
        EnableAutoAccount = 1
    }
}