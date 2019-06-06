// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common;
using GFramework.BlankWindow;
using System.ComponentModel.Composition;
using System.Windows;

namespace WDG.Wallet.Win
{
    /// <summary>
    /// Shell.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(IShell))]
    public partial class Shell : BlankWindow, IShell
    {
        public Shell()
        {
            InitializeComponent();
        }

        public Window GetWindow()
        {
            return this;
        }

        private void Menu_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is System.Windows.Controls.MenuItem)
            {
                e.Handled = true;
            }
        }
    }
}
