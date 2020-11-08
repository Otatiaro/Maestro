using System.Collections.Generic;
using System.Xml.Serialization;

namespace Maestro.Common.Model
{
    public class Descriptor : ModelBase
    {
        public string Product { get => product; 
            set => Set(ref product, value); }
        public string Firmware { get => firmware; set => Set(ref firmware, value); }
        public ushort VersionMajor { get => versionMajor; set => Set(ref versionMajor, value); }
        public ushort VersionMinor { get => versionMinor; set => Set(ref versionMinor, value); }
        public ushort VersionBuild { get => versionBuild; set => Set(ref versionBuild, value); }
        public ushort HardwareRevision { get => hardwareRevision; set => Set(ref hardwareRevision, value); }

        [XmlIgnore]
        public string Name { get => name; set => Set(ref name, value); }

        private string product = "Sample Product";
        private string firmware = "Sample firmware";
        private ushort versionMajor = 1;
        private ushort versionMinor = 2;
        private ushort versionBuild = 3;
        private ushort hardwareRevision = 4;
        private string name;

        protected override IEnumerable<(string, object)> Errors()
        {
            yield break;
        }

        public override IEnumerable<string> TranslatableResources()
        {
            yield break;
        }
    }
}
