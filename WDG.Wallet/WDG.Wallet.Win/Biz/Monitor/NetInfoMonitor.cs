// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Biz.Services;

namespace WDG.Wallet.Win.Biz.Monitor
{
    public class NetInfoMonitor : ServiceMonitorBase<long?>
    {
        private static NetInfoMonitor _default;

        public static NetInfoMonitor Default
        {
            get
            {
                if (_default == null)
                    _default = new NetInfoMonitor();
                return _default;
            }
        }

        protected override long? ExecTaskAndGetResult()
        {
            var result = NetWorkService.Default.GetConnectionCount();
            if (result.IsFail)
                return null;
            return result.Value;
        }
    }
}
