using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Maestro.Common.Model;
using Maestro.Common.Model.Parameters;
using Button = Maestro.Common.Model.Parameters.Button;

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
            ModelBase.Add(list, new Button());
        }
        private void AddBoolean(object sender, RoutedEventArgs e)
        {
            var list = DataContext as ObservableCollection<ParameterBase>;
            ModelBase.Add(list, new Boolean());
        }
        private void AddString(object sender, RoutedEventArgs e)
        {
            var list = DataContext as ObservableCollection<ParameterBase>;
            ModelBase.Add(list, new String());
        }
        private void AddByteStream(object sender, RoutedEventArgs e)
        {
            var list = DataContext as ObservableCollection<ParameterBase>;
            ModelBase.Add(list, new ByteStream());
        }
        private void AddEnumeration(object sender, RoutedEventArgs e)
        {
            var list = DataContext as ObservableCollection<ParameterBase>;
            ModelBase.Add(list, new Enumeration());
        }
        private void AddInteger(object sender, RoutedEventArgs e)
        {
            var list = DataContext as ObservableCollection<ParameterBase>;
            ModelBase.Add(list, new Integer());
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
                case Button button:
                    ModelBase.Add(list, new Button(button));
                    break;
                case String str:
                    ModelBase.Add(list, new String(str));
                    break;
                case Boolean boolean:
                    ModelBase.Add(list, new Boolean(boolean));
                    break;
                case ByteStream stream:
                    ModelBase.Add(list, new ByteStream(stream));
                    break;
                case Integer integer:
                    ModelBase.Add(list, new Integer(integer));
                    break;
                case Enumeration enumeration:
                    ModelBase.Add(list, new Enumeration(enumeration));
                    break;
            }
        }
    }
}
