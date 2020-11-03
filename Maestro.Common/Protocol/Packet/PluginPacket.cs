using System.Collections.Generic;
using System.Linq;

namespace Maestro.Common.Protocol.Packet
{
    public class PluginPacket : PacketBase
    {
        public byte PluginId { get; }
        public IReadOnlyList<byte> PluginData { get; }

        public PluginPacket(IReadOnlyList<byte> data)
        {
            PluginId = data[0];
            PluginData = new List<byte>(data.Skip(1));
        }

        public PluginPacket(byte pluginId, IEnumerable<byte> pluginData)
        {
            PluginId = pluginId;
            PluginData = new List<byte>(pluginData);
        }

        public override Commands Command => Commands.PluginCommand;
        public override IReadOnlyList<byte> Payload => new[] { PluginId }.Concat(PluginData).ToList();
    }
}