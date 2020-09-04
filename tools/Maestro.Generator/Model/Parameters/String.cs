using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Maestro.Generator.Model.Parameters
{
    public class String : ParameterBase
    {
        public String() : base()
        {

        }

        public String(String str) : base(str)
        {
            defaultValue = str.defaultValue;
            maxSize = str.maxSize;
            isPersistant = str.isPersistant;
            isReadOnly = str.isReadOnly;
        }

        private string defaultValue = "Default string";
        private byte maxSize = 16;
        private bool isPersistant;
        private bool isReadOnly;
        public bool IsPersistant { get => isPersistant; set => Set(ref isPersistant, value); }
        public bool IsReadOnly { get => isReadOnly; set => Set(ref isReadOnly, value); }


        public byte MaxSize { get => maxSize; set => Set(ref maxSize, value); }
        public string DefaultValue { get => defaultValue; set => Set(ref defaultValue, value); }

        protected override IEnumerable<(string, object)> Errors()
        {
            foreach (var e in base.Errors())
                yield return e;

            
            if (Encoding.ASCII.GetBytes(defaultValue).Length >= MaxSize)
            {
                yield return (nameof(DefaultValue), "Default value is longer than allowed size");
                yield return (nameof(MaxSize), "Max size if too low to contain default value");
            }

            if (MaxSize == 0)
                yield return (nameof(MaxSize), "Max size cannot be zero");
        }
    }
}
