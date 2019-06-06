// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Biz.Services;
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Common.interfaces;
using WDG.Wallet.Win.Models;
using WDG.Wallet.Win.Models.UiModels;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WDG.Wallet.Win.Biz.Invokes
{
    [Export(typeof(IInvoke))]
    public class ImportBackInvoke : VmBase, IInvoke
    {
        public string GetInvokeName()
        {
            return InvokeKeys.Restore;
        }

        public void Invoke<T>(T obj)
        {
            StartImport();
        }

        public FileInfo GetFile()
        {
            FileInfo fileResult = null;
            var settingResult = FiiiCoinService.Default.GetTxSettings();
            if (settingResult.IsFail)
            {
                ShowMessage(settingResult.GetErrorMsg());
                return fileResult;
            }
            
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = settingResult.Value.Encrypt? "BackUp Data|*.fcdatx": "BackUp Data| *.fcdat";
            fileDialog.RestoreDirectory = true;
            var result = fileDialog.ShowDialog(BootStrapService.Default.Shell.GetWindow());
            if (result.HasValue && result.Value)
            {
                var file = fileDialog.FileName;
                fileResult = new FileInfo(file);
            }
            return fileResult;
        }

        void StartImport()
        {
            FileInfo fileInfo = GetFile();
            if (fileInfo == null)
                return;
            if (fileInfo.Extension == ".fcdatx")
            {
                ImportWithPassword(fileInfo.FullName);
            }
            else
            {
                Import(fileInfo.FullName);
            }
        }

        void ImportWithPassword(string filePath)
        {
            SendMsgData<InputWalletPwdPageTopic> data = new SendMsgData<InputWalletPwdPageTopic>();
            data.Token = InputWalletPwdPageTopic.RequestPassword;
            data.SetCallBack(() =>
            {
                var password = "";
                if (data.CallBackParams != null)
                {
                    password = data.CallBackParams.ToString();
                    Import(filePath, password);
                }
                
            });
            SendMessenger(Pages.InputWalletPwdPage, SendMessageTopic.Refresh);
            SendMessenger(Pages.InputWalletPwdPage, data);
            UpdatePage(Pages.InputWalletPwdPage);
        }

        void Import(string file, string password = null)
        {
            var exportResult = WalletService.Default.ImportBackupWallet(file,password);
            if (!exportResult.IsFail)
            {
                //ShowMessage(LanguageService.Default.GetLanguageValue(MessageKeys.Import_sucesses));

                MessagePageData data = new MessagePageData();
                data.PageTitle = LanguageService.Default.GetLanguageValue(MessageKeys.Import_sucesses);
                MsgData iconData = new MsgData("path_msg_warming", Colors.Red);
                data.IconData = iconData;
                data.MsgItems.Add(new MsgData(LanguageService.Default.GetLanguageValue("Import_caption1"), Colors.Red));
                data.MsgItems.Add(new MsgData(LanguageService.Default.GetLanguageValue("Import_caption2"), "#333333"));
                data.MsgBtnShowType = MsgBtnType.Ok;
                data.CloseIsWord = true;
                data.CloseWord = "";
                data.SetOkCallBack(RestartApp);
                SendMessenger(Pages.MessagePage, data);
                UpdatePage(Pages.MessagePage);
            }
            else
            {
                ShowMessage(LanguageService.Default.GetLanguageValue(MessageKeys.Import_Fail));
                OnClosePopup();
            }
        }


        void RestartApp()
        {
            Messenger.Default.Send(new PopUpParams { IsOpen = false }, MessageTopic.ChangedPopupViewState);
            Messenger.Default.Send(Pages.ClosingPage, MessageTopic.UpdateMainView);
            Task task = new Task(() =>
            {
                SendMsgData<Window> data = new SendMsgData<Window>();
                data.Token = BootStrapService.Default.Shell.GetWindow();
                Messenger.Default.Send(data, Pages.ClosingPage);
            });
            task.Start();
        }
    }
}