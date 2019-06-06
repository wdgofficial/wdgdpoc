// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System.Windows;
using System.Windows.Controls;

namespace WDG.Wallet.Win.Views.ShellPages
{
    /// <summary>
    /// TitleControl.xaml 的交互逻辑
    /// </summary>
    public partial class TitleControl : UserControl
    {
        public TitleControl()
        {
            InitializeComponent();
        }



        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(TitleControl), new PropertyMetadata("Title"));




        public bool CloseIsWord
        {
            get { return (bool)GetValue(CloseIsWordProperty); }
            set { SetValue(CloseIsWordProperty, value); }
        }
        
        public static readonly DependencyProperty CloseIsWordProperty =
            DependencyProperty.Register("CloseIsWord", typeof(bool), typeof(TitleControl), new PropertyMetadata(false));



        public string CloseWord
        {
            get { return (string)GetValue(CloseWordProperty); }
            set { SetValue(CloseWordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CloseWord.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CloseWordProperty =
            DependencyProperty.Register("CloseWord", typeof(string), typeof(TitleControl), new PropertyMetadata(""));


    }
}
