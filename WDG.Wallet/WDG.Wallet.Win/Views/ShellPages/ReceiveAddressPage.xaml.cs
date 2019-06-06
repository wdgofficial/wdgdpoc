// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace WDG.Wallet.Win.Views.ShellPages
{
    /// <summary>
    /// ReceiveAddressPage.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(IPage))]
    public partial class ReceiveAddressPage : Page, IPage
    {
        public ReceiveAddressPage()
        {
            InitializeComponent();
        }

        public Page GetCurrentPage()
        {
            return this;
        }

        public string GetPageName()
        {
            return Pages.ReceiveAddressPage;
        }
    }
}
