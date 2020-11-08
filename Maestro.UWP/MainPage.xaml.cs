using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Maestro.Common.Protocol;
using Maestro.Common.Protocol.Simulator;
using Maestro.UWP.Communication;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Maestro.UWP
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly Scanner _scanner = new Scanner(new IChannelManager[] { /*new SerialManager(),*/ new SimulatorChannelManager(Environment.CurrentDirectory) });

        public ObservableCollection<Device> Devices { get; } = new ObservableCollection<Device>();

        public MainPage()
        {
            this.DataContext = this;
            this.InitializeComponent();

            _scanner.DeviceFound += DeviceFound;

            _scanner.Start();
        }

        private async void DeviceFound(Device obj)
        {
            obj.Channel.OnDisconnected += async () => await Dispatcher.RunAsync(CoreDispatcherPriority.High, () => Devices.Remove(obj));
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Devices.Add(obj));
        }

        public IEnumerable<string> Managers => _scanner.Managers;

    }
}
