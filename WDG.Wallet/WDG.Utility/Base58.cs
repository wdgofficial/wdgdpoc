// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

using System;

namespace WDG.Utility
{
    public static class Base58
    {
        /*
        /// <summary>
        /// Base58Check Encoding / Decoding (Bitcoin-style)
        /// </summary>
        /// <remarks>
        /// See here for more details: https://en.bitcoin.it/wiki/Base58Check_encoding
        /// </remarks>

        private const int CHECK_SUM_SIZE = 4;
        private const string DIGITS = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        /// <summary>
        /// Encodes data with a 4-byte checksum
        /// </summary>
        /// <param name="data">Data to be encoded</param>
        /// <returns></returns>
        public static string Encode(byte[] data)
        {
            return EncodePlain(_AddCheckSum(data));
        }

        /// <summary>
        /// Encodes data in plain Base58, without any checksum.
        /// </summary>
        /// <param name="data">The data to be encoded</param>
        /// <returns></returns>
        public static string EncodePlain(byte[] data)
        {
            // Decode byte[] to BigInteger
            var intData = data.Aggregate<byte, BigInteger>(0, (current, t) => current * 256 + t);

            // Encode BigInteger to Base58 string
            var result = string.Empty;
            while (intData > 0)
            {
                var remainder = (int)(intData % 58);
                intData /= 58;
                result = DIGITS[remainder] + result;
            }

            // Append `1` for each leading 0 byte
            for (var i = 0; i < data.Length && data[i] == 0; i++)
            {
                result = '1' + result;
            }

            return result;
        }

        /// <summary>
        /// Decodes data in Base58Check format (with 4 byte checksum)
        /// </summary>
        /// <param name="data">Data to be decoded</param>
        /// <returns>Returns decoded data if valid; throws FormatException if invalid</returns>
        public static byte[] Decode(string data)
        {
            var dataWithCheckSum = DecodePlain(data);
            var dataWithoutCheckSum = _VerifyAndRemoveCheckSum(dataWithCheckSum);

            if (dataWithoutCheckSum == null)
            {
                throw new FormatException("Base58 checksum is invalid");
            }

            return dataWithoutCheckSum;
        }

        /// <summary>
        /// Decodes data in plain Base58, without any checksum.
        /// </summary>
        /// <param name="data">Data to be decoded</param>
        /// <returns>Returns decoded data if valid; throws FormatException if invalid</returns>
        public static byte[] DecodePlain(string data)
        {
            // Decode Base58 string to BigInteger 
            BigInteger intData = 0;
            for (var i = 0; i < data.Length; i++)
            {
                var digit = DIGITS.IndexOf(data[i]); //Slow

                if (digit < 0)
                {
                    throw new FormatException(string.Format("Invalid Base58 character `{0}` at position {1}", data[i], i));
                }

                intData = intData * 58 + digit;
            }

            // Encode BigInteger to byte[]
            // Leading zero bytes get encoded as leading `1` characters
            var leadingZeroCount = data.TakeWhile(c => c == '1').Count();
            var leadingZeros = Enumerable.Repeat((byte)0, leadingZeroCount);
            var bytesWithoutLeadingZeros =
              intData.ToByteArray()
              .Reverse()// to big endian
              .SkipWhile(b => b == 0);//strip sign byte
            var result = leadingZeros.Concat(bytesWithoutLeadingZeros).ToArray();

            return result;
        }

        private static byte[] _AddCheckSum(byte[] data)
        {
            var checkSum = _GetCheckSum(data);
            var dataWithCheckSum = ArrayHelpers.ConcatArrays(data, checkSum);

            return dataWithCheckSum;
        }

        //Returns null if the checksum is invalid
        private static byte[] _VerifyAndRemoveCheckSum(byte[] data)
        {
            var result = ArrayHelpers.SubArray(data, 0, data.Length - CHECK_SUM_SIZE);
            var givenCheckSum = ArrayHelpers.SubArray(data, data.Length - CHECK_SUM_SIZE);
            var correctCheckSum = _GetCheckSum(result);

            return givenCheckSum.SequenceEqual(correctCheckSum) ? result : null;
        }

        private static byte[] _GetCheckSum(byte[] data)
        {
            SHA256 sha256 = new SHA256Managed();
            var hash1 = sha256.ComputeHash(data);
            var hash2 = sha256.ComputeHash(hash1);

            var result = new byte[CHECK_SUM_SIZE];
            Buffer.BlockCopy(hash2, 0, result, 0, result.Length);

            return result;
        }
        */
        private static string Base58characters = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
        public static byte[] Decode(string text)
        {
            int i = 0;
            while (i < text.Length)
            {
                if (text[i] == 0 || !Char.IsWhiteSpace(text[i]))
                {
                    break;
                }
                i++;
            }
            int zeros = 0;
            while (text[i] == '1')
            {
                zeros++;
                i++;
            }
            byte[] b256 = new byte[(text.Length - i) * 733 / 1000 + 1];
            while (i < text.Length && !Char.IsWhiteSpace(text[i]))
            {
                int ch = Base58characters.IndexOf(text[i]);
                if (ch == -1) //null
                {
                    return null;
                }
                int carry = Base58characters.IndexOf(text[i]);
                for (int k = b256.Length - 1; k >= 0; k--)
                {
                    carry += 58 * b256[k];
                    b256[k] = (byte)(carry % 256);
                    carry /= 256;
                }
                i++;
            }
            while (i < text.Length && Char.IsWhiteSpace(text[i]))
            {
                i++;
            }
            if (i != text.Length)
            {
                return null;
            }
            int j = 0;
            while (j < b256.Length && b256[j] == 0)
            {
                j++;
            }
            byte[] destination = new byte[zeros + (b256.Length - j)];
            for (int kk = 0; kk < destination.Length; kk++)
            {
                if (kk < zeros)
                {
                    destination[kk] = 0x00;
                }
                else
                {
                    destination[kk] = b256[j++];
                }
            }
            return destination;
        }
    }
}
