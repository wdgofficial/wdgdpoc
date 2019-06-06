// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Models;
using WDG.Wallet.Win.Common;
using System;
using System.Globalization;
using System.Windows.Data;

namespace WDG.Wallet.Win.Converters
{
    public class ConfirmationToStatus : IValueConverter
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
                else
                    return LanguageService.Default.GetLanguageValue("converter_confirming");
            }
            else
            {
                if (payment.Confirmations >= 100)
                    return LanguageService.Default.GetLanguageValue("converter_confirmed");
                else
                    return LanguageService.Default.GetLanguageValue("converter_confirming");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
