// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Business;
using WDG.DTO;
using WDG.Models;
using WDG.ServiceAgent;
using WDG.Utility.Api;
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WDG.Wallet.Win.Biz.Services
{
    public class UtxoService : ServiceBase<UtxoService>
    {
        public Result<TxOutSetOM> GetTxOutSetOM()
        {
            var response = UtxoApi.GetTxOutSetInfo().Result;
            return GetResult<TxOutSetOM>(response);
        }

        public Result<long> GetTradingMoney()
        {
            ApiResponse response =  UtxoApi.GetUnconfirmedBalance().Result;
            return base.GetResult<long>(response);
        }

        const int MINCONFIRMATIONS = 6;
        public Result<List<UnspentUtxo>> ListUnspent()
        {
            ApiResponse response = UtxoApi.ListUnspent(MINCONFIRMATIONS).Result;
            return base.GetResult<List<UnspentUtxo>>(response);
        }

        public Result<ListPageUnspent> ListPageUnspent(int minConfirmations, int currentPage, int pageSize, long maxConfirmations = 9999999, long minAmount = 1, long maxAmount = long.MaxValue, bool isDesc = false)
        {
            ApiResponse response = UtxoApi.ListPageUnspent(minConfirmations, currentPage, pageSize, maxConfirmations, minAmount, maxAmount, isDesc).Result;
            return base.GetResult<ListPageUnspent>(response);
        }

        public Result<TxOutModel> GetTxOut(string txid, int vout, bool unconfirmed = false)
        {
            var response = UtxoApi.GetTxOut(txid, vout, unconfirmed).Result;
            return GetResult<TxOutModel>(response);
        }

    }
}
