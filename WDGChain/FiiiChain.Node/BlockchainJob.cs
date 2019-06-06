// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FiiiChain.Business;
using FiiiChain.Data;
using FiiiChain.DataAgent;
using FiiiChain.Entities;
using FiiiChain.Entities.CacheModel;
using FiiiChain.Framework;

namespace FiiiChain.Node
{
    public class BlockchainJob : BaseJob
    {
        public static BlockchainJob Current = null;
        public BlockJob BlockService;
        public P2PJob P2PJob;
        public RpcJob RpcService;
        public bool IsRunning = true;

        NodeConfig config = null;

        public BlockchainJob()
        {
            RpcService = new RpcJob();
            P2PJob = new P2PJob();
            BlockService = new BlockJob();
        }
        public override JobStatus Status
        {
            get
            {
                if (P2PJob.Status == JobStatus.Running &&
                    RpcService.Status == JobStatus.Running &&
                    BlockService.Status == JobStatus.Running)
                {
                    return JobStatus.Running;
                }
                else if (P2PJob.Status == JobStatus.Stopped &&
                    RpcService.Status == JobStatus.Stopped &&
                    BlockService.Status == JobStatus.Stopped)
                {
                    return JobStatus.Stopped;
                }
                else
                {
                    return JobStatus.Stopping;
                }
            }
        }

        public override void Start()
        {
            P2PJob.Start();
            RpcService.Start();
            BlockService.Start();
            IsRunning = true;
            while (true)
            {
                if (!IsRunning)
                {
                    break;
                }
                Thread.Sleep(1000);
            }
        }

        public override void Stop()
        {
            P2PJob.Stop();
            RpcService.Stop();
            BlockService.Stop();
            IsRunning = false;
        }

        public Dictionary<string, string> GetJobStatus()
        {
            var dict = new Dictionary<string, string>();

            dict.Add("ChainService", this.Status.ToString());
            dict.Add("P2pService", P2PJob.Status.ToString());
            dict.Add("BlockService", BlockService.Status.ToString());
            dict.Add("RpcService", RpcService.Status.ToString());
            dict.Add("ChainNetwork", GlobalParameters.IsTestnet ? "Testnet" : "Mainnet");
            dict.Add("Height", new BlockComponent().GetLatestHeight().ToString());

            return dict;
        }

        public static void Initialize()
        {
            var notify = new NotifyComponent();
            BlockchainComponent blockChainComponent = new BlockchainComponent();
            AccountComponent accountComponent = new AccountComponent();

            BlockchainJob.Current = new BlockchainJob();

            //从配置文件中读取
            ConfigurationTool tool = new ConfigurationTool();
            NodeConfig config = tool.GetAppSettings<NodeConfig>("NodeConfig");

            if(config != null)
            {
                notify.SetCallbackApp(config.WalletNotify);
                BlockchainJob.Current.P2PJob.IPAddress = config.Ip;
            }

            if (GlobalActions.TransactionNotifyAction == null)
            {
                GlobalActions.TransactionNotifyAction = NewTransactionNotify;
            }

            blockChainComponent.Initialize();
            var accounts = accountComponent.GetAllAccountsInDb();
            if (accounts.Count == 0)
            {
                var account = accountComponent.GenerateNewAccount(false);
                accountComponent.SetDefaultAccount(account.Id);

                UserSettingComponent component = new UserSettingComponent();
                component.SetDefaultAccount(account.Id);

                accounts.Add(account);;
            }

            Ready.ReadyCacheData(accounts);
            
            //暂时停用utxo
            //Task.Run(() =>
            //{
            //    UtxoComponent utxoComponent = new UtxoComponent();
            //    utxoComponent.Initialize();
            //});
        }

        public static void NewTransactionNotify(string txHash)
        {
            NotifyComponent notify = new NotifyComponent();
            notify.ProcessNewTxReceived(txHash);
        }
    }
}
