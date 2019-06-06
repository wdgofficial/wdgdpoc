// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

using WDG.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WDG.Wallet.Test.Utility
{
    [TestClass]
    public class JsonTest
    {
        [TestMethod]
        public void JsonDeserializeTest()
        {
            string str = "[{\"address\":\"fiiitNenK98A1jSXXqK3sZd65TR2Kvo8fnrv3r\",\"tag\":\"john\",\"amount\":100000,\"comment\":\"no comment\"},{\"address\":\"fiiitFmH9Cqk5B9gTH3LqZzBtqb8pxgHJ7sVqY\",\"tag\":\"john\",\"amount\":100000,\"comment\":\"no comment\"}]";
            SendManyModel[] model = Newtonsoft.Json.JsonConvert.DeserializeObject<SendManyModel[]>(str);
            Assert.IsNotNull(model);
        }
    }
}
