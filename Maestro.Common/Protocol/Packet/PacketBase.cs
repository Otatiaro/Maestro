using System;
using System.Collections.Generic;
using System.Linq;

namespace Maestro.Common.Protocol.Packet
{
    public abstract class PacketBase
    {
        public static PacketBase Factory(Commands command, IReadOnlyList<byte> payload = null)
        {
            switch (command)
            {
                case 0:
                    return new ProtocolVersionPacket(payload);
                case Commands.Descriptor:
                    if ((payload ?? Array.Empty<byte>()).Any())
                        return new DescriptorPacket(payload);
                    else
                        return new GetDescriptorPacket();
                case Commands.PackageData:
                    return new PackageDataPacket(payload);
                case Commands.GetValues:
                    throw new NotImplementedException();
                case Commands.SetValues:
                    throw new NotImplementedException();
                case Commands.Reset:
                    return new ResetPacket();
                case Commands.CopySetup:
                    return new CopySetupPacket(payload);
                case Commands.ActiveView:
                    return new ActiveViewPacket(payload);
                case Commands.ShowMessage:
                    return new ShowMessagePacket(payload);
                case Commands.MessageResult:
                    return new MessageResultPacket(payload);
                case Commands.PluginCommand:
                    return new PluginPacket(payload);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public abstract Commands Command { get; }
        public abstract IReadOnlyList<byte> Payload { get; }

    }
}
