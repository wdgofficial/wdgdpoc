// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common;
using System.Windows.Controls;

namespace WDG.Wallet.Win.ValidationRules
{
    public abstract class RuleBase : ValidationRule
    {
        public string ErrorKey { get; set; }

        public string GetErrorMsg()
        {
            return LanguageService.Default.GetLanguageValue(ErrorKey);
        }
    }
}
