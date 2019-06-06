// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common;
using GalaSoft.MvvmLight.Command;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace WDG.Wallet.Win.ViewModels.ShellPages
{
    public class AboutViewModel : PopupShellBase
    {
        protected override string GetPageName()
        {
            return Pages.AboutPage;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            OnLoadedCommand = new RelayCommand<WebBrowser>(OnLoaded);
        }

        private Uri _aboutUri;

        public Uri AboutUri
        {
            get { return _aboutUri; }
            set { _aboutUri = value; RaisePropertyChanged("AboutUri"); }
        }

        void OnLanguageChanged(LanguageType languageType)
        {
            if (languageType == LanguageType.zh_cn)
            {
                AboutUri = new Uri("pack://siteoforigin:,,,/lang/res/about_zhcn.html", UriKind.Absolute);
            }
            else
            {
                AboutUri = new Uri("pack://siteoforigin:,,,/lang/res/about_enus.html", UriKind.Absolute);
            }
            _webBrowser.Source = AboutUri;
        }

        public ICommand OnLoadedCommand { get; private set; }

        WebBrowser _webBrowser = null;
        void OnLoaded(WebBrowser webBrowser)
        {
            _webBrowser = webBrowser;
            OnLanguageChanged(LanguageService.Default.LanguageType);
            LanguageService.Default.OnLanguageChanged += OnLanguageChanged;
        }
    }
}
