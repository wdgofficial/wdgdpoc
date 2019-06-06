// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

namespace WDG.Wallet.Win.ViewModels
{
    public static class StaticViewModel
    {
        private static GlobalViewModel _globalViewModel = null;

        public static GlobalViewModel GlobalViewModel
        {
            get
            {
                if (_globalViewModel == null)
                {
                    _globalViewModel = new GlobalViewModel();
                }
                return _globalViewModel;
            }
        }
    }
}