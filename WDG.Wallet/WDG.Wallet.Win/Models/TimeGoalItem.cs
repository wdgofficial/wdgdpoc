// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common;
using System.Xml.Serialization;

namespace WDG.Wallet.Win.Models
{
    [XmlRoot]
    public class TimeGoalItem : VmBase
    {
        private string _value;

        [XmlIgnore]
        public string Value { get { return _value; } set { _value = value; RaisePropertyChanged("Value"); } }

        [XmlAttribute("Value")]
        public string ValueKey { get; set; }

        [XmlAttribute("Key")]
        public double Key { get; set; }
    }
}
