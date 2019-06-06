// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System.Windows.Controls;

namespace WDG.Wallet.Win.Common
{
    public interface IPage
    {
        string GetPageName();

        Page GetCurrentPage();
    }
}
