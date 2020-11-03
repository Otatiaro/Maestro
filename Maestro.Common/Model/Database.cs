using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Maestro.Common.Model.Controls;
using Maestro.Common.Model.Parameters;

namespace Maestro.Common.Model
{
    public class Database : ModelBase
    {
        public Database()
        {
            ListNames.Add(CommonParameters, "Common parameters");
            ListNames.Add(SetupParameters, "Setup parameters");
        }

        ~Database()
        {
            ListNames.Remove(CommonParameters);
            ListNames.Remove(SetupParameters);
        }

        public string BaseURL { get => baseURL; set => Set(ref baseURL, value); }
        public string DefaultLanguage { get => defaultLanguage; set => Set(ref defaultLanguage, value); }
        public string GenerationDirectory { get => generationDirectory; set => Set(ref generationDirectory, value); }
        public byte SetupCount { get => setupCount; set => Set(ref setupCount, value); }
        public Descriptor Descriptor { get => descriptor; set => Set(ref descriptor, value);
        }

        public ObservableCollection<ParameterBase> CommonParameters { get; } = new ObservableCollection<ParameterBase>();


        public ObservableCollection<ParameterBase> SetupParameters { get; } = new ObservableCollection<ParameterBase>();
        public ObservableCollection<View> Views { get; } = new ObservableCollection<View>();

        public ushort DefaultView { get => defaultView; set => Set(ref defaultView, value); }

        public byte DefaultSetup { get => defaultSetup; set => Set(ref defaultSetup, value); }


        public static IEnumerable<KeyValuePair<string, string>> Languages => CultureInfo.GetCultures(CultureTypes.NeutralCultures)
                    .Select(culture => new KeyValuePair<string, string>(culture.TwoLetterISOLanguageName, culture.DisplayName))
                    .OrderBy(pair => pair.Value);

        public static IEnumerable<byte> Setups => Enumerable.Range(1, 200).Select(i => (byte)i);

        protected static XmlSerializer serializer = new XmlSerializer(typeof(Database));



        private string baseURL = "maestro.synapseos.org";
        private string defaultLanguage = "en";
        private string generationDirectory = "./";
        public ushort defaultView = 0;
        private byte setupCount = 3;
        private byte defaultSetup = 1;
        private Descriptor descriptor = new Descriptor();

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

        public static Database LoadXml(byte[] buffer)
        {
            using(var stream = new MemoryStream(buffer))
                return serializer.Deserialize(stream) as Database;
        }

        public override string ToString()
        {
            return $"{Descriptor.Product} ({Descriptor.Firmware})";
        }

        protected override IEnumerable<(string, object)> Errors()
        {
            if (DefaultSetup == 0)
                yield return (nameof(DefaultSetup), "The default setup cannot be zero");

            if (defaultSetup > setupCount)
            {
                yield return (nameof(DefaultSetup), $"The default setup is too high, there are only {setupCount} setups");
                yield return (nameof(SetupCount), $"The setup count is too low for the default setup");
            }
        }


        public override IEnumerable<string> TranslatableResources()
        {
            return CommonParameters.SelectMany(p => p.TranslatableResources())
                .Concat(SetupParameters.SelectMany(p => p.TranslatableResources()))
                .Concat(Views.SelectMany(v => v.TranslatableResources()));
        }
    }
}
