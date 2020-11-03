using System.Windows;

namespace Maestro.Generator
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            new MainWindow(e.Args.Length == 1 ? e.Args[0] : null).Show();
        }
    }
}
