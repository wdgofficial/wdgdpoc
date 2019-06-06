using WDG.Wallet.Win.Common;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace WDG.Wallet.Win.Views.ShellPages
{
    /// <summary>
    /// SendSettingPage.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(IPage))]
    public partial class SendSettingPage : Page, IPage
    {
        public SendSettingPage()
        {
            InitializeComponent();
        }

        public Page GetCurrentPage()
        {
            return this;
        }

        public string GetPageName()
        {
            return Pages.SendSettingPage;
        }
    }
}
