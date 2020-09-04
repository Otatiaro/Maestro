using System.Collections.Generic;
using System.Xml.Serialization;

namespace Maestro.Generator.Model.Controls
{

    [XmlInclude(typeof(Grid))]
    [XmlInclude(typeof(Groupbox))]
    [XmlInclude(typeof(Menu))]
    [XmlInclude(typeof(Text))]
    [XmlInclude(typeof(View))]
    public abstract class ControlBase : ModelBase
    {
        private HorizontalAlignment horizontalAlignment = HorizontalAlignment.None;
        private VerticalAlignment verticalAlignment = VerticalAlignment.None;

        public HorizontalAlignment HorizontalAlignment { get => horizontalAlignment; set => Set(ref horizontalAlignment, value); }
        public VerticalAlignment VerticalAlignment { get => verticalAlignment; set => Set(ref verticalAlignment, value); }

        protected override IEnumerable<(string, object)> Errors()
        {
            yield break;
        }
    }
}
