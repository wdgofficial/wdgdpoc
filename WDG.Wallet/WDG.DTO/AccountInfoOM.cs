// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.


namespace WDG.DTO
{
    public class AccountInfoOM
    {
        public string Address { get; set; }

        public string PublicKey { get; set; }

        public long Balance { get; set; }

        public bool IsDefault { get; set; }

        public bool WatchOnly { get; set; }

        public string Tag { get; set; }
    }
}
