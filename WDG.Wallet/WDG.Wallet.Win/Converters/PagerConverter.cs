// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WDG.Wallet.Win.Converters
{
    public enum PagerType
    {
        CurrentPage,
        TotalPage,
        TotalCount
    }

    public class PagerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null || !(parameter is PagerType))
                return null;

            var parameterType = (PagerType)parameter;

            string stringFormat = "";
            switch (parameterType)
            {
                case PagerType.CurrentPage:
                    stringFormat = LanguageService.Default.GetLanguageValue("CurrentPageFormat");
                    break;
                case PagerType.TotalPage:
                    stringFormat = LanguageService.Default.GetLanguageValue("TotalPageFormat");
                    break;
                case PagerType.TotalCount:
                    stringFormat = LanguageService.Default.GetLanguageValue("TotalCountFormat");
                    break;
                default:
                    break;
            }
            if (string.IsNullOrEmpty(stringFormat))
                return null;

            return string.Format(stringFormat, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}