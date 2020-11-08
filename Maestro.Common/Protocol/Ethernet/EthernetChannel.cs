using System;
using System.Threading.Tasks;
using Maestro.Common.Protocol.Packet;

namespace Maestro.Common.Protocol.Ethernet
{
    public class EthernetChannel : IChannel
    {
        public bool Equals(IChannel other)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task Send(PacketBase data)
        {
            throw new NotImplementedException();
        }

        public event Action<PacketBase> OnDataReceived;
        public event Action OnDisconnected;
        public string Name { get; }
        public string Address { get; }
        public IChannelManager Manager { get; }
        public TimeSpan Timeout { get; }
    }
}
