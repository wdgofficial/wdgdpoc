// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using Castle.DynamicProxy;
using FiiiChain.Data.Accesses;
using FiiiChain.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace FiiiChain.Data.Proxy
{
    class LogProxy : StandardInterceptor
    {
        /// <summary>
        /// 创建代理类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateProxy<T>() where T : class
        {
            ProxyGenerator generator = new ProxyGenerator();
            var testa = generator.CreateClassProxy<T>(new LogProxy());
            return testa;
        }

        public void PreProceede(MethodInfo msg)
        {
            var methodName = msg.Name;
            LogHelper.Info(methodName);
        }

        protected override void PerformProceed(IInvocation invocation)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                //LogHelper.Warn($"start method {invocation.Method.Name}");
                base.PerformProceed(invocation);
            }
            catch (Exception ex)
            {
                //LogHelper.Debug(string.Format("Error Method in {0}", invocation.Method.Name));
                LogHelper.Debug(ex.ToString());
            }
            stopwatch.Stop();
            if (stopwatch.ElapsedMilliseconds > 1000)
                LogHelper.Warn($"{invocation.Method.Name} use time {stopwatch.ElapsedMilliseconds} ms");
        }
    }

    //TransparentProxy
    public static class TransparentProxy
    {
        public static T Create<T>() where T : class
        {
            //var proxy = LogProxy.CreateProxy<T>();
            //return proxy;
            var proxy = System.Activator.CreateInstance<T>();
            return proxy;
        }
    }
}