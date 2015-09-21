using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SmackBrosMatchmakingServer
{
    abstract class Packet
    {
        protected byte typeID;
        public abstract void WritePacketData(List<byte> stream);
        public abstract void ReadPacketData(Stream stream);
        public static Packet ReadPacket(Stream stream)
        {
            var packetType = stream.ReadByte();
            Packet packet = null;
            if (packetType == 1)
            {
                packet = new QueueInteractionPacket();
                packet.ReadPacketData(stream);
            }
            if (packetType == 2)
            {
                packet = new QueueAcceptedJoinPacket();
                packet.ReadPacketData(stream);
            }
            if (packetType == 3)
            {
                packet = new QueueFinishedPacket();
                packet.ReadPacketData(stream);
            }
            return packet;
        }
        //read-write functions for data types
        public static short ReadShortFromStream(Stream stream)
        {
            var intBytes = new byte[2];
            stream.Read(intBytes, 0, 2);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(intBytes);
            return BitConverter.ToInt16(intBytes, 0);
        }
        public static int ReadIntFromStream(Stream stream)
        {
            var intBytes = new byte[4];
            stream.Read(intBytes, 0, 4);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(intBytes);
            return BitConverter.ToInt32(intBytes, 0);
        }
        public static bool ReadBoolFromStream(Stream stream)
        {
            var boolbyte = new byte[1];
            stream.Read(boolbyte, 0, 1);
            return BitConverter.ToBoolean(boolbyte, 0);
        }
        protected void WriteStringBytes(List<byte> stream, string str)
        {
            var numBytes = (short)ASCIIEncoding.ASCII.GetByteCount(str);

            var arr = new byte[2 + numBytes];

            var lengthBytes = BitConverter.GetBytes(numBytes);
            if (BitConverter.IsLittleEndian)
                lengthBytes = lengthBytes.Reverse().ToArray();
            lengthBytes.CopyTo(arr, 0);

            var stringBytes = ASCIIEncoding.ASCII.GetBytes(str);
            stringBytes.CopyTo(arr, 2);

            stream.AddRange(arr);
        }
        public static string ReadStringFromStream(Stream stream)
        {
            var bytes = new byte[2];
            stream.Read(bytes, 0, 2);

            if (BitConverter.IsLittleEndian)
                bytes = bytes.Reverse().ToArray();
            var length = BitConverter.ToInt16(bytes, 0);

            var stringBytes = new byte[length];
            stream.Read(stringBytes, 0, length);

            return ASCIIEncoding.ASCII.GetString(stringBytes);
        }
        public static double ReadDoubleFromStream(Stream stream)
        {
            var doubleBytes = new byte[8];
            stream.Read(doubleBytes, 0, 8);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(doubleBytes);
            return BitConverter.ToDouble(doubleBytes, 0);
        }
        public static void WriteBool(List<byte> stream, bool toWrite)
        {
            var boolToWrite = BitConverter.GetBytes(toWrite);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(boolToWrite);
            stream.AddRange(boolToWrite);
        }
        public static void WriteDouble(List<byte> stream, double toWrite)
        {
            var dblToWrite = BitConverter.GetBytes(toWrite);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(dblToWrite);
            stream.AddRange(dblToWrite);
        }
        public static void WriteShort(List<byte> stream, short toWrite)
        {
            var shortToWrite = BitConverter.GetBytes(toWrite);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(shortToWrite);
            stream.AddRange(shortToWrite);
        }
        public static void WriteInt(List<byte> stream, int toWrite)
        {
            var intToWrite = BitConverter.GetBytes(toWrite);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(intToWrite);
            stream.AddRange(intToWrite);
        }
        public static void WritePacket(List<byte> stream, Packet packet)
        {
            stream.Add(packet.typeID);
            packet.WritePacketData(stream);
        }
        public byte GetPacketType()
        {
            return typeID;
        }
    }
}
