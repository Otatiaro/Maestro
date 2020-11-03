using System;
using System.Threading.Tasks;
using Maestro.Common.Protocol.Packet;

namespace Maestro.Common.Protocol
{
    public interface IChannel : IEquatable<IChannel>, IDisposable
    {
        Task Send(PacketBase data);
        event Action<PacketBase> OnDataReceived;
        event Action OnDisconnected;
        string Name { get; }
        string Address { get; }
        IChannelManager Manager { get; }

        TimeSpan Timeout { get; }
    }
}
