using FiiiChain.Entities;
using FiiiChain.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiiicoin.Messages
{
    public class OutputMsg : BasePayload
    {
        public string Hash { get; set; }
        public long Amount { get; set; }
        public int Size { get; set; }
        public string LockScript { get; set; }

        public string GetHash()
        {
            var bytes = new List<byte>();
            var amountBytes = BitConverter.GetBytes(Amount);
            var sizeBytes = BitConverter.GetBytes(Size);
            var scriptBytes = Encoding.UTF8.GetBytes(LockScript);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(amountBytes);
                Array.Reverse(sizeBytes);
            }

            bytes.AddRange(amountBytes);
            bytes.AddRange(sizeBytes);
            bytes.AddRange(scriptBytes);

            return Base16.Encode(
                HashHelper.Hash(
                    bytes.ToArray()
                ));
        }

        public override void Deserialize(byte[] bytes, ref int index)
        {
            var hashBytes = new byte[32];
            var amountBytes = new byte[8];
            var sizeBytes = new byte[4];

            Array.Copy(bytes, index, hashBytes, 0, hashBytes.Length);
            this.Hash = Base16.Encode(hashBytes);

            index += hashBytes.Length;
            Array.Copy(bytes, index, amountBytes, 0, amountBytes.Length);
            index += amountBytes.Length;
            Array.Copy(bytes, index, sizeBytes, 0, sizeBytes.Length);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(amountBytes);
                Array.Reverse(sizeBytes);
            }

            this.Amount = BitConverter.ToInt64(amountBytes, 0);
            this.Size = BitConverter.ToInt32(sizeBytes, 0);

            index += sizeBytes.Length;
            var scriptBytes = new byte[Size];
            Array.Copy(bytes, index, scriptBytes, 0, this.Size);
            this.LockScript = Encoding.UTF8.GetString(scriptBytes);

            index += Size;
        }

        public override byte[] Serialize()
        {
            var bytes = new List<byte>();
            var hashBytes = Base16.Decode(Hash);
            var amountBytes = BitConverter.GetBytes(Amount);
            var sizeBytes = BitConverter.GetBytes(Size);
            var scriptBytes = Encoding.UTF8.GetBytes(LockScript);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(amountBytes);
                Array.Reverse(sizeBytes);
            }

            bytes.AddRange(hashBytes);
            bytes.AddRange(amountBytes);
            bytes.AddRange(sizeBytes);
            bytes.AddRange(scriptBytes);

            return bytes.ToArray();
        }

        public Output ConvertToEntity()
        {
            var output = new Output();
            output.Hash = this.Hash;
            //output.TransactionHash = 
            //output.ReceiverId = 
            output.Amount = this.Amount;
            output.Size = this.Size;
            output.LockScript = this.LockScript;

            return output;
        }

        public static OutputMsg LoadFromEntity(Output entity)
        {
            var msg = new OutputMsg();
            msg.Hash = entity.Hash;
            msg.Amount = entity.Amount;
            msg.Size = entity.Size;
            msg.LockScript = entity.LockScript;

            return msg;
        }
    }
}
