// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using WDG.Utility;
using System.Windows;
using WDG.Wallet.Win.Models;
using System.Threading.Tasks;
using System.Threading;

namespace WDG.Wallet.Win.Common.Proxys
{
    class LogProxy<T> : RealProxy
    {
        private T _target;
        public LogProxy(T target) : base(typeof(T))
        {
            this._target = target;
        }

        public override IMessage Invoke(IMessage msg)
        {
            IMethodCallMessage callMessage = (IMethodCallMessage)msg;
            //PreProceede(callMessage);

            //AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            object returnResult = null;
            var task = Task.Factory.StartNew(() =>
            {
                try
                {
                    returnResult = callMessage.MethodBase.Invoke(this._target, callMessage.Args);
                }
                catch (Exception ex)
                {
                    Logger.Singleton.Error(string.Format("Error Method in {0}", callMessage.MethodName));
                    Logger.Singleton.Error(ex.ToString());
                }
            });
            Task.WaitAll(task);

            if (returnResult is Result)
            {
                var result = returnResult as Result;
                PostProceede(callMessage.MethodName, result);
            }

            return new ReturnMessage(returnResult, new object[0], 0, null, callMessage);
        }

        public void PreProceede(IMethodCallMessage msg)
        {
            var methodName = msg.MethodName;
            Application.Current.Dispatcher.Invoke(() =>
            {
                Logger.Singleton.Info(methodName);
            });
        }

        public void PostProceede(string methodName, Result result)
        {
            if (result == null) return;

            if (result.ApiResponse == null || result.ApiResponse.Result == null)
                return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                if (result.ApiResponse.HasError)
                {
                    Logger.Singleton.InfoFormat("Method({0}) ErrorCode = {1} , ErrorMessage = {2}", methodName, result.ApiResponse.Error.Code, result.GetErrorMsg());
                }
                else
                {
                    var receiveContent = result.ApiResponse.Result.ToString();
                    Logger.Singleton.InfoFormat("Method({0}) Result = {1}", methodName, receiveContent.Replace("\r\n", ""));
                }
            });
        }
    }

    //TransparentProxy
    public static class TransparentProxy
    {
        public static T Create<T>()
        {
            T instance = Activator.CreateInstance<T>();
            LogProxy<T> realProxy = new LogProxy<T>(instance);
            T transparentProxy = (T)realProxy.GetTransparentProxy();
            return transparentProxy;
        }
    }
}