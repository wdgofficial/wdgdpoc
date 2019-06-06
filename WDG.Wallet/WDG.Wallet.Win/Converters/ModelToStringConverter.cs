// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WDG.Wallet.Win.Converters
{
    public enum ModelType
    {
        AccountInfo,
        PageUnspent
    }

    public static class Extensions
    {
        public static string GetTag(this PageUnspent unspentUtxo)
        {
            var amount = StaticConverters.LongToDoubleConverter.Convert(unspentUtxo.Amount, typeof(object), null, StaticConverters.CultureInfo);
            var amountStr = amount == null ? "Null" : amount.ToString();
            var result = string.Format("{0}({1})", unspentUtxo.Address, amountStr);
            return result;
        }

        public static string GetTag(this AccountInfo accountInfo)
        {
            if (string.IsNullOrEmpty(accountInfo.Tag))
                return string.Format("{0}", accountInfo.Address);
            else
                return string.Format("{0}({1})", accountInfo.Address, accountInfo.Tag);
        }
    }

    public class ModelToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null || !(parameter is ModelType))
                return null;

            var result = "";
            var model = (ModelType)parameter;
            switch (model)
            {
                case ModelType.AccountInfo:
                    if (value is AccountInfo)
                    {
                        result = ((AccountInfo)value).GetTag();
                    }
                    break;
                case ModelType.PageUnspent:
                    if (value is PageUnspent)
                    {
                        result = ((PageUnspent)value).GetTag();
                    }
                    break;
                default:
                    break;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
