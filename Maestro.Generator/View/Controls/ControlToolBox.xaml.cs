using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Maestro.Common.Model;
using Maestro.Common.Model.Controls;
using Grid = Maestro.Common.Model.Controls.Grid;
using Menu = Maestro.Common.Model.Controls.Menu;


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

            ModelBase.Add(list, new Text());
        }

        private void AddGrid(object sender, RoutedEventArgs e)
        {
            var list = DataContext as ObservableCollection<ControlBase>;
            if (list == null)
                return;

            ModelBase.Add(list, new Grid());
        }
        private void AddGroupbox(object sender, RoutedEventArgs e)
        {
            var list = DataContext as ObservableCollection<ControlBase>;
            if (list == null)
                return;

            ModelBase.Add(list, new Groupbox());
        }
  

        private void AddMenu(object sender, RoutedEventArgs e)
        {
            var list = DataContext as ObservableCollection<ControlBase>;
            if (list == null)
                return;

            ModelBase.Add(list, new Menu());
        }


    }
}
