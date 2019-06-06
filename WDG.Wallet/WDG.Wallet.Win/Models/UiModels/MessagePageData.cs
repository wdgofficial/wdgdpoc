// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common;
using System;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace WDG.Wallet.Win.Models.UiModels
{
    public class MessagePageData : NotifyBase
    {
        private string _pageTitle;
        private ObservableCollection<MsgData> _msgItems;
        private MsgType _msgType;
        private MsgData _iconData;
        private MsgBtnType _msgBtnShowType;
        private bool _closewhendone;
        private bool _closeIsWord = false;
        private string _closeWord = "";



        public string PageTitle { get => _pageTitle; set { _pageTitle = value; RaisePropertyChanged("PageTitle"); } }
        public ObservableCollection<MsgData> MsgItems { get { _msgItems = _msgItems ?? new ObservableCollection<MsgData>(); return _msgItems; } set { _msgItems = value; RaisePropertyChanged("MsgItems"); } }
        public MsgType MsgType { get => _msgType; set { _msgType = value; RaisePropertyChanged("MsgType"); MsgTypeChanged(); } }
        public MsgBtnType MsgBtnShowType { get => _msgBtnShowType; set { _msgBtnShowType = value; RaisePropertyChanged("MsgBtnShowType"); } }

        public MsgData IconData { get => _iconData; set { _iconData = value; RaisePropertyChanged("IconData"); } }
        public bool Closewhendone { get => _closewhendone; set { _closewhendone = value; RaisePropertyChanged("Closewhendone"); } }

        public string CloseWord { get => _closeWord; set { _closeWord = value; RaisePropertyChanged("CloseWord"); } }
        public bool CloseIsWord { get => _closeIsWord; set { _closeIsWord = value; RaisePropertyChanged("CloseIsWord"); } }

        private Action OkCallBack;
        private Action CancelCallBack;

        public void SetOkCallBack(Action action)
        {
            OkCallBack = action;
        }

        public void SetCancelCallBack(Action action)
        {
            CancelCallBack = action;
        }

        public bool InvokeOkCallBack()
        {
            if (OkCallBack != null)
            {
                OkCallBack.Invoke();
                return true;
            }
            return false;
        }

        public bool InvokeCancelCallBack()
        {
            if (CancelCallBack != null)
            {
                CancelCallBack.Invoke();
                return true;
            }
            return false;
        }


        private void MsgTypeChanged()
        {
            switch (_msgType)
            {
                case MsgType.Warwarning:
                    IconData = new MsgData("path_msg_warming", "#F65952");
                    break;
                case MsgType.Capion:
                    IconData = new MsgData("path_msg_true", "#44CC4E");
                    break;
                default:
                    break;
            }
        }
    }

    public class MsgData : NotifyBase
    {
        public MsgData(string msg, Color color)
        {
            Msg = msg;
            Color = new SolidColorBrush(color);
        }

        public MsgData(string msg, SolidColorBrush colorBrush)
        {
            Msg = msg;
            Color = colorBrush;
        }

        public MsgData(string msg, string colorBrush)
        {
            Msg = msg;
            var color = (Color)ColorConverter.ConvertFromString(colorBrush);
            Color = new SolidColorBrush(color);
        }

        private string _msg;
        private SolidColorBrush _color;

        public string Msg { get => _msg; set { _msg = value; RaisePropertyChanged("Msg"); } }
        public SolidColorBrush Color { get => _color; set { _color = value; RaisePropertyChanged("Color"); } }
    }

    public enum MsgType
    {
        Warwarning,
        Capion,
    }

    public enum MsgBtnType
    {
        Ok = 0,
        Cancel = 1,
        OkAndCancel = Ok | Cancel,
    }
}
