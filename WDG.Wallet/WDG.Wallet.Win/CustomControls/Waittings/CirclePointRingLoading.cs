// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System.Windows;
using System.Windows.Controls;

namespace WDG.Wallet.Win.CustomControls.Waittings
{
    [TemplateVisualState(Name = "Large", GroupName = "SizeStates")]
    [TemplateVisualState(Name = "Small", GroupName = "SizeStates")]
    [TemplateVisualState(Name = "Inactive", GroupName = "ActiveStates")]
    [TemplateVisualState(Name = "Active", GroupName = "ActiveStates")]
    public partial class CirclePointRingLoading : Control
    {
        public double BindableWidth
        {
            get
            {
                return (double)base.GetValue(CirclePointRingLoading.BindableWidthProperty);
            }
            private set
            {
                base.SetValue(CirclePointRingLoading.BindableWidthProperty, value);
            }
        }

        private static void BindableWidthCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            CirclePointRingLoading circlePointRingLoading = dependencyObject as CirclePointRingLoading;
            bool flag = circlePointRingLoading == null;
            if (!flag)
            {
                circlePointRingLoading.SetEllipseDiameter((double)e.NewValue);
                circlePointRingLoading.SetEllipseOffset((double)e.NewValue);
                circlePointRingLoading.SetMaxSideLength((double)e.NewValue);
            }
        }

        private void SetEllipseDiameter(double width)
        {
            this.EllipseDiameter = width / 8.0;
        }

        private void SetEllipseOffset(double width)
        {
            this.EllipseOffset = new Thickness(0.0, width / 2.0, 0.0, 0.0);
        }

        private void SetMaxSideLength(double width)
        {
            this.MaxSideLength = ((width <= 20.0) ? 20.0 : width);
        }

        public bool IsActive
        {
            get
            {
                return (bool)base.GetValue(CirclePointRingLoading.IsActiveProperty);
            }
            set
            {
                base.SetValue(CirclePointRingLoading.IsActiveProperty, value);
            }
        }


        private static void IsActiveChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CirclePointRingLoading circlePointRingLoading = sender as CirclePointRingLoading;
            bool flag = circlePointRingLoading == null;
            if (!flag)
            {
                circlePointRingLoading.UpdateActiveState();
            }
        }


        private void UpdateActiveState()
        {
            bool isActive = this.IsActive;
            if (isActive)
            {
                VisualStateManager.GoToState(this, this.StateActive, true);
            }
            else
            {
                VisualStateManager.GoToState(this, this.StateInActive, true);
            }
        }

        public bool IsLarge
        {
            get
            {
                return (bool)base.GetValue(CirclePointRingLoading.IsLargeProperty);
            }
            set
            {
                base.SetValue(CirclePointRingLoading.IsLargeProperty, value);
            }
        }

        private static void IsLargeChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CirclePointRingLoading circlePointRingLoading = sender as CirclePointRingLoading;
            bool flag = circlePointRingLoading == null;
            if (!flag)
            {
                circlePointRingLoading.UpdateLargeState();
            }
        }

        private void UpdateLargeState()
        {
            bool isLarge = this.IsLarge;
            if (isLarge)
            {
                VisualStateManager.GoToState(this, this.StateLarge, true);
            }
            else
            {
                VisualStateManager.GoToState(this, this.StateSmall, true);
            }
        }

        public double MaxSideLength
        {
            get
            {
                return (double)base.GetValue(CirclePointRingLoading.MaxSideLengthProperty);
            }
            set
            {
                base.SetValue(CirclePointRingLoading.MaxSideLengthProperty, value);
            }
        }

        public double EllipseDiameter
        {
            get
            {
                return (double)base.GetValue(CirclePointRingLoading.EllipseDiameterProperty);
            }
            set
            {
                base.SetValue(CirclePointRingLoading.EllipseDiameterProperty, value);
            }
        }

        public Thickness EllipseOffset
        {
            get
            {
                return (Thickness)base.GetValue(CirclePointRingLoading.EllipseOffsetProperty);
            }
            set
            {
                base.SetValue(CirclePointRingLoading.EllipseOffsetProperty, value);
            }
        }

        public CirclePointRingLoading()
        {
            base.SizeChanged += delegate (object sender, SizeChangedEventArgs e)
            {
                this.BindableWidth = base.ActualWidth;
            };
        }

        static CirclePointRingLoading()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(CirclePointRingLoading), new FrameworkPropertyMetadata(typeof(CirclePointRingLoading)));
            UIElement.VisibilityProperty.OverrideMetadata(typeof(CirclePointRingLoading), new FrameworkPropertyMetadata(delegate (DependencyObject ringObject, DependencyPropertyChangedEventArgs e)
            {
                bool flag = e.NewValue != e.OldValue;
                if (flag)
                {
                    CirclePointRingLoading circlePointRingLoading = (CirclePointRingLoading)ringObject;
                    bool flag2 = (Visibility)e.NewValue > Visibility.Visible;
                    if (flag2)
                    {
                        circlePointRingLoading.SetCurrentValue(CirclePointRingLoading.IsActiveProperty, false);
                    }
                    else
                    {
                        circlePointRingLoading.IsActive = true;
                    }
                }
            }));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.UpdateLargeState();
            this.UpdateActiveState();
        }

        private string StateActive = "Active";

        private string StateInActive = "InActive";

        private string StateLarge = "Large";

        private string StateSmall = "Small";

        public static readonly DependencyProperty BindableWidthProperty = DependencyProperty.Register("BindableWidth", typeof(double), typeof(CirclePointRingLoading), new PropertyMetadata(0.0, new PropertyChangedCallback(CirclePointRingLoading.BindableWidthCallback)));

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(CirclePointRingLoading), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(CirclePointRingLoading.IsActiveChanged)));

        public static readonly DependencyProperty IsLargeProperty = DependencyProperty.Register("IsLarge", typeof(bool), typeof(CirclePointRingLoading), new PropertyMetadata(true, new PropertyChangedCallback(CirclePointRingLoading.IsLargeChangedCallback)));

        public static readonly DependencyProperty MaxSideLengthProperty = DependencyProperty.Register("MaxSideLength", typeof(double), typeof(CirclePointRingLoading), new PropertyMetadata(0.0));

        public static readonly DependencyProperty EllipseDiameterProperty = DependencyProperty.Register("EllipseDiameter", typeof(double), typeof(CirclePointRingLoading), new PropertyMetadata(0.0));

        public static readonly DependencyProperty EllipseOffsetProperty = DependencyProperty.Register("EllipseOffset", typeof(Thickness), typeof(CirclePointRingLoading), new PropertyMetadata(default(Thickness)));
    }
}
