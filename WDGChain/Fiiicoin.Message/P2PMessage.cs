using Fiiicoin.Messages;
using System;

namespace Fiiicoin.Message
{
    public class P2PMessage
    {
        public int Prefix { get; set; }
        public int Nonce { get; set; }
        public string Command { get; set; }
        public int Size { get; set; }
        public BasePayload Payload { get; set; }
        public int Checksum { get; set; }
        public int Suffix { get; set; }
    }
}
