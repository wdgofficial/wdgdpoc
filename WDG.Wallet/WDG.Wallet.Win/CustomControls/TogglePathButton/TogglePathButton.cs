// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace WDG.Wallet.Win.CustomControls
{
    public class TogglePathButton : ToggleButton
    {
        public TogglePathButton()
        {
            var resourceLoactor = new Uri("pack://application:,,,/WDG.Wallet.Win;component/CustomControls/TogglePathButton/TogglePathButton.xaml");
            ResourceDictionary resource = new ResourceDictionary();
            resource.Source = resourceLoactor;
            if (!Application.Current.Resources.MergedDictionaries.Any(x => x.Source == resourceLoactor))
                Application.Current.Resources.MergedDictionaries.Add(resource);
        }

        public Geometry PathData
        {
            get { return (Geometry)GetValue(PathDataProperty); }
            set { SetValue(PathDataProperty, value); }
        }

        public static readonly DependencyProperty PathDataProperty =
            DependencyProperty.Register("PathData", typeof(Geometry), typeof(TogglePathButton), new PropertyMetadata(new PathGeometry()));

        public Brush DefaultFillBrush
        {
            get { return (Brush)GetValue(DefaultFillBrushProperty); }
            set { SetValue(DefaultFillBrushProperty, value); }
        }

        public static readonly DependencyProperty DefaultFillBrushProperty =
            DependencyProperty.Register("DefaultFillBrush", typeof(Brush), typeof(TogglePathButton), new PropertyMetadata(Brushes.DarkGray));

        public Brush MouseOverBrush
        {
            get { return (Brush)GetValue(MouseOverBrushProperty); }
            set { SetValue(MouseOverBrushProperty, value); }
        }

        public static readonly DependencyProperty MouseOverBrushProperty =
            DependencyProperty.Register("MouseOverBrush", typeof(Brush), typeof(TogglePathButton), new PropertyMetadata(Brushes.DeepSkyBlue));

        public Brush IsPressedBrush
        {
            get { return (Brush)GetValue(IsPressedBrushProperty); }
            set { SetValue(IsPressedBrushProperty, value); }
        }

        public static readonly DependencyProperty IsPressedBrushProperty =
            DependencyProperty.Register("IsPressedBrush", typeof(Brush), typeof(TogglePathButton), new PropertyMetadata(Brushes.DodgerBlue));

        public Brush IsEnabledBrush
        {
            get { return (Brush)GetValue(IsEnabledBrushProperty); }
            set { SetValue(IsEnabledBrushProperty, value); }
        }

        public static readonly DependencyProperty IsEnabledBrushProperty =
            DependencyProperty.Register("IsEnabledBrush", typeof(Brush), typeof(TogglePathButton), new PropertyMetadata(Brushes.LightGray));

    }
}
