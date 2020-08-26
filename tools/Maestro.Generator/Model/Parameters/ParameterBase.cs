using System;
using System.Xml.Serialization;

namespace Maestro.Generator.Model.Parameters
{
    [XmlInclude(typeof(Button))]
    [XmlInclude(typeof(Boolean))]
    [XmlInclude(typeof(ByteStream))]
    [XmlInclude(typeof(Enumeration))]
    [XmlInclude(typeof(Integer))]
    [XmlInclude(typeof(String))]
    public abstract class ParameterBase : ModelBase
    {
        public ParameterBase()
        {

        }

        public ParameterBase(ParameterBase parameter)
        {
            tooltip = parameter.Tooltip;
            displayName = parameter.DisplayName + " Copy";
            tag = parameter.Tag + "_copy";
        }

        private string tooltip = "Sample tooltip";
        private string displayName = "Display Name";
        private string tag = "display_name";

        public string Tooltip { get => tooltip; set => Set(ref tooltip, value); }
        public string DisplayName { get => displayName; set => Set(ref displayName, value); }
        public string Tag { get => tag; set => Set(ref tag, value); }

        public string DisplayType => GetType().Name;


        public override string ToString()
        {
            return $"{GetType().Name} \"{displayName}\" ({tag})";
        }
    }
}
