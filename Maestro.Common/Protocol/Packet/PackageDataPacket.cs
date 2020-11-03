using System.Collections.Generic;
using System.Linq;
using Maestro.Common.Model;

namespace Maestro.Common.Protocol.Packet
{
    public class PackageDataPacket : PacketBase
    {
        public IReadOnlyList<byte> PackageFragment { get; }
        public ushort PacketCount { get; }
        public ushort PacketIndex { get; }

        public PackageDataPacket(IReadOnlyList<byte> data)
        {
            PacketCount = (ushort)((data[0] << 8) + data[1]);
            PacketIndex = (ushort)((data[2] << 8) + data[3]);
            PackageFragment = new List<byte>(data.Skip(4));
        }

        public PackageDataPacket(ushort packetCount, ushort packetIndex, IReadOnlyList<byte> packageFragment)
        {
            PacketCount = packetCount;
            PacketIndex = packetIndex;
            PackageFragment = packageFragment;
        }

        public override Commands Command => Commands.PackageData;
        public override IReadOnlyList<byte> Payload => PayloadEnumeration.ToList();

        private IEnumerable<byte> PayloadEnumeration
        {
            get
            {
                yield return (byte)(PacketCount >> 8);
                yield return (byte)(PacketCount & 0xFF);
                yield return (byte)(PacketIndex >> 8);
                yield return (byte)(PacketIndex & 0xFF);

                foreach (var b in PackageFragment)
                    yield return b;
            }
        }

        public static bool IsPackageComplete(IReadOnlyList<PackageDataPacket> fragments)
        {
            if (fragments.Count == 0)
                return false;

            var count = fragments[0].PacketCount;

            var same = fragments.Where(f => f.PacketCount == count).OrderBy(f => f.PacketIndex).ToArray();

            if (same.Length != count)
                return false;

            return !same.Where((t, i) => t.PacketIndex != i).Any();
        }

        public static Database ExtractPackage(IReadOnlyList<PackageDataPacket> fragments)
        {
            var count = fragments[0].PacketCount;
            var same = fragments.Where(f => f.PacketCount == count).OrderBy(f => f.PacketIndex).ToArray();

            var buffer = new List<byte>();

            foreach (var dataPacket in same)
                buffer.AddRange(dataPacket.Payload);

            return Database.LoadXml(buffer.ToArray());
        }
    }
}