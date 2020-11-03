using System.Collections.Generic;

namespace Maestro.Common.Protocol.Packet
{
    public class ProtocolVersionPacket : PacketBase
    {
        public byte Version { get; }

        public ProtocolVersionPacket(byte version)
        {
            Version = version;
        }

        public ProtocolVersionPacket(IReadOnlyList<byte> data)
        {
            Version = data[0];
        }

        public override Commands Command => Commands.ProtocolVersion;
        public override IReadOnlyList<byte> Payload => new[] { Version };
    }
}