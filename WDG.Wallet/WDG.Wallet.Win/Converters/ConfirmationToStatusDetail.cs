// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Models;
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Models;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace WDG.Wallet.Win.Converters
{
    public class ConfirmationToStatusDetail : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            var payment = (Payment)value;
            if (payment == null)
                return null;

            if (payment.Confirmations <= 0)
            { 
                return LanguageService.Default.GetLanguageValue("converter_unconfirmed");
            }
            if (payment.Category != "generate")
            {
                if (payment.Confirmations >= 6)
                    return LanguageService.Default.GetLanguageValue("converter_confirmed");
                var format = LanguageService.Default.GetLanguageValue("converter_confirmFormat") + "{0}/6";
                return string.Format(format, payment.Confirmations);
            }
            else
            {
                if (payment.Confirmations >= 100)
                    return LanguageService.Default.GetLanguageValue("converter_confirmed");
                var format = LanguageService.Default.GetLanguageValue("converter_confirmFormat") + "{0}/100";
                return string.Format(format, payment.Confirmations);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (value == null || !(value is string))
            //    return null;
            


            //var txt = value.ToString();
            //if (txt ==  LanguageService.Default.GetLanguageValue("converter_confirmed"))
            //    return 6;
            //if (txt == LanguageService.Default.GetLanguageValue("converter_unconfirmed"))
            //    return 0;
            //var format = LanguageService.Default.GetLanguageValue("converter_confirmFormat");
            //var pattern = format + "(?'value'[^/]+)/6";
            //Regex regex = new Regex(pattern);
            //var match = regex.Match(pattern);
            //var result = match.Groups["value"].Value;

            //int i = 0;
            //if (int.TryParse(result, out i))
            //{
            //    return i;
            //}
            return null;
        }
    }
}
