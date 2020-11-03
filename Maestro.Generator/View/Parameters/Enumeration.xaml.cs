using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Maestro.Common.Model;

namespace Maestro.Generator.View.Parameters
{
    /// <summary>
    /// Logique d'interaction pour Enumeration.xaml
    /// </summary>
    public partial class Enumeration : UserControl
    {
        public Enumeration()
        {
            InitializeComponent();
        }

        private void AddValue(object sender, RoutedEventArgs e)
        {
            var enumeration = DataContext as Common.Model.Parameters.Enumeration;

            if (enumeration == null)
                return;

            ModelBase.Add(enumeration.Values, new Common.Model.Parameters.Enumeration.EnumerationValue());

            if (DefaultBox.SelectedIndex == -1)
                DefaultBox.SelectedIndex = 0;
        }

        private void Remove(object sender, RoutedEventArgs e)
        {
            var list = (DataContext as Common.Model.Parameters.Enumeration)?.Values;
            var item = (sender as System.Windows.Controls.Button).DataContext as Common.Model.Parameters.Enumeration.EnumerationValue;

            ModelBase.Remove(list, item);
        }
    }
}
