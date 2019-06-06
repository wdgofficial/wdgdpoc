// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using EdjCase.JsonRpc.Router.Abstractions;
using FiiiChain.Business;
using FiiiChain.Framework;
using System;

namespace FiiiChain.Wallet.API
{
    public class PaymentRequestController : BaseRpcController
    {
        public IRpcMethodResult CreateNewPaymentRequest(string address, string tag, long amount, string comment)
        {
            try
            {
                var result = new PaymentRequestComponent().Add(address, tag, comment, amount);

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

        public IRpcMethodResult DeletePaymentRequestsByIds(long[] ids)
        {
            try
            {
                new PaymentRequestComponent().DeleteByIds(ids);
                return Ok();
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

        public IRpcMethodResult GetAllPaymentRequests()
        {
            try
            {
                var result = new PaymentRequestComponent().GetAll();
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
