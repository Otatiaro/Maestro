using Maestro.Generator.Model.Parameters;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Maestro.Generator.View
{
    /// <summary>
    /// Logique d'interaction pour ParameterList.xaml
    /// </summary>
    public partial class ParameterList : UserControl
    {
        public ParameterList()
        {
            InitializeComponent();
        }

        private void AddButton(object sender, RoutedEventArgs e)
        {
            var list = DataContext as ObservableCollection<ParameterBase>;
            ModelBase.Add(list, new Model.Parameters.Button());
        }
        private void AddBoolean(object sender, RoutedEventArgs e)
        {
            var list = DataContext as ObservableCollection<ParameterBase>;
            ModelBase.Add(list, new Model.Parameters.Boolean());
        }
        private void AddString(object sender, RoutedEventArgs e)
        {
            var list = DataContext as ObservableCollection<ParameterBase>;
            ModelBase.Add(list, new Model.Parameters.String());
        }
        private void AddByteStream(object sender, RoutedEventArgs e)
        {
            var list = DataContext as ObservableCollection<ParameterBase>;
            ModelBase.Add(list, new Model.Parameters.ByteStream());
        }
        private void AddEnumeration(object sender, RoutedEventArgs e)
        {
            var list = DataContext as ObservableCollection<ParameterBase>;
            ModelBase.Add(list, new Model.Parameters.Enumeration());
        }
        private void AddInteger(object sender, RoutedEventArgs e)
        {
            var list = DataContext as ObservableCollection<ParameterBase>;
            ModelBase.Add(list, new Model.Parameters.Integer());
        }

        private void Remove(object sender, RoutedEventArgs e)
        {
            var list = DataContext as ObservableCollection<ParameterBase>;
            var item = (sender as System.Windows.Controls.Button).DataContext as ParameterBase;

            ModelBase.Remove(list, item);
        }

        private void Duplicate(object sender, RoutedEventArgs e)
        {
            var list = DataContext as ObservableCollection<ParameterBase>;
            var item = (sender as System.Windows.Controls.Button).DataContext as ParameterBase;

            switch(item)
            {
                case Model.Parameters.Button button:
                    ModelBase.Add(list, new Model.Parameters.Button(button));
                    break;
                case Model.Parameters.String str:
                    ModelBase.Add(list, new Model.Parameters.String(str));
                    break;
                case Model.Parameters.Boolean boolean:
                    ModelBase.Add(list, new Model.Parameters.Boolean(boolean));
                    break;
                case Model.Parameters.ByteStream stream:
                    ModelBase.Add(list, new Model.Parameters.ByteStream(stream));
                    break;
                case Model.Parameters.Integer integer:
                    ModelBase.Add(list, new Model.Parameters.Integer(integer));
                    break;
                case Model.Parameters.Enumeration enumeration:
                    ModelBase.Add(list, new Model.Parameters.Enumeration(enumeration));
                    break;
            }
        }
    }
}
