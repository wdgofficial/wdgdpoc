// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Common.Utils;
using WDG.Wallet.Win.Models;
using WDG.Wallet.Win.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace WDG.Wallet.Win
{
    public class AppSettingConfig : SerializerBase<AppSettingConfig>
    {
        public AppSettingConfig()
        {
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppSetting.config");
            this.ConfigFilePath = configPath;
            _appSetting = this.LoadConfig<AppSetting>();
            if (_appSetting != null)
            {
                if (_appSetting.LanguageType == LanguageType.Default)
                {
                    var defaultLanguage = GetDefaultLanguage();
                    _appSetting.LanguageType = defaultLanguage;
                    UpdateConfig(defaultLanguage, AppConfig.WalletModel);
                }
                LanguageService.Default.SetLanguage(_appSetting.LanguageType, this);
                StaticViewModel.GlobalViewModel.WalletModel = _appSetting.WalletModel;
            }
        }

        private LanguageType GetDefaultLanguage()
        {
            try
            {
                var defaultLanguage = (LanguageType)Enum.Parse(typeof(LanguageType), Win32Util.GetSystemLanguage().ToString());
                return defaultLanguage;
            }
            catch
            {
                return LanguageType.en_us;
            }
        }


        private AppSetting _appSetting = null;
        public AppSetting AppConfig
        {
            get
            {
                if (_appSetting == null)
                    _appSetting = LoadConfig<AppSetting>();
                return _appSetting;
            }
        }

        protected override T LoadConfig<T>()
        {
            if (!File.Exists(ConfigFilePath))
                throw new FileNotFoundException();
            T result = default(T);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (StreamReader reader = new StreamReader(ConfigFilePath))
            {
                result = xmlSerializer.Deserialize(reader) as T;
            }
            return result;
        }

        public void SwitchLanguage()
        {
            if (AppConfig == null)
                return;

            if (_appSetting.MenuItems != null)
            {
                var menuItems = GetAllMenus(_appSetting.MenuItems);
                menuItems.ForEach(x => x.Header = LanguageService.Default.GetLanguageValue(x.HeaderKey));
            }

            if (_appSetting.MainWindowTabs != null)
            {
                var tabs = _appSetting.MainWindowTabs;
                tabs.ForEach(x =>
                {
                    x.Header = LanguageService.Default.GetLanguageValue(x.HeaderKey);
                    x.Description = LanguageService.Default.GetLanguageValue(x.DescriptionKey);
                });
            }

            if (_appSetting.TimeGoalItems != null)
            {
                _appSetting.TimeGoalItems.ForEach(x =>
                {
                    x.Value = LanguageService.Default.GetLanguageValue(x.ValueKey);
                });
            }
        }

        internal List<MenuInfo> GetAllMenus(IEnumerable<MenuBase> menuInfos)
        {
            var result = new List<MenuInfo>();
            if (menuInfos == null)
                return result;
            
            menuInfos.ToList().ForEach(x =>
            {
                if (x.MenuType == MenuType.Item)
                {
                    var item = (MenuInfo)x;
                    result.Add(item);
                    if (item.MenuItems != null && item.MenuItems.Any())
                    {
                        result.AddRange(GetAllMenus(item.MenuItems));
                    }
                }
            });
            return result;
        }

        public void UpdateConfig(LanguageType languageType, WalletModel walletModel)
        {
            var language = languageType.ToString().ToLower();
            var model = walletModel.ToString();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(ConfigFilePath);
            XmlNode xns = xmlDoc.SelectSingleNode("AppSettingConfig/LanguageType");
            xns.InnerText = language;
            XmlNode mdoelNode = xmlDoc.SelectSingleNode("AppSettingConfig/WalletModel");
            mdoelNode.InnerText = model;
            xmlDoc.Save(ConfigFilePath);
        }
    }

}
