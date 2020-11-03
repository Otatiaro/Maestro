using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Maestro.Common.Protocol;

namespace Maestro.UWP.Communication
{
    public class SerialManager : IChannelManager
    {
        private DeviceWatcher _watcher;
        public void Start()
        {
            if (_watcher == null)
            {
                _watcher = DeviceInformation.CreateWatcher(SerialDevice.GetDeviceSelector());
                _watcher.Added += WatcherOnAdded;
                _watcher.Removed += WatcherOnRemoved;
            }

            _watcher.Start();
        }

        private void WatcherOnRemoved(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            Removed?.Invoke(new SerialChannel(args.Id, null, this));
        }

        private void WatcherOnAdded(DeviceWatcher sender, DeviceInformation args)
        {
            Added?.Invoke(new SerialChannel(args.Id, args.Name, this));
        }

        public void Stop()
        {
            _watcher.Stop();
            _watcher.Added -= WatcherOnAdded;
            _watcher.Removed -= WatcherOnRemoved;
            _watcher = null;
        }

        public event Action<IChannel> Added;
        public event Action<IChannel> Removed;
        public async Task<IChannel> GetIfAvailable(string id)
        {
            var info = await DeviceInformation.CreateFromIdAsync(id);
            return info == null ? null : new SerialChannel(id, info.Name, this);
        }

        public string Name => "USB Serial";
    }
}
