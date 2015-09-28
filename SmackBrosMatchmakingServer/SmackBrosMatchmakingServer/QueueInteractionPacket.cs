using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SmackBrosMatchmakingServer
{
    class QueueInteractionPacket : Packet
    {
        public string name;
        public short mmr;
        public string IPAddress;
        public bool joining;
        public QueueInteractionPacket(bool joiningQueue)
        {
            joining = joiningQueue;
            typeID = 1;
        }
        public override void WritePacketData(List<byte> stream)
        {
            WriteBool(stream, joining);
            WriteStringBytes(stream, name);
            WriteShort(stream, mmr);
            WriteStringBytes(stream, IPAddress);
        }
        public override void ReadPacketData(Stream stream)
        {
            joining = ReadBoolFromStream(stream);
            name = ReadStringFromStream(stream);
            mmr = ReadShortFromStream(stream);
            IPAddress = ReadStringFromStream(stream);
        }
    }
}
