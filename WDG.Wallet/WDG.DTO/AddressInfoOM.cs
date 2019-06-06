// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.


namespace WDG.DTO
{
    public class AddressInfoOM
    {
        public bool IsValid { get; set; }

        public string Address { get; set; }

        public string ScriptPubKey { get; set; }

        public bool IsMine { get; set; }

        public bool IsWatchOnly { get; set; }

        public bool IsScript { get; set; }

        public string Script { get; set; }

        public string Hex { get; set; }

        public string[] Addresses { get; set; }

        public long Sigrequired { get; set; }

        public string PubKey { get; set; }

        public bool IsCompressed { get; set; }

        public string Account { get; set; }

        public string Hdkeypath { get; set; }

        public string Hdmasterkeyid { get; set; }
    }
}
