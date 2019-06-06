// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace WDG.Wallet.Win.Views
{
    /// <summary>
    /// OverviewPage.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(IPage))]
    public partial class OverviewPage : Page , IPage
    {
        public OverviewPage()
        {
            InitializeComponent();
        }

        public Page GetCurrentPage()
        {
            return this;
        }

        public string GetPageName()
        {
            return Pages.OverviewPage;
        }
    }
}
