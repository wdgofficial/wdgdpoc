// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Utility;
using WDG.Wallet.Win.ViewModels;
using HtmlAgilityPack;
using System;
using System.Net;
using System.Text;

namespace WDG.Wallet.Win.Biz.Monitor
{
    public class AppUdateMonitor : ServiceMonitorBase<bool?>
    {
        private static AppUdateMonitor _default;

        public static AppUdateMonitor Default
        {
            get
            {
                if (_default == null)
                {
                    ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                    _default = new AppUdateMonitor();
                }
                return _default;
            }
        }

        protected override bool? ExecTaskAndGetResult()
        {
            return CheckVersion();
        }

        public const string AppUrl = "https://github.com/wdgofficial/wdgdpoc/releases";

        public string GetRemoteVersion()
        {
            try
            {
                WebClient webClient = new WebClient();
                webClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
                byte[] pageData = webClient.DownloadData(AppUrl);
                string pageHtml = Encoding.UTF8.GetString(pageData);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(pageHtml);
                Logger.Singleton.Debug(pageHtml);
                HtmlNodeCollection hrefList = doc.DocumentNode.SelectNodes("//div[@class='f1 flex-auto min-width-0 text-normal']/a");
                if (hrefList != null && hrefList.Count > 0)
                {
                    return hrefList[0].InnerHtml;
                }
            }
            catch (Exception ex)
            {
                Logger.Singleton.Warn(ex.ToString());
            }
            return null;
        }

        bool? CheckVersion()
        {
            //return true;
            var remoteVersion = GetRemoteVersion();
            Logger.Singleton.Debug($"GetVersion Result = {(remoteVersion == null ? "NULL" : remoteVersion)}");
            if (remoteVersion == null)
                return true;
            if (StaticViewModel.GlobalViewModel.VERSION != remoteVersion)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}