﻿// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace WDG.Wallet.Win.Views
{
    /// <summary>
    /// PopupShell.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(IPage))]
    public partial class PopupShell : Page,IPage
    {
        public PopupShell()
        {
            InitializeComponent();
        }

        public Page GetCurrentPage()
        {
            return this;
        }

        public string GetPageName()
        {
            return Pages.PopupShell;
        }
    }
}
