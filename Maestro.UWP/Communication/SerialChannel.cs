using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;
using Maestro.Common.Protocol;
using Maestro.Common.Protocol.Packet;

namespace Maestro.UWP.Communication
{
    public class SerialChannel : IChannel
    {
        public const int DefaultBaudRate = 115200;
        public const SerialParity DefaultParity = SerialParity.None;
        public const ushort DefaultBits = 8;
        public const SerialStopBitCount DefaultStopBitCount = SerialStopBitCount.One;
        public const uint MaxReceive = 500;

        private readonly PacketStream _packetStream = new PacketStream();

        private SerialDevice _device;
        private readonly CancellationTokenSource _cancellationToken = new CancellationTokenSource();

        public SerialChannel(string id, string name, IChannelManager manager)
        {
            Manager = manager;
            Address = id;
            Name = name;

            Manager.Removed += ManagerOnRemoved;
            _packetStream.OnPacketReceived += packet => OnDataReceived?.Invoke(packet);
        }

        private void ManagerOnRemoved(IChannel obj)
        {
            if (!Equals(obj)) return;
            OnDisconnected?.Invoke();
            Close();
        }

        public bool Equals(IChannel other)
        {
            return other?.Address == Address;
        }

        public async Task Send(PacketBase packet)
        {
            if (!IsOpen && !await Open())
            {
                OnDisconnected?.Invoke();
                Close();
            }

            using (var dw = new DataWriter(_device.OutputStream))
            {
                dw.WriteBytes(_packetStream.Serialize(packet).ToArray());
            }
        }

        public event Action<PacketBase> OnDataReceived;
        public event Action OnDisconnected;
        public void Close()
        {
            if(!_cancellationToken.IsCancellationRequested)
                _cancellationToken.Cancel();
            Manager.Removed -= ManagerOnRemoved;
            if (!IsOpen) return;
            //_device.Dispose();
            //_device = null;
        }

        public async Task<bool> Open()
        {
            if (IsOpen)
                return true;

            _device = await SerialDevice.FromIdAsync(Address);


            if (_device == null)
                return false;

            _device.BaudRate = DefaultBaudRate;
            _device.Parity = DefaultParity;
            _device.DataBits = DefaultBits;
            _device.StopBits = DefaultStopBitCount;

#pragma warning disable 4014
            Task.Run(async () =>
            {
                try
                {
                    while (!_cancellationToken.IsCancellationRequested)
                    {
                            using (var reader = new DataReader(_device.InputStream)
                                {InputStreamOptions = InputStreamOptions.Partial})
                            {
                                var count = await reader.LoadAsync(MaxReceive).AsTask(_cancellationToken.Token);
                                var bytes = new byte[count];
                                reader.ReadBytes(bytes);

                                foreach (var b in bytes)
                                    _packetStream.Feed(b);

                                reader.DetachStream();
                            }
                    }
                }
                catch (Exception)
                {
                    OnDisconnected?.Invoke();
                }

            }, _cancellationToken.Token);
#pragma warning restore 4014

            return true;
        }

        public bool IsOpen => _device != null;

        public string Name { get; }
        public string Address { get; }
        public IChannelManager Manager { get; }
        public TimeSpan Timeout => new TimeSpan(0, 0, 0, 2);

        public void Dispose()
        {
            Close();
        }
    }
}
