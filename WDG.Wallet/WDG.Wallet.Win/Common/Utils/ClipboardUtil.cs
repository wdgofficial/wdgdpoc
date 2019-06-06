// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace WDG.Wallet.Win.Common.Utils
{
    public class ClipboardUtil
    {
        internal static void SetText(string text)
        {
            if (!Win32Util.OpenClipboard(IntPtr.Zero))
            {
                SetText(text);
                return;
            }
            Win32Util.EmptyClipboard();
            Win32Util.SetClipboardData(13, Marshal.StringToHGlobalUni(text));
            Win32Util.CloseClipboard();
        }
    }
}
