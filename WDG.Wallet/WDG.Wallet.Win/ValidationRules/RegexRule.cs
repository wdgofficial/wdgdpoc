// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace WDG.Wallet.Win.ValidationRules
{
    public class RegexRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty(Pattern))
                return new ValidationResult(true, null);

            if (value == null)
                return new ValidationResult(false, LanguageService.Default.GetLanguageValue(ErrorMsg));

            var text = value.ToString();
            
            if (!MatchAll(text, Pattern))
                return new ValidationResult(false, LanguageService.Default.GetLanguageValue(ErrorMsg));
            else
                return new ValidationResult(true, null);
        }

        public bool MatchAll(string content, string pattern)
        {
            if (content == null)
                return false;
            if (!string.IsNullOrWhiteSpace(pattern))
            {
                var regex = new Regex(pattern);
                
                if (regex.IsMatch(content) && regex.Match(content).Value == content)
                    return true;
                else
                    return false;
            }
            return true;
        }


        public string Pattern { get; set; }

        public string ErrorMsg { get; set; }
    }
}
