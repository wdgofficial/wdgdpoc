// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.


namespace WDG.DTO
{
    public class ChainTipsOM
    {
        public long Height { get; set; }
        public string Hash { get; set; }
        public long BranchLen { get; set; }
        public string Status { get; set; }
    }
}
