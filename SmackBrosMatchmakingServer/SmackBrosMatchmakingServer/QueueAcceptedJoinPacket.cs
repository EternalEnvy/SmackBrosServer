using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SmackBrosMatchmakingServer
{
    class QueueAcceptedJoinPacket : Packet
    {
        bool joinAccepted;
        public override void WritePacketData(List<byte> stream)
        {
            WriteBool(stream, true);
        }
        public override void ReadPacketData(Stream stream)
        {
            joinAccepted = ReadBoolFromStream(stream);
        }
    }
}
