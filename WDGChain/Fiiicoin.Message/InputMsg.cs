using FiiiChain.Entities;
using FiiiChain.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiiicoin.Messages
{
    public class InputMsg : BasePayload
    {
        public string TransactionHash { get; set; }
        public string OutputHash { get; set; }
        public int Size { get; set; }
        public string UnlockScript { get; set; }

        public override void Deserialize(byte[] bytes, ref int index)
        {
            var txHashBytes = new byte[32];
            var outputHashBytes = new byte[32];
            var sizeBytes = new byte[4];

            Array.Copy(bytes, index, txHashBytes, 0, txHashBytes.Length);
            this.TransactionHash = Base16.Encode(txHashBytes);

            index += outputHashBytes.Length;
            Array.Copy(bytes, index, outputHashBytes, 0, outputHashBytes.Length);
            this.OutputHash = Base16.Encode(outputHashBytes);

            index += sizeBytes.Length;
            Array.Copy(bytes, index, sizeBytes, 0, sizeBytes.Length);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(sizeBytes);
            }

            this.Size = BitConverter.ToInt32(sizeBytes, 0);
            index += sizeBytes.Length;
            var scriptBytes = new byte[Size];
            Array.Copy(bytes, index, scriptBytes, 0, this.Size);
            this.UnlockScript = Encoding.UTF8.GetString(scriptBytes);

            index += Size;
        }

        public override byte[] Serialize()
        {
            var bytes = new List<byte>();
            bytes.AddRange(Base16.Decode(TransactionHash));
            bytes.AddRange(Base16.Decode(OutputHash));

            var sizeBytes = BitConverter.GetBytes(Size);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(sizeBytes);
            }

            bytes.AddRange(sizeBytes);
            bytes.AddRange(Encoding.UTF8.GetBytes(UnlockScript));

            return bytes.ToArray();

        }

        public Input ConvertToEntity()
        {
            var input = new Input();
            input.TransactionHash = this.TransactionHash;
            input.OutputHash = this.OutputHash;
            input.Size = this.Size;
            input.UnlockScript = this.UnlockScript;

            return input;
        }

        public static InputMsg LoadFromEntiyt(Input entity)
        {
            var msg = new InputMsg();
            msg.TransactionHash = entity.TransactionHash;
            msg.OutputHash = entity.OutputHash;
            msg.Size = entity.Size;
            msg.UnlockScript = entity.UnlockScript;

            return msg;
        }
    }
}
