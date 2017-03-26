using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace SmackBrosMatchmakingServer
{
    class Program
    {
        static bool serverInitialized = false;
        static int port1 = 1521;
        static int port2 = 1522;
        static int port3 = 1523;
        static UdpClient client;
        static UdpClient client2;
        static UdpClient client3;
        static Thread ReceivingThread;
        static Thread MatchFinderThread;
        static string ServerIP;
        static List<string> clientIPList = new List<string>();
        static readonly object packetProcessQueueLock = new object();
        static Queue<Packet> packetProcessQueue = new Queue<Packet>();
        static DateTime lastUpdateServerThread = DateTime.Now;
        static DateTime lastUpdateMatchThread = DateTime.Now;
        static TheQueue mmQueue = new TheQueue();

        static void Main(string[] args)
        {
            if (!serverInitialized)
                StartServer();
            while(serverInitialized)
            {
                if (DateTime.Now - lastUpdateServerThread > TimeSpan.FromMilliseconds(100))
                {
                    lock (packetProcessQueueLock)
                        while (packetProcessQueue.Any())
                        {
                            var packet = packetProcessQueue.Dequeue();
                            if (packet.GetPacketType() == 1)
                            {
                                var packet2 = (QueueInteractionPacket)packet;
                                if (packet2.joining)
                                {
                                    AddPlayerToQueue(packet2);
                                    new Task(() =>
                                    {
                                        clientIPList.Add(packet2.IPAddress);
                                        PacketQueue.Instance.AddPacket(new QueueStatusUpdatePacket { Accepted = true });
                                    }).Start();
                                }
                            }
                            if (packet.GetPacketType() == 2)
                            {

                            }
                            if (packet.GetPacketType() == 3)
                            {

                            }
                        }
                }         
            }
        }
        static void AddPlayerToQueue(QueueInteractionPacket packet)
        {
            StoredPlayer playerToAdd = new StoredPlayer();
            {
                playerToAdd.name = packet.name; 
                playerToAdd.playerIP = packet.IPAddress;
                playerToAdd.mmrTolerance = 25;
                playerToAdd.mmr = packet.mmr;
                playerToAdd.searchedThisIteration = false;
                playerToAdd.priority = 0;
            }
            mmQueue.Enqueue(playerToAdd);
        }
        static void FindMatches()
        {
            mmQueue.Match();
        }
        static void StartServer()
        {
            new Task(() =>
            {
                client = client ?? new UdpClient(port1, AddressFamily.InterNetwork);
                client2 = client2 ?? new UdpClient(port2, AddressFamily.InterNetwork);
                client3 = client3 ?? new UdpClient(port3, AddressFamily.InterNetwork);

                IPHostEntry host;
                string localIP = "?";
                host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIP = ip.ToString();
                    }
                }
                ServerIP = localIP;
                ReceivingThread = new Thread(() => PacketQueue.Instance.ReceivingLoop(client2, new IPEndPoint(IPAddress.Any, port2), packetProcessQueue, packetProcessQueueLock));
                ReceivingThread.IsBackground = true;
                ReceivingThread.Start();
            }).Start();
            new Task(() =>
            {

                MatchFinderThread = new Thread(() => FindMatches());
                MatchFinderThread.IsBackground = true;
                MatchFinderThread.Start();
            }).Start();
            serverInitialized = true;
        }
    }
}

