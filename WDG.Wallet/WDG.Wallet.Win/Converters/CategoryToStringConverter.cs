// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace WDG.Wallet.Win.Converters
{
    public class CategoryToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var category = value.ToString();
            var categoryType = Enum.Parse(typeof(PaymentCategoryType), category);
            string result = LanguageService.Default.GetLanguageValue(categoryType.ToString());
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var category = value.ToString();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (PaymentCategoryType suit in Enum.GetValues(typeof(PaymentCategoryType)))
            {
                var key = suit.ToString();
                dic.Add(key, LanguageService.Default.GetLanguageValue(key));
            }

            if (dic.ContainsValue(category))
            {
                var keyvalue= dic.FirstOrDefault(x => x.Value == category);
                return keyvalue.Key.ToString();
            }

            return null;
        }
    }
}
