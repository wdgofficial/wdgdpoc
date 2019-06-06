// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common.Keys;
using WDG.Wallet.Win.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;

namespace WDG.Wallet.Win.Common
{
    public delegate void LanguageChanged(LanguageType languageType);

    public class LanguageService : InstanceBase<LanguageService>
    {
        public event LanguageChanged OnLanguageChanged;

        public LanguageService()
        {
            LanguageEntry = new Dictionary<string, string>();
            ErrorCodes = new Dictionary<string, string>();
        }

        public LanguageType LanguageType { get; set; }

        private ResourceDictionary _languageResource { get; set; }

        public void SetLanguage(LanguageType languageType, AppSettingConfig appSetting)
        {
            SetLanguage(languageType);
            appSetting.SwitchLanguage();
        }

        public void SetLanguage(LanguageType languageType)
        {
            LanguageType = languageType;

            if (languageType == LanguageType.en_us)
                StaticViewModel.GlobalViewModel.IsEnglish = true;
            else
                StaticViewModel.GlobalViewModel.IsEnglish = false;
            //1.get resource uri
            string path = string.Format("pack://application:,,,/WDG.Wallet.Win;component/lang/{0}.xaml", languageType);
            var resourceLoactor = new Uri(path);

            //2.find the resource file in Application, if it exists, it must be deleted
            var resources = Application.Current.Resources;
            var languageResource = resources.MergedDictionaries.FirstOrDefault(x => x.Source == resourceLoactor);
            if (languageResource != null)
                resources.MergedDictionaries.Remove(languageResource);

            //3.Readd the language file to the top
            ResourceDictionary resource = new ResourceDictionary();
            resource.Source = resourceLoactor;
            Application.Current.Resources.MergedDictionaries.Add(resource);
            _languageResource = resource;

            ReLoadLanguageEntry();
            LoadErrorCodes(languageType);

            OnLanguageChanged?.Invoke(languageType);
        }

        private void LoadErrorCodes(LanguageType languageType)
        {
            var name = string.Format("error_{0}", languageType.ToString().Replace("_", ""));

            string path = string.Format("pack://application:,,,/WDG.Wallet.Win;component/lang/errorcodes/{0}.xaml", name);
            var resourceLoactor = new Uri(path);

            ResourceDictionary resource = new ResourceDictionary();
            resource.Source = resourceLoactor;

            ErrorCodes.Clear();
            var keys = resource.Keys.Cast<string>();
            foreach (var key in keys)
            {
                var value = resource[key];
                var str = value == null ? null : value.ToString();
                ErrorCodes.Add(key, str);
            }
        }


        public string GetLanguageValue(string key)
        {
            if (LanguageEntry.ContainsKey(key))
                return LanguageEntry[key];
            else
                return null;
        }


        public Dictionary<string, string> LanguageEntry { get; set; }

        void ReLoadLanguageEntry()
        {
            if (_languageResource == null)
                return;
            LanguageEntry.Clear();
            var keys = _languageResource.Keys.Cast<string>().ToList();
            keys.ForEach(key =>
            {
                var value = _languageResource[key];
                var str = value == null ? null : value.ToString();
                LanguageEntry.Add(key, str);
            });
        }


        public Dictionary<string, string> ErrorCodes { get; set; }

        public string GetErrorMsg(int errorCode)
        {
            var codeStr = errorCode.ToString("0000000");
            if (ErrorCodes == null || !ErrorCodes.Any() || !ErrorCodes.ContainsKey(codeStr))
                return "";
            return ErrorCodes[codeStr];
        }
    }

    [XmlRoot("LanguageType")]
    public enum LanguageType
    {
        Default = -1,
        zh_cn = 0,
        en_us =1
    }
}
