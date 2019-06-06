// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FiiiChain.Data.Accesses
{
    public sealed class TaskQueue
    { 
        public static TaskWork TaskWorker = new TaskWork();

        public static Task AddAction(Action action)
        {
            var task = new Task(() => {
                action.Invoke();
            });
            TaskWorker.Add(task);
            return task;
        }

        public static void AddWaitAction(Action action,string actionName = null)
        {
            //var waitTaskCount = TaskWorker.Count;
            //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //sw.Start();
            var task = new Task(() => {
                action.Invoke();
            });
            TaskWorker.Add(task,actionName);
            Task.WaitAll(task);
            //sw.Stop();
            //if (!string.IsNullOrWhiteSpace(actionName))
            //{
            //    Framework.LogHelper.Warn($"{actionName} 等待中任务数 == {waitTaskCount} ,AddExec耗时:{sw.ElapsedMilliseconds}");
            //}
        }
    }
}
