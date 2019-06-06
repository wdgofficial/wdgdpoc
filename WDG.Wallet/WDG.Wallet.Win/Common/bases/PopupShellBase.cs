// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WDG.Wallet.Win.Common
{
    public abstract class PopupShellBase : VmBase
    {
        public PopupShellBase()
        {
            Init();
        }

        public ICommand ClosePopupCommand { get; private set; }
        public ICommand OkCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        private void OnOkClick(List<DependencyObject> dependencyObjects)
        {
            var flag = true;
            if (dependencyObjects != null)
            {
                foreach (var item in dependencyObjects)
                {
                    if (Validation.GetHasError(item))
                    {
                        flag = false;
                        break;
                    }
                }
            }

            if (flag)
                OnOkClick();
        }

        public virtual void OnOkClick()
        {
            OnClosePopup();
        }


        public virtual void OnCancelClick()
        {
            OnClosePopup();
        }

        public virtual void Init()
        {
            ClosePopupCommand = new RelayCommand(OnClosePopup);
            OkCommand = new RelayCommand<List<DependencyObject>>(OnOkClick);
            CancelCommand = new RelayCommand(OnCancelClick);
        }
    }
}
