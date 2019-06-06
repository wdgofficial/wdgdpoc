// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.Windows.Controls;

namespace WDG.Wallet.Win.Views.ShellPages
{
    /// <summary>
    /// AboutView.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(IPage))]
    public partial class AboutPage : Page ,IPage
    {
        public AboutPage()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                webBrowser.ObjectForScripting = new WpfSender();
            };
        }

        public Page GetCurrentPage()
        {
            return this;
        }

        public string GetPageName()
        {
            return Pages.AboutPage;
        }
    }

    [ComVisible(true)]
    public class WpfSender
    {
        public void Jump(string url)
        {
            System.Diagnostics.Process.Start(url);
        }
    }
}
