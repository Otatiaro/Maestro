using System.Collections.Generic;

namespace Maestro.Common.Protocol.Packet
{
    public class CopySetupPacket : PacketBase
    {
        public byte From { get; }
        public byte To { get; }

        public CopySetupPacket(byte from, byte to)
        {
            From = @from;
            To = to;
        }

        public CopySetupPacket(IReadOnlyList<byte> data)
        {
            From = data[0];
            To = data[1];
        }

        public override Commands Command => Commands.CopySetup;

        public override IReadOnlyList<byte> Payload => new[] { From, To };
    }
}