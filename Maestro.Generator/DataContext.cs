using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Maestro.Common;
using Maestro.Common.Model;

namespace Maestro.Generator
{
    public class DataContext : INotifyPropertyChanged
    {
        private Database database = new Database();
        private string fileName;

        private const string DatabaseExtensions = ".mtrdb";
        private const string CompressedDatabaseExtension = ".mtrcp";
        
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

            switch (Path.GetExtension(FileName).ToLowerInvariant())
            {
                case CompressedDatabaseExtension:
                    Database = Database.LoadXml(File.ReadAllBytes(FileName).Decompress());
                    break;
                case DatabaseExtensions:
                    Database = Database.LoadXml(File.ReadAllBytes(FileName));
                    break;
                default:
                    throw new Exception("Unknown file extension");
            }
        }

        public void Save(string path = null)
        {
            if (!string.IsNullOrWhiteSpace(path))
                FileName = path;

            switch (Path.GetExtension(FileName).ToLowerInvariant())
            {
                case CompressedDatabaseExtension:
                    File.WriteAllBytes(FileName, Database.SaveXml().Compress());
                    break;
                case DatabaseExtensions:
                    File.WriteAllBytes(FileName, Database.SaveXml());
                    break;
                default:
                    throw new Exception("Unknown file extension");
            }

            Generate();
        }

        public void Generate()
        {
            // TODO generate the output files (c++ code and maestro config binary file)
        }


    }
}
