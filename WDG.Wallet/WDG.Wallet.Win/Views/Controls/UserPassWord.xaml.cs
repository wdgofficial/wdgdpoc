// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System.Windows;
using System.Windows.Controls;

namespace WDG.Wallet.Win.Views.Controls
{
    public partial class UserPassWord : UserControl
    {
        public UserPassWord()
        {
            InitializeComponent();
        }




        public string WaterMark
        {
            get { return (string)GetValue(WaterMarkProperty); }
            set { SetValue(WaterMarkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WaterMark.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WaterMarkProperty =
            DependencyProperty.Register("WaterMark", typeof(string), typeof(UserPassWord), new PropertyMetadata(string.Empty));


        public bool IsShowPassword
        {
            get { return (bool)GetValue(IsShowPasswordProperty); }
            set { SetValue(IsShowPasswordProperty, value); }
        }

        public static readonly DependencyProperty IsShowPasswordProperty =
            DependencyProperty.Register("IsShowPassword", typeof(bool), typeof(UserPassWord), new PropertyMetadata(false, IsShowPasswordPropertyChanged));


        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(UserPassWord), new UIPropertyMetadata(string.Empty, PasswordPropertyChanged));

        static void IsShowPasswordPropertyChanged(DependencyObject dependency, DependencyPropertyChangedEventArgs eventArgs)
        {
            var ctl = (UserPassWord)dependency;
            if (ctl.IsShowPassword)
            {
                ctl.textBox.Text = ctl.Password;
                ctl.textBox.Visibility = Visibility.Visible;
                ctl.passwordBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                ctl.passwordBox.Password = ctl.Password;
                ctl.textBox.Visibility = Visibility.Collapsed;
                ctl.passwordBox.Visibility = Visibility.Visible;
            }
        }

        static void PasswordPropertyChanged(DependencyObject dependency, DependencyPropertyChangedEventArgs eventArgs)
        {
            var ctl = (UserPassWord)dependency;
            if (string.IsNullOrEmpty(ctl.Password))
            {
                ctl.textblock.Visibility = Visibility.Visible;
                ctl.textBox.Text = "";
                ctl.passwordBox.Password = "";
            }
            else
            {
                ctl.textblock.Visibility = Visibility.Collapsed;
            }
        }


        private void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!this.IsKeyboardFocusWithin && string.IsNullOrEmpty(this.passwordBox.Password))
            {
                this.passwordBox.Password = Password;
                return;
            }
            Password = this.passwordBox.Password;
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Password = this.textBox.Text;
        }
    }
}
