using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maestro.Common.Model;

namespace Maestro.Common.Protocol.Packet
{
    public class DescriptorPacket : PacketBase
    {
        public ushort Major { get; }
        public ushort Minor { get; }
        public ushort Build { get; }
        public ushort Hardware { get; }
        public string Product { get; }
        public string Firmware { get; }
        public string Name { get; }


        public Descriptor ToDescriptor()
        {
            return new Descriptor{Firmware = Firmware, Name = Name, HardwareRevision = Hardware, Product = Product, VersionBuild = Build, VersionMajor = Major, VersionMinor = Minor};
        }

        public DescriptorPacket(IReadOnlyList<byte> data)
        {
            var i = 0;
            Major = data[i++];
            Major = (ushort)((Major << 8) + data[i++]);
            Minor = data[i++];
            Minor = (ushort)((Minor << 8) + data[i++]);
            Build = data[i++];
            Build = (ushort)((Build << 8) + data[i++]);
            Hardware = data[i++];
            Hardware = (ushort)((Hardware << 8) + data[i++]);

            var buffer = new List<byte>();

            while (data[i] != 0 && i < data.Count)
                buffer.Add(data[i++]);

            Product = Encoding.UTF8.GetString(buffer.ToArray());
            buffer.Clear();

            while (data[i] != 0 && i < data.Count)
                buffer.Add(data[i++]);

            Firmware = Encoding.UTF8.GetString(buffer.ToArray());
            buffer.Clear();

            while (data[i] != 0 && i < data.Count)
                buffer.Add(data[i++]);

            Name = Encoding.UTF8.GetString(buffer.ToArray());
        }

        public DescriptorPacket(byte major, byte minor, byte build, byte hardware, string product, string firmware,
            string name)
        {
            Major = major;
            Minor = minor;
            Build = build;
            Hardware = hardware;
            Product = product;
            Firmware = firmware;
            Name = name;
        }

        public DescriptorPacket(Descriptor descriptor)
        {
            Major = descriptor.VersionMajor;
            Minor = descriptor.VersionMinor;
            Build = descriptor.VersionBuild;
            Hardware = descriptor.HardwareRevision;
            Product = descriptor.Product;
            Firmware = descriptor.Firmware;
            Name = descriptor.Name;
        }

        public override Commands Command => Commands.Descriptor;
        public override IReadOnlyList<byte> Payload => PayloadEnumeration.ToList();

        private IEnumerable<byte> PayloadEnumeration
        {
            get
            {
                yield return (byte)(Major >> 8);
                yield return (byte)(Major & 0xFF);
                yield return (byte)(Minor >> 8);
                yield return (byte)(Minor & 0xFF);
                yield return (byte)(Build >> 8);
                yield return (byte)(Build & 0xFF);
                yield return (byte)(Hardware >> 8);
                yield return (byte)(Hardware & 0xFF);

                foreach (var b in Encoding.UTF8.GetBytes(Product))
                    yield return b;
                foreach (var b in Encoding.UTF8.GetBytes(Firmware))
                    yield return b;
                foreach (var b in Encoding.UTF8.GetBytes(Name))
                    yield return b;
            }
        }
    }
}