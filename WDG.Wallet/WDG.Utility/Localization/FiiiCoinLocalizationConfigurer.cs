// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

using WDG.Utility.Localization.SourceFiles;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace WDG.Utility.Localization
{
    public static class FiiiCoinLocalizationConfigurer
    {
        
        public static string GetResource(string key)
        {
            CultureInfo culture = Thread.CurrentThread.CurrentUICulture;
            ResourceManager resource = new ResourceManager(FiiiCoinLocalization.ResourceManager.BaseName, Assembly.GetExecutingAssembly());
            return resource.GetString(key);
        }
    }
}
