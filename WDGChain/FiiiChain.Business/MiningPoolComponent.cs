// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.Consensus;
using FiiiChain.Data;
using FiiiChain.Data.Accesses;
using FiiiChain.DataAgent;
using FiiiChain.Entities;
using FiiiChain.Framework;
using FiiiChain.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace FiiiChain.Business
{
    public class MiningPoolComponent
    {
        public static Action<NewMiningPoolMsg> OnNewMiningPoolHandle; 

        public static List<MiningPool> CurrentMiningPools = new List<MiningPool>();

        public static void LoadMiningPools()
        {
            CurrentMiningPools = MiningPoolDac.Default.GetAllMiningPools();
        }

        public MiningPoolComponent()
        {
            if (CurrentMiningPools == null)
            {
                CurrentMiningPools = MiningPoolDac.Default.GetAllMiningPools();
            }
        }

        public MiningMsg CreateNewMiningPool(string minerName, string publicKey)
        {
            MiningPoolDac miningPoolDac = MiningPoolDac.Default;
            MiningMsg newMiningPoolMsg = new MiningMsg();
            newMiningPoolMsg.Name = minerName;
            newMiningPoolMsg.PublicKey = publicKey;
            return newMiningPoolMsg;
        }

        private List<MiningPool> GetAllMiningPoolsInDb()
        {
            MiningPoolDac miningPoolDac = MiningPoolDac.Default;
            var result = miningPoolDac.GetAllMiningPools();
            return result;
        }

        public List<MiningPool> GetAllMiningPools()
        {
            return MiningPoolComponent.CurrentMiningPools.ToList();
        }


        public void UpdateMiningPools(List<MiningMsg> miningMsgs)
        {
            MiningPoolDac miningPoolDac = MiningPoolDac.Default;
            List<MiningMsg> needUpdateItems = null;
            List<MiningMsg> needAddItems = null;
            GetItemsUpdate(miningMsgs, out needUpdateItems, out needAddItems);

            var updateItems = needUpdateItems.Select(x => ConvertToMiningPool(x));
            var addItems = needAddItems.Select(x => ConvertToMiningPool(x));

            if (!updateItems.Any() && !addItems.Any())
                return;

            //TaskQueue.AddWaitAction(() =>
            //{
                if (updateItems.Any())
                {
                    miningPoolDac.UpdateMiningPools(updateItems);
                }
                if (addItems.Any())
                {
                    miningPoolDac.SaveToDB(addItems);
                }
                if (updateItems.Any() || addItems.Any())
                {
                    CurrentMiningPools = this.GetAllMiningPoolsInDb();
                }
            //});
        }

        public bool AddMiningToPool(MiningMsg msg)
        {
            if (!POC.VerifyMiningPoolSignature(msg.PublicKey, msg.Signature))
                return false;

            var result = false;
            var item = CurrentMiningPools.FirstOrDefault(x => x.PublicKey == msg.PublicKey && x.Signature == msg.Signature);
            if (item == null)
            {
                MiningPoolDac miningPoolDac = MiningPoolDac.Default;
                MiningPool miningPool = ConvertToMiningPool(msg);
                result = miningPoolDac.SaveToDB(miningPool) > 0;
                result = true;
            }
            else if (item.Name != msg.Name)
            {
                MiningPoolDac miningPoolDac = MiningPoolDac.Default;
                MiningPool miningPool = new MiningPool() { Name = msg.Name, PublicKey = msg.PublicKey, Signature = msg.Signature };
                miningPoolDac.UpdateMiningPool(miningPool);
                result = true;
            }
            if (result && OnNewMiningPoolHandle != null)
            {
                NewMiningPoolMsg newMsg = new NewMiningPoolMsg();
                newMsg.MinerInfo = new MiningMsg() { Name = msg.Name, PublicKey = msg.PublicKey, Signature = msg.Signature };
                OnNewMiningPoolHandle(newMsg);
            }

            CurrentMiningPools = GetAllMiningPoolsInDb();

            return result;
        }

        private MiningPool ConvertToMiningPool(MiningMsg msg)
        {
            MiningPool miningPool = new MiningPool() { Name = msg.Name, PublicKey = msg.PublicKey, Signature = msg.Signature };
            return miningPool;
        }

        void GetItemsUpdate(List<MiningMsg> miningMsgs ,out List<MiningMsg> updateItems,out List<MiningMsg> addItems)
        {
            updateItems = new List<MiningMsg>();
            addItems = new List<MiningMsg>();
            foreach (var item in miningMsgs)
            {
                if (item.Signature.Length != 128)
                {
                    continue;
                }

                if (!CurrentMiningPools.Any(x => x.PublicKey == item.PublicKey))
                {
                    addItems.Add(item);
                }
                else if (CurrentMiningPools.Any(x => x.PublicKey == item.PublicKey && x.Name != item.Name))
                {
                    updateItems.Add(item);
                }
            }
        }


        public long GetLocalMiningPoolCount()
        {
            return CurrentMiningPools.Count;
        }

        public MiningPool GetMiningPoolByName(string poolName)
        {
            return MiningPoolDac.Default.SelectMiningPoolByName(poolName);
        }
    }
}
