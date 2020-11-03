using System;
using System.Threading.Tasks;

namespace Maestro.Common.Protocol
{
    public interface IChannelManager
    {
        void Start();
        void Stop();

        event Action<IChannel> Added;
        event Action<IChannel> Removed;

        Task<IChannel> GetIfAvailable(string id);

        string Name { get; }
    }
}
