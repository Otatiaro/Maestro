using System;
using System.Collections.Generic;
using System.Linq;

namespace Maestro.Common.Protocol
{
    public class Scanner
    {
        private readonly IEnumerable<IChannelManager> _managers;

        public Scanner(IEnumerable<IChannelManager> managers)
        {
            _managers = managers;

            foreach (var manager in _managers)
                manager.Added += Manager_Added;
        }

        private async void Manager_Added(IChannel obj)
        {
            var device = new Device(obj);

            if((await device.GetVersion()).HasValue)
                DeviceFound?.Invoke(device);
            else
                obj.Dispose();
        }

        public void Start()
        {
            foreach (var manager in _managers)
                manager.Start();
        }

        public void Stop()
        {
            foreach (var manager in _managers)
                manager.Stop();
        }

        public event Action<Device> DeviceFound;

        public IEnumerable<string> Managers => _managers.Select(m => m.Name);
    }
}
