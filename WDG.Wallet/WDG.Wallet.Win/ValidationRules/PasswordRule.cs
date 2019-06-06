// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace WDG.Wallet.Win.ValidationRules
{
    public class PasswordRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string password = value as string;

            if (!string.IsNullOrWhiteSpace(password))
            {
                var isMatch = Regex(password);

                // 检查输入的字符串是否符合IP地址格式
                if (!isMatch)
                {
                    return new ValidationResult(false, LanguageService.Default.GetLanguageValue(ValidationType.Error_Password.ToString()));
                }
            }
            return new ValidationResult(true, null);
        }

        public static bool Regex(string content)
        {
            var regex = new Regex(@"((?=.*[0-9])(?=.*[a-zA-Z])(?=.*[!@#$%^&*\.])[a-zA-Z0-9!@#$%^&*\._-]+)|((?=.*[0-9])(?=.*[a-zA-Z])[a-zA-Z0-9]+)|((?=.*[0-9])(?=.*[!@#$%^&*\.])[0-9!@#$%^&*\._-]+)|((?=.*[!@#$%^&*\.])(?=.*[a-zA-Z])[a-zA-Z!@#$%^&*\._-]+).{7,30}");

            var result = false;
            // 检查输入的字符串是否符合IP地址格式
            var mathches = regex.Matches(content).Cast<Match>();
            var gs = mathches.SelectMany(x => x.Groups.Cast<Group>());
            if (gs.Any(x => x.Value.Equals(content) && x.Value.Length >= 8 && x.Value.Length <= 30))
                result = true;
            else
                result = false;
            return result;
        }
    }
}
