using System;
using System.IO;
using System.Threading.Tasks;

namespace Maestro.Common.Protocol.Simulator
{
    public class SimulatorChannelManager : IChannelManager
    {
        private readonly string _directory;
        private FileSystemWatcher _watcher;

        public SimulatorChannelManager(string directory)
        {
            _directory = directory;
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }

        public void Start()
        {
            if (_watcher != null)
                Stop();

            foreach (var file in Directory.EnumerateFiles(_directory))
            {
                switch (Path.GetExtension(file))
                {
                    case ".mtrdb":
                    case ".mtrcp":
                        Added?.Invoke(new SimulatorChannel(this, file));
                        break;
                }
            }

            _watcher = new FileSystemWatcher(_directory) { Filter = "*.mtr*" };
            _watcher.Created += Watcher_Created;
            _watcher.Deleted += WatcherOnDeleted;
            _watcher.EnableRaisingEvents = true;
        }

        private void WatcherOnDeleted(object sender, FileSystemEventArgs e)
        {
            switch (Path.GetExtension(e.FullPath))
            {
                case ".mtrdb":
                case ".mtrcp":
                    Removed?.Invoke(new SimulatorChannel(this, e.FullPath));
                    break;
            }
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            switch (Path.GetExtension(e.FullPath))
            {
                case ".mtrdb":
                case ".mtrcp":
                    Added?.Invoke(new SimulatorChannel(this, e.FullPath));
                    break;
            }
        }

        public void Stop()
        {
            if (_watcher == null) return;
            _watcher.EnableRaisingEvents = false;
            _watcher.Created -= Watcher_Created;
            _watcher.Deleted -= WatcherOnDeleted;
            _watcher.Dispose();
            _watcher = null;
        }

        public event Action<IChannel> Added;
        public event Action<IChannel> Removed;
        public async Task<IChannel> GetIfAvailable(string id)
        {
            return await Task.Run(() => new SimulatorChannel(this, id));
        }

        public string Name => "Simulator";
    }
}
