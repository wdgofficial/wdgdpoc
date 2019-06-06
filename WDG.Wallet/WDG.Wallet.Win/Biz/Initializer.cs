// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Models;
using WDG.Utility;
using WDG.Wallet.Win.Biz.Monitor;
using WDG.Wallet.Win.Biz.Services;
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Common.Keys;
using WDG.Wallet.Win.Models;
using WDG.Wallet.Win.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using System.Linq;
using System.Threading;
using System.Windows;

namespace WDG.Wallet.Win.Biz
{
    public delegate void InitializedInvokeHandle(InitMsgEvent e);

    public class Initializer : InstanceBase<Initializer>
    {
        public event InitializedInvokeHandle InitializedInvoke;

        public AccountInfo DefaultAccount { get; private set; }

        public void Start()
        {
            StaticViewModel.GlobalViewModel.InitStep = InitStep.ServiceStarting;
            NodeInitializer.Default.StartNode();

            //升级判断是否有新版本
            AppUdateMonitor.Default.MonitorCallBack += AppUpdateCallBack;
            AppUdateMonitor.Default.Start(1000 * 60 * 60 * 3);//3Hour

            UpdateBlocksMonitor.Default.MonitorCallBack += UpdateBlocksCallBack;
        }

        void AppUpdateCallBack(bool? isNeedUpdate)
        {
            if (isNeedUpdate.HasValue && !isNeedUpdate.Value)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var msgResult = MessageBox.Show("Application has been updated. \r\n Download and install it?",
                        "Application Detect", MessageBoxButton.OK);
                    if (msgResult == MessageBoxResult.OK)
                    {
                        System.Diagnostics.Process.Start(AppUdateMonitor.AppUrl);
                    }
                    Messenger.Default.Send(BootStrapService.Default.Shell.GetWindow(), Pages.ClosingPage);
                });
            }
            else
            {
                Logger.Singleton.Info("AmountMonitor Start");
                AmountMonitor.Default.Start(3000);
                Logger.Singleton.Info("UpdateBlocksMonitor Start");
                UpdateBlocksMonitor.Default.Start(3000);
            }
        }


        int blockSyncCount = 0;
        void UpdateBlocksCallBack(BlockSyncInfo blockSyncInfo)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (blockSyncInfo.IsSyncStart())
                {
                    UpdateBlocksMonitor.Default.MonitorCallBack -= UpdateBlocksCallBack;
                    ContinueStart(blockSyncInfo);
                    return;
                }
                if (blockSyncCount >= 20 && InitializedInvoke != null && blockSyncInfo.ConnectCount < 2)
                {
                    InitializedInvoke.Invoke(new InitMsgEvent(false, LanguageService.Default.GetLanguageValue("Error_NoNet")));
                }
                blockSyncCount++;
            });
        }


        void ContinueStart(BlockSyncInfo blockSyncInfo)
        {
            InitWalletSataus();

            var accountResult = AccountsService.Default.GetDefaultAccount();
            if (!accountResult.IsFail)
            {
                DefaultAccount = accountResult.Value;
            }

            Registers();

            if (InitializedInvoke != null)
                InitializedInvoke.Invoke(new InitMsgEvent(true));

            if (blockSyncInfo.BlockLeft > 0 && blockSyncInfo.Progress < 100)
            {
                StaticViewModel.GlobalViewModel.IsFirstShowProgress = false;
                Messenger.Default.Send(Pages.SynchroDataPage, MessageTopic.UpdatePopupView);
                PopUpParams popUpParams = new PopUpParams { IsOpen = true };
                Messenger.Default.Send(popUpParams, MessageTopic.ChangedPopupViewState);
            }
            
            TxSettingMonitor.Default.Start(1000);
            TradeRecodesMonitor.Default.Start(10000);
            PayAddressBookMonitor.Default.Start(3000);
            ReceiveAddressBookMonitor.Default.Start(3000);
        }


        void Registers()
        {
            Messenger.Default.Register<CommonTopic>(this, OnGetRespose);
        }

        void OnGetRespose(CommonTopic commonTopic)
        {
            switch (commonTopic)
            {
                case CommonTopic.UpdateWalletStatus:
                    InitWalletSataus();
                    break;
                case CommonTopic.ExportBackUp:
                default:
                    break;
            }
        }

        private bool InitWalletSataus()
        {
            var settingResult = FiiiCoinService.Default.GetTxSettings();
            if (settingResult.IsFail) return false;
            var isEncrypt = settingResult.Value.Encrypt;
            if (isEncrypt)
            {
                var menus = AppSettingConfig.Default.GetAllMenus(AppSettingConfig.Default.AppConfig.MenuItems);
                var encryptWalletItem = menus.FirstOrDefault(x => x.HeaderKey == ConfigKeys.Config_Setting_Encrypt);

                if (encryptWalletItem != null)
                    encryptWalletItem.IsEnabled = false;

                var changedPwdItem = menus.FirstOrDefault(x => x.HeaderKey == ConfigKeys.Config_Setting_ChangePwd);

                if (changedPwdItem != null)
                    changedPwdItem.IsEnabled = true;
            }
            else
            {
                var menus = AppSettingConfig.Default.GetAllMenus(AppSettingConfig.Default.AppConfig.MenuItems);
                var changedPwdItem = menus.FirstOrDefault(x => x.HeaderKey == ConfigKeys.Config_Setting_ChangePwd);
                if (changedPwdItem != null)
                    changedPwdItem.IsEnabled = false;

                var encryptWalletItem = menus.FirstOrDefault(x => x.HeaderKey == ConfigKeys.Config_Setting_Encrypt);

                if (encryptWalletItem != null)
                    encryptWalletItem.IsEnabled = true;
            }
            return true;
        }

        internal void ShowMessage(string msg)
        {
            Messenger.Default.Send(msg, MessageTopic.ShowMessageAutoClose);
        }

    }

    public class InitMsgEvent : NotifyBase
    {
        public InitMsgEvent(bool issucesses ,string msg = "")
        {
            IsSucesses = issucesses;
            Message = msg;
        }

        private bool _isSucesses;
        public bool IsSucesses { get { return _isSucesses; } set { _isSucesses = value; RaisePropertyChanged("IsSucesses"); } }

        private string _message;
        public string Message { get { return _message; } set { _message = value; RaisePropertyChanged("Message"); } }
    }
}
