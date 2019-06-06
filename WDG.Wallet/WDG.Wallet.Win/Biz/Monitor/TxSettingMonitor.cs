// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Models;
using WDG.Wallet.Win.Biz.Services;

namespace WDG.Wallet.Win.Biz.Monitor
{
    public class TxSettingMonitor : ServiceMonitorBase<TransactionFeeSetting>
    {
        private static TxSettingMonitor _default;

        public static TxSettingMonitor Default
        {
            get
            {
                if (_default == null)
                    _default = new TxSettingMonitor();
                return _default;
            }
        }

        protected override TransactionFeeSetting ExecTaskAndGetResult()
        {
            var feeResult = FiiiCoinService.Default.GetTxSettings();

            if (!feeResult.IsFail)
            {
                return feeResult.Value;
            }
            else
                return null;
        }
    }
}
