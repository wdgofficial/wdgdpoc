// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using GalaSoft.MvvmLight;
using System.Xml.Serialization;

namespace WDG.Wallet.Win.Models
{
    [XmlRoot("MenuBase")]
    [XmlInclude(typeof(MenuInfo))]
    [XmlInclude(typeof(MenuSeparator))]
    public abstract class MenuBase : ViewModelBase
    {
        [XmlAttribute("MenuType")]
        public MenuType MenuType
        {
            get;
            set;
        }
    }

    public enum MenuType
    {
        Item,
        Separator
    }
}