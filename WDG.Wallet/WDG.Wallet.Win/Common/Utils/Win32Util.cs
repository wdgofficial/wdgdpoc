// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Runtime.InteropServices;

namespace WDG.Wallet.Win.Common.Utils
{
    public class Win32Util
    {
        [DllImport("User32")]
        internal static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("User32")]
        internal static extern bool CloseClipboard();

        [DllImport("User32")]
        internal static extern bool EmptyClipboard();

        [DllImport("User32")]
        internal static extern bool IsClipboardFormatAvailable(int format);

        [DllImport("User32")]
        internal static extern IntPtr GetClipboardData(int uFormat);

        [DllImport("User32", CharSet = CharSet.Unicode)]
        internal static extern IntPtr SetClipboardData(int uFormat, IntPtr hMem);


        [DllImport("kernel32.dll", EntryPoint = "GetUserDefaultUILanguage")]
        internal static extern int GetUserDefaultUILanguage();

        public static LanguageType GetSystemLanguage()
        {
            var lan = GetUserDefaultUILanguage();
            if (lan == 0x804)
            {
                return LanguageType.zh_cn;
            }
            else
            {
                return LanguageType.en_us;
            }
        }
    }
}
