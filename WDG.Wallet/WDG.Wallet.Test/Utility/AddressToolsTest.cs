// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

using WDG.Utility;
using WDG.Utility.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace WDG.Wallet.Test.Utility
{
    [TestClass]
    public class AddressToolsTest
    {
        [TestMethod]
        public void AddressVerfy()
        {
            bool result = AddressTools.AddressVerfy("main", "fiiimB6yxbLHKU2YuLhSDSjiB5mdWXt51duHBi");
            //bool rbsult = AddressTools.AddressVerfy("test", "fiiit2sTHKSFrYJHo1ddUNNHCKqJnAhjATgr6c");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Base58Decode()
        {
            byte[] rbsult = Base58.Decode("fiiimB6yxbLHKU2YuLhSDSjiB5mdWXt51duHBi");
        }

        public bool AddressVerify()
        {
            var bytes = Base58.Decode("fiiimB6yxbLHKU2YuLhSDSjiB5mdWXt51duHBi");
            //var prefix = bytes[prefixLen];
            var checksum = new byte[4];
            var data = new byte[bytes.Length - 4];
            Array.Copy(bytes, 0, data, 0, bytes.Length - 4);
            Array.Copy(bytes, bytes.Length - checksum.Length, checksum, 0, checksum.Length);

            var newChecksum = HashHelper.DoubleHash(data).Take(4);
            return BitConverter.ToInt32(checksum, 0) == BitConverter.ToInt32(newChecksum.ToArray(), 0);
        }
    }
}
