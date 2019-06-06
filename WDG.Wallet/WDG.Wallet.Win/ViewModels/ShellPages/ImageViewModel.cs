// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Common.Utils;
using WDG.Wallet.Win.Models;
using WDG.Wallet.Win.Models.CommonParams;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WDG.Wallet.Win.ViewModels.ShellPages
{
    public class ImageViewModel : PopupShellBase
    {
        protected override string GetPageName()
        {
            return Pages.ImagePage;
        }

        public ICommand CopyAddressCommand { get; private set; }
        public ICommand SaveImageCommand { get; private set; }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            RegeistMessenger<SendMsgData<TitleWithParams<string>>>(UpdateImage);
            CopyAddressCommand = new RelayCommand(OnCopyAddress);
            SaveImageCommand = new RelayCommand(OnSaveImage);
        }

        SendMsgData<TitleWithParams<string>> _sendMsgData;

        private void UpdateImage(SendMsgData<TitleWithParams<string>> msg)
        {
            var address = msg.Token.Params;
            if (!string.IsNullOrEmpty(address) && !address.Equals(Address))
            {
                Address = address;
                Title = LanguageService.Default.GetLanguageValue(msg.Token.Title);
                QrCodePath = QRCodeUtil.Default.GenerateQRCodes(Address);
            }
            _sendMsgData = msg;
        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set { _title = value; RaisePropertyChanged("Title"); }
        }
        
        private string _qrCodePath;

        public string QrCodePath
        {
            get { return _qrCodePath; }
            set { _qrCodePath = value; RaisePropertyChanged("QrCodePath"); }
        }

        private string _address;

        public string Address
        {
            get { return _address; }
            set { _address = value; RaisePropertyChanged("Address"); }
        }

        void OnCopyAddress()
        {
            ClipboardUtil.SetText(Address);
        }

        void OnSaveImage()
        {
            if (string.IsNullOrEmpty(QrCodePath))
            {
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG（*.png）|*.png";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;
            var result = saveFileDialog.ShowDialog(BootStrapService.Default.Shell.GetWindow());
            if (result.HasValue && result.Value)
            {
                var file = saveFileDialog.FileName;
                FileInfo imageFile = new FileInfo(QrCodePath);

                imageFile.CopyTo(file);
            }
        }

        public override void OnClosePopup()
        {
            base.OnClosePopup();
            _sendMsgData.CallBack();
        }
    }
}
