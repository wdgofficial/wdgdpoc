// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.


namespace WDG.DTO
{
    public class NetworkInfoOM
    {
        public int Version { get; set; }

        public int MinimumSupportedVersion { get; set; }

        public int ProtocolVersion { get; set; }

        public bool IsRunning { get; set; }

        public int Connections { get; set; }
    }
}
