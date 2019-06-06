// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FiiiChain.Data.Accesses
{
    public class AccountAccess
    {
        private static AccountAccess _current;
        public static AccountAccess Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new AccountAccess();
                }
                return _current;
            }
        }

        public void Insert(Account account, Action continueAction = null)
        {
            Task continueTask = null;
            var task = new Task(() =>
            {
                AccountDac accountDac = AccountDac.Default;
                accountDac.Insert(account);
            });
            if (continueAction != null)
            {
                continueTask = task.ContinueWith(t =>
                {
                    continueAction();
                });
            }
            TaskQueue.TaskWorker.Add(task, "InsertAccount");
            if (continueTask == null)
                Task.WaitAll(task);
            else
                Task.WaitAll(task, continueTask);
        }

        public void UpdateBalance(string id, long amount, Action continueAction = null)
        {
            Task continueTask = null;
            var task = new Task(() =>
            {
                AccountDac accountDac = AccountDac.Default;
                accountDac.UpdateBalance(id, amount);
            });
            if (continueAction != null)
            {
                continueTask = task.ContinueWith(t =>
                {
                    continueAction();
                });
            }
            TaskQueue.TaskWorker.Add(task, "UpdateBalance");
            if (continueTask == null)
                Task.WaitAll(task);
            else
                Task.WaitAll(task, continueTask);
        }

        public void UpdateTag(string id, string tag, Action continueAction = null)
        {
            Task continueTask = null;
            var task = new Task(() =>
            {
                AccountDac accountDac = AccountDac.Default;
                accountDac.UpdateTag(id,tag);
            });
            if (continueAction != null)
            {
                continueTask = task.ContinueWith(t =>
                {
                    continueAction();
                });
            }
            TaskQueue.TaskWorker.Add(task, "UpdateTag");
            if (continueTask == null)
                Task.WaitAll(task);
            else
                Task.WaitAll(task, continueTask);
        }

        public void UpdatePrivateKeyAr(List<Account> aclist, Action<int> continueAction = null)
        {
            Task continueTask = null;
            var task = new Task<int>(() =>
            {
                AccountDac accountDac = AccountDac.Default;
                int result = accountDac.UpdatePrivateKeyAr(aclist);
                return result;
            });
            if (continueAction != null)
            {
                continueTask = task.ContinueWith(t =>
                {
                    continueAction(t.Result);
                });
            }
            TaskQueue.TaskWorker.Add(task, "UpdatePrivateKeyAr");
            if (continueTask == null)
                Task.WaitAll(task);
            else
                Task.WaitAll(task, continueTask);
        }

        public void SetDefaultAccount(string id, Action continueAction = null)
        {
            Task continueTask = null;
            var task = new Task(() =>
            {
                AccountDac accountDac = AccountDac.Default;
                accountDac.SetDefaultAccount(id);
            });
            if (continueAction != null)
            {
                continueTask = task.ContinueWith(t =>
                {
                    continueAction();
                });
            }
            TaskQueue.TaskWorker.Add(task, "SetDefaultAccount");
            if (continueTask == null)
                Task.WaitAll(task);
            else
                Task.WaitAll(task, continueTask);
        }
    }
}
