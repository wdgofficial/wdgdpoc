// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.
using EdjCase.JsonRpc.Router.Abstractions;
using FiiiChain.Business;
using FiiiChain.Framework;
using FiiiChain.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiiiChain.Wallet.API
{
    public class MiningPoolController : BaseRpcController
    {
        public IRpcMethodResult AddMiningPool(string name, string publicKey,string signature)
        {
            try
            {
                var miningMsg = new MiningMsg() { Name = name, PublicKey = publicKey , Signature = signature};
                var miningPoolComponent = new MiningPoolComponent();
                var result = miningPoolComponent.AddMiningToPool(miningMsg);
                return Ok(result);
            }
            catch (CommonException ce)
            {
                return Error(ce.ErrorCode, ce.Message, ce);
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.UNKNOWN_ERROR, ex.Message, ex);
            }
        }
    }
}
