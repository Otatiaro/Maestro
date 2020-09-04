using Maestro.Generator.Model.Controls;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;


namespace Maestro.Generator.View.Controls
{
    /// <summary>
    /// Logique d'interaction pour ControlToolBox.xaml
    /// </summary>
    public partial class ControlToolBox : UserControl
    {
        public ControlToolBox()
        {
            InitializeComponent();
        }

        private void AddText(object sender, RoutedEventArgs e)
        {
            var list = DataContext as ObservableCollection<ControlBase>;
            if (list == null)
                return;

            ModelBase.Add(list, new Model.Controls.Text());
        }

        private void AddGrid(object sender, RoutedEventArgs e)
        {
            var list = DataContext as ObservableCollection<ControlBase>;
            if (list == null)
                return;

            ModelBase.Add(list, new Model.Controls.Grid());
        }
        private void AddGroupbox(object sender, RoutedEventArgs e)
        {
            var list = DataContext as ObservableCollection<ControlBase>;
            if (list == null)
                return;

            ModelBase.Add(list, new Model.Controls.Groupbox());
        }
  

        private void AddMenu(object sender, RoutedEventArgs e)
        {
            var list = DataContext as ObservableCollection<ControlBase>;
            if (list == null)
                return;

            ModelBase.Add(list, new Model.Controls.Menu());
        }


    }
}
