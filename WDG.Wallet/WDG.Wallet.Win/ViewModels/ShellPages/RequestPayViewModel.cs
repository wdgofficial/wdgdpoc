// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Models;
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Common.Utils;
using WDG.Wallet.Win.Models;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System.IO;
using System.Windows.Input;

namespace WDG.Wallet.Win.ViewModels.ShellPages
{
    public class RequestPayViewModel : PopupShellBase
    {
        protected override string GetPageName()
        {
            return Pages.RequestPayPage;
        }

        public override void Init()
        {
            base.Init();
            RegeistMessenger<PayRequest>(OnGetRequest);
            CopyAccountCommand = new RelayCommand(OnCopyAccount);
            CopyURLCommand = new RelayCommand(OnCopyURL);
            SaveImageCommand = new RelayCommand(OnSaveImage);
        }

        private string _qrCodePath;

        public string QrCodePath
        {
            get { return _qrCodePath; }
            set { _qrCodePath = value; RaisePropertyChanged("QrCodePath"); }
        }

        private string _qrCodeStr;

        public string QrCodeStr
        {
            get { return _qrCodeStr; }
            set { _qrCodeStr = value; RaisePropertyChanged("QrCodeStr"); }
        }


        private PayRequest _payRequest;

        public PayRequest PayRequest
        {
            get { return _payRequest; }
            set { _payRequest = value; RaisePropertyChanged("PayRequest"); }
        }


        void OnGetRequest(PayRequest request)
        {
            var qrCodeStr =string.Format("wdgcoin:{0}?amount={1}&label={2}&message={3}",
                                        request.AccountId,
                                        request.Amount,
                                        request.Tag,
                                        request.Comment);
            var path = QRCodeUtil.Default.GenerateQRCodes(request.AccountId);
            QrCodeStr = qrCodeStr;
            PayRequest = request;
            QrCodePath = path;
        }

        public ICommand CopyAccountCommand { get; private set; }
        public ICommand CopyURLCommand { get; private set; }
        public ICommand SaveImageCommand { get; private set; }

        void OnCopyAccount()
        {
            ClipboardUtil.SetText(PayRequest.AccountId);
        }

        void OnCopyURL()
        {
            ClipboardUtil.SetText(QrCodeStr);
        }

        void OnSaveImage()
        {
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
            SendMessenger(Pages.ReceiveAddressPage, SendMessageTopic.Refresh);
        }
    }
}
