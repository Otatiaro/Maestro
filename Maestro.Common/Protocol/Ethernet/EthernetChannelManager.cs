using System;
using System.Threading.Tasks;

namespace Maestro.Common.Protocol.Ethernet
{
    public class EthernetChannelManager  : IChannelManager
    {
        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public event Action<IChannel> Added;
        public event Action<IChannel> Removed;
        public Task<IChannel> GetIfAvailable(string id)
        {
            throw new NotImplementedException();
        }

        public string Name { get; }
    }
}
