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
        static int port1 = 1521;
        static int port2 = 1522;
        static int port3 = 1523;
        static UdpClient client;
        static UdpClient client2;
        static UdpClient client3;
        static Thread ReceivingThread;
        static string ServerIP;
        static string ClientIP;
        static readonly object packetProcessQueueLock = new object();
        static Queue<Packet> packetProcessQueue = new Queue<Packet>();
        static void Main(string[] args)
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
                ReceivingThread = new Thread(() => PacketQueue.Instance.TestLoop(client2, new IPEndPoint(IPAddress.Any, port2), packetProcessQueue, packetProcessQueueLock));
                ReceivingThread.IsBackground = true;
                ReceivingThread.Start();
            }).Start();
        }

    }
}

