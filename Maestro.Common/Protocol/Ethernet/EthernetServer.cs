using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Maestro.Common.Model;

namespace Maestro.Common.Protocol.Ethernet
{
    public class EthernetServer : IDisposable
    {
        private const ushort DefaultPort = 60769;
        private readonly TimeSpan _beaconTimeSpan = new TimeSpan(0, 0, 0, 5);
        private readonly Database _database;
        private readonly ushort _tcpPort;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly Task _beaconTask;
        private readonly Task _tcpServer;

        public EthernetServer(Database database, ushort tcpPort = DefaultPort)
        {
            _database = database;
            _tcpPort = tcpPort;

            _tcpServer = Task.Run(TcpServer, _cancellationTokenSource.Token);
            _beaconTask = Task.Run(UdpBeacon, _cancellationTokenSource.Token);
        }


        private async Task UdpBeacon()
        {
            var port = new[] { (byte)(_tcpPort >> 8), (byte)(_tcpPort & 0xFF) };
            var hello = Encoding.ASCII.GetBytes("Hello Maestro!");
            var helloPacket = port.Concat(hello).ToArray();

            using (var udpClient = new UdpClient(DefaultPort) { EnableBroadcast = true })
            {
                try
                {

                    while (!_cancellationTokenSource.IsCancellationRequested)
                    {
                        await Task.Run(async () => await udpClient.SendAsync(helloPacket, helloPacket.Length),
                            _cancellationTokenSource.Token);
                        await Task.Delay(_beaconTimeSpan);
                    }

                    var goodbye = Encoding.ASCII.GetBytes("Goodbye Maestro!");
                    var goodbyePacket = port.Concat(goodbye).ToArray();
                    await udpClient.SendAsync(goodbyePacket, goodbyePacket.Length);
                }
                catch (OperationCanceledException)
                {
                    // ignore
                }
            }

        }

        private async Task TcpServer()
        {
            var listener = new TcpListener(IPAddress.Any, _tcpPort);
            // TODO : configure tcp listener
            listener.Start();

            var client = await Task.Run(async () => await listener.AcceptTcpClientAsync(), _cancellationTokenSource.Token);

        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();

            using (var udpClient = new UdpClient(DefaultPort) { EnableBroadcast = true })
            {
                var port = new[] { (byte)(_tcpPort >> 8), (byte)(_tcpPort & 0xFF) };
                var goodbye = Encoding.ASCII.GetBytes("Goodbye Maestro!");
                var goodbyePacket = port.Concat(goodbye).ToArray();
                udpClient.Send(goodbyePacket, goodbyePacket.Length);
            }

            _beaconTask.Wait();
            _tcpServer.Wait();
            _cancellationTokenSource.Dispose();
        }
    }
}
