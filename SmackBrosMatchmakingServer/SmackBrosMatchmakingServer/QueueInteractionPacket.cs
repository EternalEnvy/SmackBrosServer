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
        public QueueInteractionPacket()
        {
            typeID = 1;
        }
        public override void WritePacketData(List<byte> stream)
        {
            WriteStringBytes(stream, name);
        }
        public override void ReadPacketData(Stream stream)
        {
            name = ReadStringFromStream(stream);
            mmr = ReadShortFromStream(stream);
        }
    }
}
