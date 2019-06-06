// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Models;

namespace WDG.Wallet.Win.ViewModels.ShellPages
{
    public class OptionViewModel : PopupShellBase
    {
        protected override string GetPageName()
        {
            return Pages.OptionPage;
        }

        private int _selectedIndex = 0;
        private int _modelSelectedIndex = 0;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { _selectedIndex = value; RaisePropertyChanged("SelectedIndex"); }
        }

        void OnSelectionChanged()
        {
            Biz.Monitor.UpdateBlocksMonitor.Default.blockSyncInfo.BehindTime = "";
            var langaugeType = (LanguageType)SelectedIndex;
            LanguageService.Default.SetLanguage(langaugeType);
        }

        public override void OnOkClick()
        {
            var langaugeType = (LanguageType)SelectedIndex;
            var walletModel = (WalletModel)ModelSelectedIndex;
            if (LanguageService.Default.LanguageType != langaugeType)
            {
                OnSelectionChanged();
                AppSettingConfig.Default.SwitchLanguage();
            }
            if (walletModel != StaticViewModel.GlobalViewModel.WalletModel)
            {
                OnModelSelectionChanged();
            }
            AppSettingConfig.Default.UpdateConfig(langaugeType, walletModel);
            base.OnOkClick();
        }

        protected override void OnLoaded()
        {
            SelectedIndex = (int)LanguageService.Default.LanguageType;
            ModelSelectedIndex = (int)AppSettingConfig.Default.AppConfig.WalletModel;
            base.OnLoaded();
        }


        public int ModelSelectedIndex
        {
            get { return _modelSelectedIndex; }
            set { _modelSelectedIndex = value; RaisePropertyChanged("ModelSelectedIndex"); }
        }

        void OnModelSelectionChanged()
        {
            var walletModel = (WalletModel)ModelSelectedIndex;
            StaticViewModel.GlobalViewModel.WalletModel = walletModel;
        }
    }
}