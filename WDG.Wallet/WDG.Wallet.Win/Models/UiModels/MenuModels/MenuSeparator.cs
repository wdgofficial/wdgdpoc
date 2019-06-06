// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System.Xml.Serialization;

namespace WDG.Wallet.Win.Models
{

    [XmlRoot("MenuSeparator")]
    public class MenuSeparator : MenuBase
    {
        public MenuSeparator()
        {
            this.MenuType = MenuType.Separator;
        }
    }
}
