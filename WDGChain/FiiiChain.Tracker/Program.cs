// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.Business;
using FiiiChain.Framework;
using System;
using System.Threading.Tasks;

namespace FiiiChain.Tracker
{
    class Program
    {
        static void Main(string[] args)
        {
            bool testnet = false;

            if (args.Length == 1 && args[0].ToLower() == "-testnet")
            {
                testnet = true;
                LogHelper.Info("FiiiChain Tracker Server Testnet is Started.");
            }
            else
            {
                LogHelper.Info("FiiiChain Tracker Server is Started.");
            }

            try
            {
                GlobalParameters.IsTestnet = testnet;
                var p2pComponent = new P2PComponent();
                p2pComponent.P2PStart(Guid.NewGuid(), "", true);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message, ex);
            }
        }
    }
}
