using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using Maestro.Common.Model;
using Maestro.Common.Protocol.Packet;

namespace Maestro.Common.Protocol.Simulator
{
    public class SimulatorChannel : IChannel
    {
        private readonly Database _database;

        public SimulatorChannel(IChannelManager manager, string file)
        {
            Address = file;
            Manager = manager;
            Manager.Removed += Manager_Removed;

            var data = File.ReadAllBytes(file);
            if (Path.GetExtension(file) == ".mtrcp")
                data = data.Decompress();

            _database = Database.LoadXml(data);
        }

        private void Manager_Removed(IChannel obj)
        {
            if(Equals(obj))
                OnDisconnected?.Invoke();
        }

        public bool Equals(IChannel other)
        {
            return other is SimulatorChannel simulator && simulator.Address == Address;
        }

        public void Dispose()
        {
            Manager.Removed -= Manager_Removed;
        }

        public async Task Send(PacketBase data)
        {
            await Task.Run(() =>
            {
                switch (data)
                {
                    case ProtocolVersionPacket protocol:
                        OnDataReceived?.Invoke(new ProtocolVersionPacket(4));
                        break;
                    case GetDescriptorPacket descriptor:
                        OnDataReceived?.Invoke(new DescriptorPacket(_database.Descriptor));
                        break;
                }
            });
        }

        public event Action<PacketBase> OnDataReceived;
        public event Action OnDisconnected;
        public string Name => Path.GetFileName(Address);
        public string Address { get; }
        public IChannelManager Manager { get; }
        public TimeSpan Timeout => new TimeSpan(0, 0, 0, 1);
        
    }
}
