// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace WDG.Wallet.Win.CustomControls
{
    public class ToggleImageButton : ToggleButton
    {
        public ToggleImageButton()
        {
            var resourceLoactor = new Uri("pack://application:,,,/WDG.Wallet.Win;component/CustomControls/ToggleImageButton/ToggleImageButtonStyle.xaml");
            ResourceDictionary resource = new ResourceDictionary();
            resource.Source = resourceLoactor;
            if (!Application.Current.Resources.MergedDictionaries.Any(x => x.Source == resourceLoactor))
                Application.Current.Resources.MergedDictionaries.Add(resource);
        }

        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
        
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(ToggleImageButton), new FrameworkPropertyMetadata(default(ImageSource)));
        

    }
}
