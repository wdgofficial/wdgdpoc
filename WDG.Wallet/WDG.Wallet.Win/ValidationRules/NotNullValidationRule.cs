// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common;
using System.Windows.Controls;

namespace WDG.Wallet.Win.ValidationRules
{
    public class NotNullValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty(value as string) || string.IsNullOrWhiteSpace(value as string))
            {
                return new ValidationResult(false, LanguageService.Default.GetLanguageValue(ValidationType.Error_NotNull.ToString()));
            }
            return new ValidationResult(true, null);
        }
    }
}
