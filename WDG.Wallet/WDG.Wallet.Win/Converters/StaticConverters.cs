// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace WDG.Wallet.Win.Converters
{
    public static class StaticConverters
    {
        private static Dictionary<Type, IValueConverter> keyValuePairs = new Dictionary<Type, IValueConverter>();

        public static CultureInfo CultureInfo = new CultureInfo(1033);

        public static LongToDoubleConverter LongToDoubleConverter
        {
            get
            {
                return GetValue<LongToDoubleConverter>();
            }
        }

        public static PagerConverter PagerConverter
        {
            get
            {
                return GetValue<PagerConverter>();
            }
        }

        public static ModelToStringConverter ModelToStringConverter
        {
            get
            {
                return GetValue<ModelToStringConverter>();
            }
        }

        public static InitStepToStringConverter InitStepToStringConverter
        {
            get
            {
                return GetValue<InitStepToStringConverter>();
            }
        }

        private static T GetValue<T>() where T : IValueConverter
        {
            var currentType = typeof(T);
            if (keyValuePairs.ContainsKey(currentType))
            {
                return (T)keyValuePairs[currentType];
            }
            else
            {
                var instance = System.Activator.CreateInstance<T>();
                keyValuePairs.Add(currentType, instance);
                return instance;
            }
        }
    }


    public static class StaticMultiConverters
    {
        private static Dictionary<Type, IMultiValueConverter> keyValuePairs = new Dictionary<Type, IMultiValueConverter>();
        
        private static T GetValue<T>() where T : IMultiValueConverter
        {
            var currentType = typeof(T);
            if (keyValuePairs.ContainsKey(currentType))
            {
                return (T)keyValuePairs[currentType];
            }
            else
            {
                var instance = System.Activator.CreateInstance<T>();
                keyValuePairs.Add(currentType, instance);
                return instance;
            }
        }


        public static MultiLongConverter MultiLongConverter
        {
            get
            {
                return GetValue<MultiLongConverter>();
            }
        }

    }


    public static class StaticConverterParams
    {
        public static PagerType PagerType_CurrentPage = PagerType.CurrentPage;
        public static PagerType PagerType_TotalCount = PagerType.TotalCount;
        public static PagerType PagerType_TotalPage = PagerType.TotalPage;

        public static ModelType ModelType_AccountInfo = ModelType.AccountInfo;
        public static ModelType ModelType_PageUnspent = ModelType.PageUnspent;

    }


}
