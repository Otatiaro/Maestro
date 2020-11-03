using System.Collections.Generic;

namespace Maestro.Common.Protocol.Packet
{
    public class GetPackageData : PacketBase
    {
        public override Commands Command => Commands.PackageData;
        public override IReadOnlyList<byte> Payload => new byte[0];
    }
}
