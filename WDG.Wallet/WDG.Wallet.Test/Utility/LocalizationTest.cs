// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

using WDG.Business;
using WDG.Utility.Api;
using WDG.Utility.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WDG.Wallet.Test.Utility
{
    [TestClass]
    public class LocalizationTest
    {
        [TestMethod]
        public async Task GetCurrentUIString()
        {
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            ApiResponse response = await UtxoApi.GetTxOutSetInfo();
            if (response.HasError)
            {
                //Get the global message from resource dictionary by response error code
                string value = FiiiCoinLocalizationConfigurer.GetResource(response.Error.Code.ToString());
                MessageBox.Show(value);
            }
        }
    }
}
