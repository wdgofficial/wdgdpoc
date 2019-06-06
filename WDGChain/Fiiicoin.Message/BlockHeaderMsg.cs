using FiiiChain.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiiicoin.Messages
{
    public class BlockHeaderMsg : BasePayload
    {
        public BlockHeaderMsg()
        {
            this.Version = Int32.Parse(Resource.MsgVersion);
        }

        public int Version { get; set; }
        public string Hash { get; set; }
        public long Height { get; set; }
        public string PreviousBlockHash { get; set; }
        public long Bits { get; set; }
        public long Nonce { get; set; }
        public long Timestamp { get; set; }
        public int TotalTransaction { get; set; }

        public string GetHash()
        {
            var bytes = new List<byte>();
            var previousBlockHashBytes = new byte[32];
            var bitsBytes = new byte[8];
            var nonceBytes = new byte[8];
            var timestampBytes = new byte[8];

            previousBlockHashBytes = Base16.Decode(PreviousBlockHash);
            bitsBytes = BitConverter.GetBytes(Bits);
            nonceBytes = BitConverter.GetBytes(Nonce);
            timestampBytes = BitConverter.GetBytes(Timestamp);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bitsBytes);
                Array.Reverse(nonceBytes);
                Array.Reverse(timestampBytes);
            }

            bytes.AddRange(previousBlockHashBytes);
            bytes.AddRange(bitsBytes);
            bytes.AddRange(nonceBytes);
            bytes.AddRange(timestampBytes);

            return Base16.Encode(
                HashHelper.Hash(
                    bytes.ToArray()
            ));
        }

        public override void Deserialize(byte[] bytes, ref int index)
        {
            var versionBytes = new byte[4];
            var hashBytes = new byte[32];
            var heightBytes = new byte[8];
            var previousBlockHashBytes = new byte[32];
            var bitsBytes = new byte[8];
            var nonceBytes = new byte[8];
            var timestampBytes = new byte[8];
            var totalTransactionBytes = new byte[4];

            Array.Copy(bytes, index, versionBytes, 0, versionBytes.Length);
            index += versionBytes.Length;

            Array.Copy(bytes, index, hashBytes, 0, hashBytes.Length);
            index += hashBytes.Length;

            Array.Copy(bytes, index, heightBytes, 0, heightBytes.Length);
            index += heightBytes.Length;

            Array.Copy(bytes, index, previousBlockHashBytes, 0, previousBlockHashBytes.Length);
            index += previousBlockHashBytes.Length;

            Array.Copy(bytes, index, bitsBytes, 0, bitsBytes.Length);
            index += bitsBytes.Length;

            Array.Copy(bytes, index, nonceBytes, 0, nonceBytes.Length);
            index += nonceBytes.Length;

            Array.Copy(bytes, index, timestampBytes, 0, timestampBytes.Length);
            index += timestampBytes.Length;

            Array.Copy(bytes, index, totalTransactionBytes, 0, totalTransactionBytes.Length);
            index += totalTransactionBytes.Length;

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(versionBytes);
                Array.Reverse(heightBytes);
                Array.Reverse(bitsBytes);
                Array.Reverse(nonceBytes);
                Array.Reverse(timestampBytes);
                Array.Reverse(totalTransactionBytes);
            }

            this.Version = BitConverter.ToInt32(versionBytes, 0);
            this.Hash = Base16.Encode(hashBytes);
            this.Height = BitConverter.ToInt64(heightBytes, 0);
            this.PreviousBlockHash = Base16.Encode(previousBlockHashBytes);
            this.Bits = BitConverter.ToInt64(bitsBytes, 0);
            this.Nonce = BitConverter.ToInt64(nonceBytes, 0);
            this.Timestamp = BitConverter.ToInt64(timestampBytes, 0);
            this.TotalTransaction = BitConverter.ToInt32(totalTransactionBytes, 0);
        }

        public override byte[] Serialize()
        {
            var bytes = new List<byte>();
            var versionBytes = new byte[4];
            var hashBytes = new byte[32];
            var heightBytes = new byte[8];
            var previousBlockHashBytes = new byte[32];
            var bitsBytes = new byte[8];
            var nonceBytes = new byte[8];
            var timestampBytes = new byte[8];
            var totalTransactionBytes = new byte[4];

            versionBytes = BitConverter.GetBytes(Version);
            hashBytes = Base16.Decode(Hash);
            heightBytes = BitConverter.GetBytes(Height);
            previousBlockHashBytes = Base16.Decode(PreviousBlockHash);
            bitsBytes = BitConverter.GetBytes(Bits);
            nonceBytes = BitConverter.GetBytes(Nonce);
            timestampBytes = BitConverter.GetBytes(Timestamp);
            totalTransactionBytes = BitConverter.GetBytes(TotalTransaction);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(versionBytes);
                Array.Reverse(heightBytes);
                Array.Reverse(bitsBytes);
                Array.Reverse(nonceBytes);
                Array.Reverse(timestampBytes);
                Array.Reverse(totalTransactionBytes);
            }

            bytes.AddRange(versionBytes);
            bytes.AddRange(hashBytes);
            bytes.AddRange(heightBytes);
            bytes.AddRange(previousBlockHashBytes);
            bytes.AddRange(bitsBytes);
            bytes.AddRange(nonceBytes);
            bytes.AddRange(timestampBytes);
            bytes.AddRange(totalTransactionBytes);

            return bytes.ToArray();
        }
    }
}
