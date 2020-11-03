using System.Collections.Generic;

namespace Maestro.Common.Protocol.Packet
{
    public class ShowMessagePacket : PacketBase
    {
        public ushort MessageIndex { get; }
        public ShowMessageType MessageType { get; }

        public ShowMessagePacket(ushort messageIndex, ShowMessageType type)
        {
            MessageIndex = messageIndex;
            MessageType = type;
        }

        public ShowMessagePacket(IReadOnlyList<byte> data)
        {
            MessageIndex = (ushort)((data[0] << 8) + data[1]);
            MessageType = (ShowMessageType)data[2];
        }

        public override Commands Command => Commands.ShowMessage;
        public override IReadOnlyList<byte> Payload => new[] { (byte)(MessageIndex >> 8), (byte)(MessageIndex & 0xFF), (byte)MessageType };
    }
}