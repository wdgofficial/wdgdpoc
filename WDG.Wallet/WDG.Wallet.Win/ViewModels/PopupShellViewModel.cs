// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Models;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Controls;

namespace WDG.Wallet.Win.ViewModels
{
    public class PopupShellViewModel : VmBase
    {
        public PopupShellViewModel()
        {
            Messenger.Default.Register<string>(this, MessageTopic.UpdatePopupView, UpdateView);
            Messenger.Default.Register<string>(this, MessageTopic.ClosePopUpWindow, OnClosePopupView);
        }

        private Page _popupShellView;

        public Page PopupShellView
        {
            get { return _popupShellView; }
            set
            {
                _popupShellView = value;
                RaisePropertyChanged("PopupShellView");
            }
        }


        public void UpdateView(string newPageName)
        {
            var page = BootStrapService.Default.GetPage(newPageName);
            if (page != null && page != _popupShellView)
                PopupShellView = page;
        }

        void OnClosePopupView(string pageName)
        {
            if (PopupShellView != null && PopupShellView.ToString().Contains(pageName))
            {
                PopUpParams popUpParams = new PopUpParams { IsOpen = false };
                Messenger.Default.Send(popUpParams, MessageTopic.ChangedPopupViewState);
            }
        }

        protected override string GetPageName()
        {
            return Pages.PopupShell;
        }
    }
}
