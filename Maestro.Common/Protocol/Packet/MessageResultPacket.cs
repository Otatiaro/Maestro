using System.Collections.Generic;

namespace Maestro.Common.Protocol.Packet
{
    public class MessageResultPacket : PacketBase
    {
        public ushort MessageIndex { get; }
        public ShowMessageResult Result { get; }

        public MessageResultPacket(ushort messageIndex, ShowMessageResult result)
        {
            MessageIndex = messageIndex;
            Result = result;
        }

        public MessageResultPacket(IReadOnlyList<byte> data)
        {
            MessageIndex = (ushort)((data[0] << 8) + data[1]);
            Result = (ShowMessageResult)(data[2]);
        }

        public override Commands Command => Commands.ShowMessage;
        public override IReadOnlyList<byte> Payload => new[] { (byte)(MessageIndex >> 8), (byte)(MessageIndex & 0xFF), (byte)Result };
    }
}