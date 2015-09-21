using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SmackBrosMatchmakingServer
{
    class QueueFinishedPacket : Packet
    {
        string ipAddressToConnectTo;
        public override void WritePacketData(List<byte> stream)
        {
            WriteStringBytes(stream, ipAddressToConnectTo);
        }
        public override void ReadPacketData(Stream stream)
        {

        }
    }
}
