using System.Collections.Generic;

namespace Maestro.Common.Protocol.Packet
{
    public class GetDescriptorPacket : PacketBase
    {
        public override Commands Command => Commands.Descriptor;
        public override IReadOnlyList<byte> Payload => new byte[0];
    }
}
