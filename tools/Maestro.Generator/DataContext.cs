using Maestro.Generator.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace Maestro.Generator
{
    public class DataContext : INotifyPropertyChanged
    {
        private Database database = new Database();
        private string fileName;

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }


        public Database Database { get => database; set => Set(ref database, value); }
        public string FileName { get => fileName; private set => Set(ref fileName, value); }

        public void Reset()
        {
            Database = new Database();
            FileName = string.Empty;
        }

        public void Load(string path)
        {
            FileName = path;
            Database = Database.LoadXml(path);
        }

        public void Save(string path = null)
        {
            if (!string.IsNullOrWhiteSpace(path))
                FileName = path;

            Database.SaveXml(FileName);
            Generate();
        }

        public void Generate()
        {
            string output = Path.Combine(Path.GetDirectoryName(FileName), database.GenerationDirectory);
            if (!Directory.Exists(output))
                Directory.CreateDirectory(output);

            // TODO generate the output files (c++ code and maestro config binary file)
        }



    }
}
