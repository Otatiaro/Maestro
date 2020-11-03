using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Maestro.Common.Protocol.Packet;

namespace Maestro.Common.Protocol
{
    public class PacketStream
    {
        public int SizeBytes { get; }
        public byte StartByte { get; }
        public byte EndByte { get; }
        public byte EscapeByte { get; }

        public PacketStream(int sizeBytes = 4, byte startByte = 0x55, byte endByte = 0x3C, byte escapeByte = 0xC3)
        {
            SizeBytes = sizeBytes;
            StartByte = startByte;
            EndByte = endByte;
            EscapeByte = escapeByte;
        }

        public IEnumerable<byte> Serialize(PacketBase packet)
        {

            yield return StartByte;
            yield return StartByte;

            if (IsSpecial((byte)packet.Command))
                yield return EscapeByte;
            yield return (byte)packet.Command;

            for (var i = 0; i < SizeBytes; ++i)
            {
                var b = (byte)(packet.Payload.Count >> (8 * (SizeBytes - 1 - i)));
                if (IsSpecial(b))
                    yield return EscapeByte;
                yield return b;
            }

            foreach (var b in packet.Payload)
            {
                if (IsSpecial(b))
                    yield return EscapeByte;
                yield return b;
            }

            var chk = Checksum(packet);

            if (IsSpecial(chk))
                yield return EscapeByte;
            yield return chk;

            yield return EndByte;
        }

        private enum WaitState
        {
            Start, SecondStart, Command, Size, Payload, Checksum, End,
        };

        private WaitState _waitState = WaitState.Start;
        private bool _escaping = false;
        private byte _lastCommand;
        private uint _size;
        private uint _index;
        private byte[] _data;
        private PacketBase _packet;

        public void Feed(byte b)
        {
            if (b == EscapeByte && !_escaping && (_waitState != WaitState.Start && _waitState != WaitState.SecondStart && _waitState != WaitState.End))
            {
                _escaping = true;
                return;
            }
            _escaping = false;

            switch (_waitState)
            {
                case WaitState.Start:
                    if (b == StartByte)
                        _waitState = WaitState.SecondStart;
                    break;
                case WaitState.SecondStart:
                    _waitState = b == StartByte ? WaitState.Command : WaitState.Start;
                    break;
                case WaitState.Command:
                    _lastCommand = b;
                    _index = 0;
                    _size = 0;
                    _waitState = WaitState.Size;
                    break;
                case WaitState.Size:
                    _size = (_size << 8) | b;
                    if (++_index == SizeBytes)
                    {
                        if (_size == 0)
                        {
                            _packet = PacketBase.Factory((Commands)_lastCommand);
                            _waitState = WaitState.Checksum;
                        }
                        else
                        {
                            _data = new byte[_size];
                            _index = 0;
                            _waitState = WaitState.Payload;
                        }
                    }
                    break;
                case WaitState.Payload:
                    _data[_index++] = b;
                    if (_index == _size)
                    {
                        _waitState = WaitState.Checksum;
                        _packet = PacketBase.Factory((Commands)_lastCommand, _data);
                    }
                    break;
                case WaitState.Checksum:
                    _waitState = b == Checksum(_packet) ? WaitState.End : WaitState.Start;
                    break;
                case WaitState.End:
                    if (b == EndByte)
                        OnPacketReceived?.Invoke(_packet);
                    _waitState = WaitState.Start;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public event Action<PacketBase> OnPacketReceived;

        private bool IsSpecial(byte b)
        {
            return b == StartByte || b == EndByte || b == EscapeByte;
        }

        private int RawSize(PacketBase packet)
        {
            var total = 5 + SizeBytes + packet.Payload.Count + (IsSpecial((byte)packet.Command) ? 1 : 0);

            var tmp = packet.Payload.Count;

            for (var i = 0; i < SizeBytes; ++i)
            {
                total += IsSpecial((byte)(tmp & 0xFF)) ? 1 : 0;
                tmp >>= 8;
            }

            total += packet.Payload.Count(IsSpecial);

            return total;
        }

        private byte Checksum(PacketBase packet)
        {
            var result = (byte)packet.Command;

            for (var i = 0; i < SizeBytes; ++i)
                result = (byte)(result + (packet.Payload.Count >> (8 * i)));

            result = packet.Payload.Aggregate(result, (current, b) => (byte)(current + b));

            return (byte)(~result + 1);
        }
    }

}
