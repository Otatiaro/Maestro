using Maestro.Generator.Model.Parameters;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace Maestro.Generator.Model
{
    public class Database : ModelBase
    {
        public Database()
        {
            ModelBase.ListNames.Add(commonParameters, "Common parameters");
            ModelBase.ListNames.Add(setupParameters, "Setup parameters");
        }

        ~Database()
        {
            ModelBase.ListNames.Remove(commonParameters);
            ModelBase.ListNames.Remove(setupParameters);
        }

        public string Product { get => product; set => Set(ref product, value); }
        public string Firmware { get => firmware; set => Set(ref firmware, value); }
        public string BaseURL { get => baseURL; set => Set(ref baseURL, value); }
        public ushort VersionMajor { get => versionMajor; set => Set(ref versionMajor, value); }
        public ushort VersionMinor { get => versionMinor; set => Set(ref versionMinor, value); }
        public ushort VersionBuild { get => versionBuild; set => Set(ref versionBuild, value); }
        public ushort HardwareRevision { get => hardwareRevision; set => Set(ref hardwareRevision, value); }
        public string DefaultLanguage { get => defaultLanguage; set => Set(ref defaultLanguage, value); }
        public string GenerationDirectory { get => generationDirectory; set => Set(ref generationDirectory, value); }
        public byte SetupCount { get => setupCount; set => Set(ref setupCount, value); }

        public ObservableCollection<ParameterBase> CommonParameters { get => commonParameters; set => Set(ref commonParameters, value); }
        public ObservableCollection<ParameterBase> SetupParameters { get => setupParameters; set => Set(ref setupParameters, value); }


        public static IEnumerable<KeyValuePair<string, string>> Languages => CultureInfo.GetCultures(CultureTypes.NeutralCultures)
                    .Select(culture => new KeyValuePair<string, string>(culture.TwoLetterISOLanguageName, culture.DisplayName))
                    .OrderBy(pair => pair.Value);

        protected static XmlSerializer serializer = new XmlSerializer(typeof(Database));

        [XmlElement(typeof(Boolean))]
        [XmlElement(typeof(Button))]
        [XmlElement(typeof(Enumeration))]
        [XmlElement(typeof(Integer))]
        [XmlElement(typeof(String))]
        [XmlElement(typeof(ByteStream))]
        private ObservableCollection<ParameterBase> commonParameters = new ObservableCollection<ParameterBase>();

        [XmlElement(typeof(Boolean))]
        [XmlElement(typeof(Button))]
        [XmlElement(typeof(Enumeration))]
        [XmlElement(typeof(Integer))]
        [XmlElement(typeof(String))]
        [XmlElement(typeof(ByteStream))]
        private ObservableCollection<ParameterBase> setupParameters = new ObservableCollection<ParameterBase>();

        private string product = "Sample Product";
        private string firmware = "Sample firmware";
        private string baseURL = "maestro.synapseos.org";
        private ushort versionMajor = 1;
        private ushort versionMinor = 2;
        private ushort versionBuild = 3;
        private ushort hardwareRevision = 4;
        private string defaultLanguage = "en";
        private string generationDirectory = "./";
        private byte setupCount = 3;

        public void SaveXml(string path)
        {
            if (File.Exists(path))
                File.Delete(path);

            using (var stream = new FileStream(path, FileMode.CreateNew))
                serializer.Serialize(stream, this);
        }

        public static Database LoadXml(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open))
                return serializer.Deserialize(stream) as Database;
        }

        public override string ToString()
        {
            return $"{Product} ({Firmware})";
        }
    }
}
