using System.Windows.Controls;
using Maestro.Common.Model;

namespace Maestro.Generator.View
{
    /// <summary>
    /// Logique d'interaction pour DataBaseTabs.xaml
    /// </summary>
    public partial class DataBaseTabs : UserControl
    {
        public DataBaseTabs()
        {
            InitializeComponent();
        }

        private void AddView(object sender, System.Windows.RoutedEventArgs e)
        {
            var database = DataContext as Database;
            if (database == null)
                return;

            ModelBase.Add(database.Views, new Common.Model.Controls.View());
        }

        private void RemoveView(object sender, System.Windows.RoutedEventArgs e)
        {
            var database = DataContext as Database;
            if (database == null)
                return;

            var item = (sender as Button).DataContext as Common.Model.Controls.View;

            ModelBase.Remove(database.Views, item);

        }
    }
}
