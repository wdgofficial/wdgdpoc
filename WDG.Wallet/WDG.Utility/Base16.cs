// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDG.Utility
{
    public static class Base16
    {
        public static char[] BASE16_ENC_TAB = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
        public static byte[] BASE16_DEC_TAB = {
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };
        public static String Encode(byte[] data)
        {
            StringBuilder stringBuilder = new StringBuilder();
            int i = 0, total = data.Length;
            while (i < total)
            {
                stringBuilder.Append(BASE16_ENC_TAB[(data[i] & 0xF0) >> 4]);
                stringBuilder.Append(BASE16_ENC_TAB[data[i] & 0x0F]);
                i++;
            }
            return stringBuilder.ToString();
        }

        public static byte[] Decode(string encryptedText)
        {
            byte[] data = new byte[encryptedText.Length / 2];
            int i = 0, total = (encryptedText.Length / 2) * 2, idx = 0;
            while (i<total) {
                data[idx++] = (byte) ((BASE16_DEC_TAB[encryptedText[(i++)]] << 4) | BASE16_DEC_TAB[encryptedText[(i++)]]);
            }
            return data;
        }

        /*
        public static string Encode(byte[] arr)
        {
            string[] autoCode = new string[] { "a", "2", "B", "g", "E", "5", "f", "6", "C", "8", "o", "9", "Z", "p", "k", "M" };
            StringBuilder strEn = new StringBuilder();
            for (byte i = 0; i < arr.Length; i++)
            {
                byte data = arr[i];
                int v1 = data >> 4;
                strEn.Append(autoCode[v1]);
                int v2 = ((data & 0x0f) << 4) >> 4;
                strEn.Append(autoCode[v2]);
            }
            return strEn.ToString();
        }

        public static byte[] Decode(string str)
        {
            string[] autoCode = new string[] { "a", "2", "B", "g", "E", "5", "f", "6", "C", "8", "o", "9", "Z", "p", "k", "M" };
            int k = 0;
            string dnStr = string.Empty;
            int strLength = str.Length;

            byte[] data = new byte[strLength / 2];
            for (int i = 0, j = 0; i < data.Length; i++, j++)
            {
                byte s = 0;
                int index1 = autoCode.ToList().IndexOf(str[j].ToString());
                j += 1;
                int index2 = autoCode.ToList().IndexOf(str[j].ToString());
                s = (byte)(s ^ index1);
                s = (byte)(s << 4);
                s = (byte)(s ^ index2);
                data[k] = s;
                k++;
            }
            return data;
        }
        */
    }
}
