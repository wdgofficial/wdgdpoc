using FiiiChain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiiicoin.Messages
{
    public class BlockMsg : BasePayload
    {
        public BlockMsg()
        {
            this.Transactions = new List<TransactionMsg>();
        }

        public BlockHeaderMsg Header { get; set; }
        public List<TransactionMsg> Transactions { get; set; }

        public override void Deserialize(byte[] bytes, ref int index)
        {
            this.Header = new BlockHeaderMsg();
            this.Header.Deserialize(bytes, ref index);

            var txIndex = 0;
            while(txIndex < this.Header.TotalTransaction)
            {
                var transactionMsg = new TransactionMsg();
                transactionMsg.Deserialize(bytes, ref index);
                this.Transactions.Add(transactionMsg);

                txIndex++;
            }
        }

        public override byte[] Serialize()
        {
            var bytes = new List<byte>();
            bytes.AddRange(Header.Serialize());
            
            foreach(var tx in Transactions)
            {
                bytes.AddRange(tx.Serialize());
            }

            return bytes.ToArray();
        }

        public Block ConvertToEntity()
        {
            var entity = new Block();
            entity.Hash = this.Header.Hash;
            entity.Version = this.Header.Version;
            entity.Height = this.Header.Height;
            entity.PreviousBlockHash = this.Header.PreviousBlockHash;
            entity.Bits = this.Header.Bits;
            entity.Nonce = this.Header.Nonce;
            entity.Timestamp = this.Header.Timestamp;
            //entity.NextBlockHash = 

            long totalAmount = 0L;
            long totalFee = 0L;

            foreach(var transactionMsg in this.Transactions)
            {
                var transaction = transactionMsg.ConvertToEntity();
                entity.Transactions.Add(transaction);
                
                foreach(var outputMsg in transactionMsg.Outputs)
                {
                    totalAmount += outputMsg.Amount;
                }

                totalFee += transaction.Fee;
            }

            entity.TotalAmount = totalAmount;
            entity.TotalFee = totalFee;
            //entity.GeneratorId = ;
            //entity.IsDiscarded = ;
            //entity.IsVerified = ;

            return entity;
        }

        public static BlockMsg LoadFromEntity(Block entity)
        {
            var msg = new BlockMsg();
            msg.Header = new BlockHeaderMsg();
            msg.Header.Version = entity.Version;
            msg.Header.Hash = entity.Hash;
            msg.Header.Height = entity.Height;
            msg.Header.PreviousBlockHash = entity.PreviousBlockHash;
            msg.Header.Bits = entity.Bits;
            msg.Header.Nonce = entity.Nonce;
            msg.Header.Timestamp = entity.Timestamp;

            foreach(var transaction in entity.Transactions)
            {
                var txMsg = TransactionMsg.LoadFromEntity(transaction);
                msg.Transactions.Add(txMsg);
            }

            msg.Header.TotalTransaction = msg.Transactions.Count;
            return msg;
        }
    }
}
