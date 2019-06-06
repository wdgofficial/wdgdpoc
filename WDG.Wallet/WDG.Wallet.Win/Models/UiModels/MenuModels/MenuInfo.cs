// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace WDG.Wallet.Win.Models
{
    [XmlRoot("MenuInfo")]
    public class MenuInfo : MenuBase
    {
        public MenuInfo()
        {
            this.MenuType = MenuType.Item;
        }

        public MenuInfo(string header) : base()
        {
            this.Header = header;
        }

        private string _header;
        private string _icon;
        private string _token;
        private ObservableCollection<MenuBase> _menuItems;
        public string _viewName;
        private bool _isEnabled = true;


        [XmlIgnore]
        public string Header
        {
            get { return _header; }
            set { _header = value; RaisePropertyChanged("Header"); }
        }
        
        [XmlIgnore]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; RaisePropertyChanged("IsEnabled"); }
        }
        
        [XmlAttribute("Header")]
        public string HeaderKey { get; set; }
        
        [XmlAttribute("Icon")]
        public string Icon
        {
            get { return _icon; }
            set { _icon = value; RaisePropertyChanged("Icon"); }
        }

        [XmlArrayItem(Type = typeof(MenuBase),ElementName = "MenuItem")]
        public ObservableCollection<MenuBase> MenuItems
        {
            get
            {
                if (_menuItems == null)
                    _menuItems = new ObservableCollection<MenuBase>();
                return _menuItems;
            }
            set { _menuItems = value; RaisePropertyChanged("MenuItems"); }
        }

        [XmlAttribute("Token")]
        public string Token
        {
            get
            {
                return _token;
            }
            set
            {
                _token = value;
                RaisePropertyChanged("Token");
            }
        }

        [XmlAttribute("ViewName")]
        public string ViewName
        {
            get
            {
                return _viewName;
            }
            set
            {
                _viewName = value;
                RaisePropertyChanged("ViewName");
            }
        }
    }
}
