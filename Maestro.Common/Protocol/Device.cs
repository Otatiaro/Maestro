using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Maestro.Common.Model;
using Maestro.Common.Protocol.Packet;

namespace Maestro.Common.Protocol
{
    public class Device
    {
        public const byte ProtocolVersion = 4;

        public IChannel Channel { get; }

        public Device(IChannel channel)
        {
            Channel = channel;
        }

        public async Task<byte?> GetVersion()
        {
            var source = new TaskCompletionSource<ProtocolVersionPacket>();

            void Lambda(PacketBase packet)
            {
                if (packet is ProtocolVersionPacket versionPacket) source.SetResult(versionPacket);
            }

            Channel.OnDataReceived += Lambda;
            await Channel.Send(new ProtocolVersionPacket(ProtocolVersion));
            await Task.WhenAny(Task.Delay(Channel.Timeout), source.Task);
            Channel.OnDataReceived -= Lambda;

            if (source.Task.IsCompleted)
                return source.Task.Result.Version;

            source.TrySetCanceled();
            return null;
        }

        public async Task<Descriptor> GetDescriptor()
        {
            var source = new TaskCompletionSource<DescriptorPacket>();

            void Lambda(PacketBase packet)
            {
                if (packet is DescriptorPacket descriptorPacket) source.SetResult(descriptorPacket);
            }

            Channel.OnDataReceived += Lambda;
            await Channel.Send(new GetDescriptorPacket());
            await Task.WhenAny(Task.Delay(Channel.Timeout), source.Task);
            Channel.OnDataReceived -= Lambda;

            if (source.Task.IsCompleted)
                return source.Task.Result.ToDescriptor();

            source.TrySetCanceled();
            return null;
        }

        public event Action<ushort, ushort> PackageReceived;

        public async Task<Database> GetPackage()
        {
            var source = new TaskCompletionSource<Database>();
            PackageReceived?.Invoke(0, 0);

            var fragments = new List<PackageDataPacket>();

            void Lambda(PacketBase packet)
            {
                if (!(packet is PackageDataPacket packageData)) return;
                fragments.Add(packageData);
                PackageReceived?.Invoke(packageData.PacketIndex, packageData.PacketCount);
                if (PackageDataPacket.IsPackageComplete(fragments))
                    source.SetResult(PackageDataPacket.ExtractPackage(fragments));
            }

            Channel.OnDataReceived += Lambda;
            await Channel.Send(new GetPackageData());

            await Task.WhenAny(Task.Delay(Channel.Timeout), source.Task);

            if (!source.Task.IsCompleted && fragments.Count != 0) // not completed but received at least a message, adjust timeout
                await Task.WhenAny(Task.Delay(new TimeSpan(Channel.Timeout.Ticks * (fragments.First().PacketCount - fragments.Count))), source.Task);

            Channel.OnDataReceived -= Lambda;

            if (source.Task.IsCompleted)
                return source.Task.Result;

            source.TrySetCanceled();
            return null;
        }
    }
}
