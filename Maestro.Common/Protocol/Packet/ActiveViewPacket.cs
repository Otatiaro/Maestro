using System.Collections.Generic;

namespace Maestro.Common.Protocol.Packet
{
    public class ActiveViewPacket : PacketBase
    {
        public ushort ViewIndex { get; }

        public ActiveViewPacket(IReadOnlyList<byte> data)
        {
            ViewIndex = (ushort)((data[0] << 8) + data[1]);
        }

        public ActiveViewPacket(ushort viewIndex)
        {
            ViewIndex = viewIndex;
        }

        public override Commands Command => Commands.ActiveView;
        public override IReadOnlyList<byte> Payload => new[] { (byte)(ViewIndex >> 8), (byte)(ViewIndex & 0xFF) };
    }
}