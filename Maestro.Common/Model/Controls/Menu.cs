using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Maestro.Common.Model.Controls
{
    public class Menu : ControlBase
    {
        public ObservableCollection<View> Views { get; } = new ObservableCollection<View>();

        public override IEnumerable<string> TranslatableResources() => Enumerable.Empty<string>();
    }
}
