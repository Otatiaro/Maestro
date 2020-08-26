using Maestro.Generator.Model.Controls;

namespace Maestro.Generator.Model
{
    public class View : ModelBase
    {
        private ControlBase root;
        private string icon;

        public ControlBase Root { get => root; set => Set(ref root, value); }
        public string Icon { get => icon; set => Set(ref icon, value); }

    }
}
