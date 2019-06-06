// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.Entities;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FiiiChain.Framework;

namespace FiiiChain.Data.Accesses
{
    public class BlockAccess
    {
        private static BlockAccess _current;
        public static BlockAccess Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new BlockAccess();
                }
                return _current;
            }
        }

        public void Save(Block block, Action<int> continueAction)
        {
            Task continueTask = null;
            var task = new Task<int>(() =>
            {
                try
                {
                    BlockDac blockDac = BlockDac.Default;
                    if (blockDac.SelectByHash(block.Hash) != null)
                        return -1;
                    var result = blockDac.Save(block);
                    return result;
                }
                catch(Exception ex)
                {
                    Framework.LogHelper.Error(ex.ToString());
                    return -1;
                }
            });
            task.Start();
            Task.WaitAll(task);

            if (continueAction != null && task.IsCompleted)
            {
                Task.Run(() => { continueAction(task.Result); });
            }

            //TaskQueue.TaskWorker.Add(task, "SaveBlock");
            //if (continueTask == null)
            //    Task.WaitAll(task);
            //else
            //    Task.WaitAll(task, continueTask);
        }

        public void UpdateNextBlockHash(string currentHash, string nextHash, Action continueAction)
        {
            Task continueTask = null;
            var task = new Task(() =>
            {
                BlockDac blockDac = BlockDac.Default;
                blockDac.UpdateNextBlockHash(currentHash, nextHash);
            });
            if (continueAction != null)
            {
                continueTask = task.ContinueWith(t =>
                {
                    continueAction();
                });
            }
            TaskQueue.TaskWorker.Add(task, "UpdateNextBlockHash");
            if (continueTask == null)
                Task.WaitAll(task);
            else
                Task.WaitAll(task, continueTask);
        }

        public void UpdateBlockStatusToDiscarded(string hash, Action continueAction)
        {
            var task = new Task(() =>
            {
                BlockDac blockDac = BlockDac.Default;
                blockDac.UpdateBlockStatusToDiscarded(hash);
            });

            task.Start();
            Task.WaitAll(task);
            if (task.IsCompleted && continueAction != null)
            {
                Task.Run(() => { continueAction(); });
            }
        }

        public void UpdateBlockStatusToConfirmed(string hash, Action continueAction)
        {
            Task continueTask = null;
            var task = new Task(() =>
            {
                BlockDac blockDac = BlockDac.Default;
                blockDac.UpdateBlockStatusToConfirmed(hash);
            });
            if (continueAction != null)
            {
                continueTask = task.ContinueWith(t =>
                {
                    continueAction();
                });
            }

            task.Start();
            //TaskQueue.TaskWorker.Add(task, "UpdateBlockStatusToConfirmed");
            if (continueTask == null)
                Task.WaitAll(task);
            else
                Task.WaitAll(task, continueTask);
        }
    }
}