// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Business;
using WDG.Models;
using WDG.Utility.Api;
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Models;
using System.Threading;
using System.Threading.Tasks;

namespace WDG.Wallet.Win.Biz.Services
{
    public class EngineService : InstanceBase<EngineService>
    {
        public IResult AppClosed()
        {
            Result result = new Result();
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            Task stopEngineTask = new Task(async () =>
            {
                await BlockChainEngineApi.StopEngine();
            });
            stopEngineTask.Start();

            try
            {
                Task task = new Task(async () =>
                {
                    ApiResponse response = await BlockChainEngineApi.GetBlockChainStatus();
                    if (!response.HasError)
                    {
                        bool flag = true;
                        while (flag)
                        {
                            BlockChainStatus blockChainStatus = response.GetResult<BlockChainStatus>();
                            if (blockChainStatus.RpcService == "Stopped")
                            {
                                result.IsFail = response.HasError;
                                result.ApiResponse = response;
                                flag = false;
                                autoResetEvent.Set();
                                break;
                            }
                            Thread.Sleep(1000);
                        }
                    }
                });
                task.Start();
            }
            catch
            {
                //在这里捕获错误，然后关闭整个Application
                result.IsFail = true;
                autoResetEvent.Set();
            }
            autoResetEvent.WaitOne();
            return result;
        }
    }
}
