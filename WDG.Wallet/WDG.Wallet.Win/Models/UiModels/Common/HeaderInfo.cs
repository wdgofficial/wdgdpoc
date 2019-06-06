// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WDG.Wallet.Win.Models
{
    [XmlRoot("HeaderInfo")]
    public class HeaderInfo : ViewModelBase
    {
        public HeaderInfo()
        {

        }

        public HeaderInfo(string header)
        {
            this.Header = header;
        }

        private string _header;
        private string _icon;
        private string _pageName;
        private bool _isSelected;
        private string _description;

        [XmlIgnore]
        public string Header
        {
            get { return _header; }
            set { _header = value; RaisePropertyChanged("Header"); }
        }

        [XmlAttribute("Header")]
        public string HeaderKey { get; set; }

        [XmlAttribute("Icon")]
        public string Icon
        {
            get { return _icon; }
            set { _icon = value; RaisePropertyChanged("Icon"); }
        }

        [XmlAttribute("PageName")]
        public string PageName
        {
            get { return _pageName; }
            set { _pageName = value; RaisePropertyChanged("PageName"); }
        }

        [XmlAttribute("IsSelected")]
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; RaisePropertyChanged("IsSelected"); }
        }

        [XmlIgnore]
        public string Description
        {
            get { return _description; }
            set { _description = value; RaisePropertyChanged("Description"); }
        }

        [XmlAttribute("Description")]
        public string DescriptionKey { get; set; }
    }
}
