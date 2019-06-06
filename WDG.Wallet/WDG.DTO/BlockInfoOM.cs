// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

using WDG.Utility;
using WDG.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace WDG.DTO
{
    public class BlockInfoOM
    {
        public BlockHeaderOM Header { get; set; }
        public BlockTransactionsOM[] Transactions { get; set; }
        public static bool Verify(string target, string result)
        {
            //TODO
            //两个字符串长度一样，都是16进制的

            if (target.Length % 2 != 0)
            {
                target = "0" + target;
            }

            var targetFirstByte = Convert.ToByte(target.Substring(0, 2), 16);

            if (targetFirstByte > 0x7f)
            {
                target = "00" + target;
            }

            if (result.Length % 2 != 0)
            {
                result = "0" + result;
            }

            var resultFirstByte = Convert.ToByte(result.Substring(0, 2), 16);

            if (resultFirstByte > 0x7f)
            {
                result = "00" + result;
            }


            BigInteger targetValue =  BigInteger.Parse(target, System.Globalization.NumberStyles.AllowHexSpecifier);
            BigInteger resultValue = BigInteger.Parse(result, System.Globalization.NumberStyles.AllowHexSpecifier);
            if (targetValue >= resultValue)
            {
                return true;
            }
            return false;
        }

        public byte[] Serialize()
        {
            var bytes = new List<byte>();
            bytes.AddRange(Header.Serialize());

            foreach (var tx in Transactions)
            {
                bytes.AddRange(tx.Serialize());
            }

            return bytes.ToArray();
        }
    }

    public class BlockHeaderOM
    {
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

        public void Deserialize(byte[] bytes, ref int index)
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

        public byte[] Serialize()
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

    public class BlockTransactionsOM
    {
        public int Version { get; set; }
        public string Hash { get; set; }
        public long Timestamp { get; set; }
        public long Locktime { get; set; }
        public int InputCount
        {
            get { return Inputs.Count; }
        }
        public int OutputCount
        {
            get { return Outputs.Count; }
        }
        public List<BlockTransactionInputsOM> Inputs { get; set; }
        public List<BlockTransactionOutputsOM> Outputs { get; set; }
        public long Size { get; set; }

        public byte[] Serialize()
        {
            List<byte> bytes = new List<byte>();
            byte[] versionBytes = new byte[4];
            byte[] hashBytes = new byte[32];
            byte[] timestampBytes = new byte[8];
            byte[] locktimeBytes = new byte[8];
            byte[] totalInputBytes = new byte[4];
            byte[] totalOutputBytes = new byte[4];

            versionBytes = BitConverter.GetBytes(Version);
            hashBytes = Base16.Decode(Hash);
            timestampBytes = BitConverter.GetBytes(Timestamp);
            locktimeBytes = BitConverter.GetBytes(Locktime);
            totalInputBytes = BitConverter.GetBytes(InputCount);
            totalOutputBytes = BitConverter.GetBytes(OutputCount);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(versionBytes);
                Array.Reverse(timestampBytes);
                Array.Reverse(locktimeBytes);
                Array.Reverse(totalInputBytes);
                Array.Reverse(totalOutputBytes);
            }

            bytes.AddRange(versionBytes);
            bytes.AddRange(hashBytes);
            bytes.AddRange(timestampBytes);
            bytes.AddRange(locktimeBytes);
            bytes.AddRange(totalInputBytes);

            foreach (BlockTransactionInputsOM inputMsg in Inputs)
            {
                bytes.AddRange(inputMsg.Serialize());
            }

            bytes.AddRange(totalOutputBytes);

            foreach (BlockTransactionOutputsOM outputMsg in Outputs)
            {
                bytes.AddRange(outputMsg.Serialize());
            }

            return bytes.ToArray();
        }

        public void Deserialize(byte[] bytes, ref int index)
        {
            var versionBytes = new byte[4];
            var hashBytes = new byte[32];
            var timestampBytes = new byte[8];
            var locktimeBytes = new byte[8];
            var totalInputBytes = new byte[4];
            var totalOutputBytes = new byte[4];

            Array.Copy(bytes, index, versionBytes, 0, versionBytes.Length);
            index += versionBytes.Length;
            Array.Copy(bytes, index, hashBytes, 0, hashBytes.Length);
            index += hashBytes.Length;
            Array.Copy(bytes, index, timestampBytes, 0, timestampBytes.Length);
            index += timestampBytes.Length;
            Array.Copy(bytes, index, locktimeBytes, 0, locktimeBytes.Length);
            index += locktimeBytes.Length;
            Array.Copy(bytes, index, totalInputBytes, 0, totalInputBytes.Length);
            index += totalInputBytes.Length;

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(versionBytes);
                Array.Reverse(timestampBytes);
                Array.Reverse(locktimeBytes);
                Array.Reverse(totalInputBytes);
            }

            Version = BitConverter.ToInt32(versionBytes, 0);
            Hash = Base16.Encode(hashBytes);
            Timestamp = BitConverter.ToInt64(timestampBytes, 0);
            Locktime = BitConverter.ToInt64(locktimeBytes, 0);
            var totalInput = BitConverter.ToInt32(totalInputBytes, 0);

            var inputIndex = 0;
            while (inputIndex < totalInput)
            {
                BlockTransactionInputsOM input = new BlockTransactionInputsOM();
                input.Deserialize(bytes, ref index);
                Inputs.Add(input);

                inputIndex++;
            }

            Array.Copy(bytes, index, totalOutputBytes, 0, totalOutputBytes.Length);
            index += totalOutputBytes.Length;

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(totalOutputBytes);
            }

            var totalOutput = BitConverter.ToInt32(totalOutputBytes, 0);

            var outputIndex = 0;
            while (outputIndex < totalOutput)
            {
                BlockTransactionOutputsOM output = new BlockTransactionOutputsOM();
                output.Deserialize(bytes, ref index);
                Outputs.Add(output);

                outputIndex++;
            }
        }
    }

    public class BlockTransactionInputsOM
    {
        public string OutputTransactionHash { get; set; }
        public int OutputIndex { get; set; }
        public int Size { get; set; }
        public string UnlockScript { get; set; }

        public byte[] Serialize()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(Base16.Decode(OutputTransactionHash));

            byte[] indexBytes = BitConverter.GetBytes(OutputIndex);
            byte[] sizeBytes = BitConverter.GetBytes(Size);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(indexBytes);
                Array.Reverse(sizeBytes);
            }

            bytes.AddRange(indexBytes);
            bytes.AddRange(sizeBytes);
            bytes.AddRange(Encoding.UTF8.GetBytes(UnlockScript));

            return bytes.ToArray();
        }

        public void Deserialize(byte[] bytes, ref int index)
        {
            byte[] txHashBytes = new byte[32];
            byte[] outputIndexBytes = new byte[4];
            byte[] sizeBytes = new byte[4];

            Array.Copy(bytes, index, txHashBytes, 0, txHashBytes.Length);
            index += txHashBytes.Length;
            OutputTransactionHash = Base16.Encode(txHashBytes);

            Array.Copy(bytes, index, outputIndexBytes, 0, outputIndexBytes.Length);
            index += outputIndexBytes.Length;

            Array.Copy(bytes, index, sizeBytes, 0, sizeBytes.Length);
            index += sizeBytes.Length;

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(outputIndexBytes);
                Array.Reverse(sizeBytes);
            }

            OutputIndex = BitConverter.ToInt32(outputIndexBytes, 0);
            Size = BitConverter.ToInt32(sizeBytes, 0);
            byte[] scriptBytes = new byte[Size];
            Array.Copy(bytes, index, scriptBytes, 0, Size);
            UnlockScript = Encoding.UTF8.GetString(scriptBytes);

            index += Size;
        }

    }

    public class BlockTransactionOutputsOM
    {
        public int Index { get; set; }
        public long Amount { get; set; }
        public int Size { get; set; }
        public string LockScript { get; set; }

        public byte[] Serialize()
        {
            List<byte> bytes = new List<byte>();
            byte[] indexBytes = BitConverter.GetBytes(Index);
            byte[] amountBytes = BitConverter.GetBytes(Amount);
            byte[] sizeBytes = BitConverter.GetBytes(Size);
            byte[] scriptBytes = Encoding.UTF8.GetBytes(LockScript);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(indexBytes);
                Array.Reverse(amountBytes);
                Array.Reverse(sizeBytes);
            }

            bytes.AddRange(indexBytes);
            bytes.AddRange(amountBytes);
            bytes.AddRange(sizeBytes);
            bytes.AddRange(scriptBytes);

            return bytes.ToArray();
        }

        public void Deserialize(byte[] bytes, ref int index)
        {
            byte[] indexBytes = new byte[4];
            byte[] amountBytes = new byte[8];
            byte[] sizeBytes = new byte[4];

            Array.Copy(bytes, index, indexBytes, 0, indexBytes.Length);
            index += indexBytes.Length;

            Array.Copy(bytes, index, amountBytes, 0, amountBytes.Length);
            index += amountBytes.Length;

            Array.Copy(bytes, index, sizeBytes, 0, sizeBytes.Length);
            index += sizeBytes.Length;

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(indexBytes);
                Array.Reverse(amountBytes);
                Array.Reverse(sizeBytes);
            }

            Index = BitConverter.ToInt32(indexBytes, 0);
            Amount = BitConverter.ToInt64(amountBytes, 0);
            Size = BitConverter.ToInt32(sizeBytes, 0);

            byte[] scriptBytes = new byte[Size];
            Array.Copy(bytes, index, scriptBytes, 0, Size);
            LockScript = Encoding.UTF8.GetString(scriptBytes);

            index += Size;
        }
    }
}
