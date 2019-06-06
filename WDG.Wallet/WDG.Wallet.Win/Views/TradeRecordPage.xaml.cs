// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WDG.Wallet.Win.Views
{
    /// <summary>
    /// TradeRecordPage.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(IPage))]
    public partial class TradeRecordPage : Page ,IPage
    {
        public TradeRecordPage()
        {
            InitializeComponent();
        }

        public Page GetCurrentPage()
        {
            return this;
        }

        public string GetPageName()
        {
            return Pages.TradeRecordPage;
        }

        private void UsersDataGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalOffset + e.ViewportHeight == e.ExtentHeight)
            {
                if (e.VerticalChange > 0)
                {
                    GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<bool>(true, Pages.TradeRecordPage);
                }
            }
        }

        private void UsersDataGrid_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta > 0 && UsersDataGrid.VerticalScrollBarVisibility == ScrollBarVisibility.Visible)
            {
                var scrollViewers = FindChildren<ScrollViewer>(UsersDataGrid);
                if (scrollViewers == null)
                    return;
                var scrollViewer = scrollViewers.FirstOrDefault();
                if (scrollViewer != null && scrollViewer.ScrollableHeight == scrollViewer.VerticalOffset)
                {
                    GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<bool>(true, Pages.TradeRecordPage);
                }
            }
        }

        /// <summary>
        /// FindChildren
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <returns></returns>
        public IEnumerable<T> FindChildren<T>(DependencyObject parent) where T : class
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);
            if (count > 0)
            {
                for (var i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);
                    var t = child as T;
                    if (t != null)
                        yield return t;

                    var children = FindChildren<T>(child);
                    foreach (var item in children)
                        yield return item;
                }
            }
        }

    }
}
