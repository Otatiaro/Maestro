using System.Collections.Generic;

namespace Maestro.Common.Protocol.Packet
{
    public class ResetPacket : PacketBase
    {
        public override Commands Command => Commands.Reset;
        public override IReadOnlyList<byte> Payload => new byte[0];
    }
}