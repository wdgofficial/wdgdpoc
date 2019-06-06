using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WDG.Wallet.Win.Views.Controls
{
    /// <summary>
    /// PagerControl.xaml 的交互逻辑
    /// </summary>
    public partial class PagerControl : UserControl
    {
        public PagerControl()
        {
            InitializeComponent();
        }
        
        public int PageCount
        {
            get { return (int)GetValue(PageCountProperty); }
            set { SetValue(PageCountProperty, value); }
        }
        

        public static readonly DependencyProperty PageCountProperty =
            DependencyProperty.Register("PageCount", typeof(int), typeof(PagerControl), new PropertyMetadata(1));

        public int CurrentPage
        {
            get { return (int)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }
        
        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register("CurrentPage", typeof(int), typeof(PagerControl), new PropertyMetadata(1));

        private void FirstPageClick(object sender, RoutedEventArgs e)
        {
            CurrentPage = 1;
        }

        private void PrevPageClick(object sender, RoutedEventArgs e)
        {
            if (CurrentPage == 1)
                return;
            else
                CurrentPage--;
        }

        private void NextPageClick(object sender, RoutedEventArgs e)
        {
            if (CurrentPage == PageCount)
                return;
            CurrentPage++;
        }

        private void LastPageClick(object sender, RoutedEventArgs e)
        {
            CurrentPage = PageCount;
        }
    }
}
