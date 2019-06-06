// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.Entities;
using FiiiChain.Entities.ExtensionModels;
using FiiiChain.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.Business.ParamsModel
{
    public class VerifyTransactionModel
    {
        public TransactionMsg transaction;
        public BlockMsg block;
        public List<Output> outputs;
        public List<BlockConstraint> constraints;
        public long localHeight;
    }
}
