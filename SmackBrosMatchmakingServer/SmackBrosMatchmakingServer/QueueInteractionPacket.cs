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
        string name;
        short mmr;
        public string IPAddress;
        bool joining;
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
        }
        public override void ReadPacketData(Stream stream)
        {
            joining = ReadBoolFromStream(stream);
            name = ReadStringFromStream(stream);
            mmr = ReadShortFromStream(stream);
        }
    }
}
