using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Maestro.Common.Model.Controls
{
    public class View : ControlBase
    {
        private string icon;
        private string tooltip;
        private string name = "View name";
        private string tag = "view_name";

        public string Icon { get => icon; set => Set(ref icon, value); }
        public string Tooltip { get => tooltip; set => Set(ref tooltip, value); }
        public string Name { get => name; set => Set(ref name, value); }
        public string Tag { get => tag; set => Set(ref tag, value); }

        public ObservableCollection<ControlBase> Children { get; } = new ObservableCollection<ControlBase>();

        protected override IEnumerable<(string, object)> Errors()
        {
            foreach (var e in base.Errors())
                yield return e;
        }

        public override string ToString()
        {
            return $"{Name} ({Tag})";
        }

        public override IEnumerable<string> TranslatableResources() => Children.SelectMany(c => c.TranslatableResources()).Concat(new[] { Tooltip, Name });
    }
}
