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
    public class MessageKeys
    {
        public const string Backup_Sucesses = "backupSucesses";
        public const string Backup_Fail = "backupFail";

        public const string Export_Sucesses = "exportSucesses";
        public const string Export_Fail = "exportFail";
        public const string EnterPwdFail = "enterPwd_fail";
        public const string Import_sucesses = "importSucesses";
        public const string Import_Fail = "importFail";
        public const string SetDefault_Sucesses = "setDefaultSucesses";
        public const string SetDefault_Fail = "setDefaultFail";

        public const string Add_Sucesses = "addSucesses";
        public const string Add_Fail = "addFail";
        public const string Edit_Fail = "Error_Edit";
        public const string Error_TradeAmount = "Error_TradeAmount";
        public const string Address_Existed = "addressExisted";
        public const string Delete_Fail = "DeleteFail";

        public const string Msg_Sendfailure = "Msg_Sendfailure";
        public const string Msg_Sendsuccess = "Msg_Sendsuccess";

        public const string Error_SendOverMax = "Error_Send_OverMax";
        public const string Error_SendOverMinFee = "Error_Send_OverMinFee";

        public const string Error_Uri = "Error_Uri";
        public const string Error_Address = "Error_Address";
    }
}