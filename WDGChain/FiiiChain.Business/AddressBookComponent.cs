// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.Messages;
using FiiiChain.Entities;
using FiiiChain.DataAgent;
using System;
using System.Collections.Generic;
using System.Text;
using FiiiChain.Data;
using FiiiChain.Framework;
using FiiiChain.Consensus;
using System.Linq;
using System.Threading.Tasks;
using FiiiChain.Data.Accesses;

namespace FiiiChain.Business
{
    public class AddressBookComponent
    {
        public void SetTag(string address, string tag)
        {
            if(AccountDac.Default.SelectById(address) == null)
            {
                var task = new Task(() =>
                {
                    AddressBookDac.Default.InsertOrUpdate(address, tag);
                });
                TaskQueue.TaskWorker.Add(task, "SetTag");
                Task.WaitAll(task);
            }
        }

        public void Delete(string address)
        {
            TaskQueue.AddWaitAction(() =>
            {
                AddressBookDac.Default.Delete(address);
            }, "DeleteAddressbook");
        }

        public void DeleteByIds(long[] ids)
        {
            TaskQueue.AddWaitAction(() =>
            {
                AddressBookDac.Default.DeleteByIds(ids);
            }, "DeleteByIds");
        }

        public List<AddressBookItem> GetWholeAddressBook()
        {
            return AddressBookDac.Default.SelectWholeAddressBook();
        }

        public List<AddressBookItem> GetAddressBookByIds(long[] ids)
        {
            return AddressBookDac.Default.SelectByIds(ids);
        }

        public List<AddressBookItem> GetByTag(string tag)
        {
            return AddressBookDac.Default.SelectAddessListByTag(tag);
        }

        public AddressBookItem GetByAddress(string address)
        {
            return AddressBookDac.Default.SelectByAddress(address);
        }

        public string GetTagByAddress(string address)
        {
            var item = AddressBookDac.Default.SelectByAddress(address);

            if(item != null)
            {
                return item.Tag;
            }
            else
            {
                return null;
            }
        }
    }
}
