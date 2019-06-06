// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDG.Wallet.Win.Common
{
    public static class Formats
    {
        public static string RefreshFormat = null;

        public static void Init()
        {
            LanguageService.Default.OnLanguageChanged += OnLanguageChanged;
        }

        private static void OnLanguageChanged(LanguageType languageType)
        {
            RefreshFormat = LanguageService.Default.GetLanguageValue("RefreshFormat");
        }
    }
}
