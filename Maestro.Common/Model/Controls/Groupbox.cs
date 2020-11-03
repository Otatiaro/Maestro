using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Maestro.Common.Model.Controls
{
    public class Groupbox : ControlBase
    {
        private string header;

        public ObservableCollection<ControlBase> Children { get; } = new ObservableCollection<ControlBase>();
        public string Header { get => header; set => Set(ref header, value); }

        public override IEnumerable<string> TranslatableResources() => new[] { Header };
    }
}
