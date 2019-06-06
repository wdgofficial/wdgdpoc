// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace WDG.Wallet.Win.Models
{
    [XmlRoot("AppSettingConfig")]
    public class AppSetting
    {
        public AppSetting()
        {
            MenuItems = new List<MenuInfo>();
            MainWindowTabs = new List<HeaderInfo>();
        }

        [XmlArrayItem(Type = typeof(MenuInfo), ElementName = "MenuItem")]
        public List<MenuInfo> MenuItems;

        [XmlArrayItem(Type = typeof(HeaderInfo), ElementName = "TabHeader")]
        public List<HeaderInfo> MainWindowTabs;
        
        [XmlArrayItem(Type = typeof(TimeGoalItem), ElementName = "TimeGoalItem")]
        public List<TimeGoalItem> TimeGoalItems;

        [XmlElement(Type = typeof(LanguageType))]
        public LanguageType LanguageType { get; set; }

        [XmlElement(Type = typeof(WalletModel))]
        public WalletModel WalletModel { get; set; }
    }
}
