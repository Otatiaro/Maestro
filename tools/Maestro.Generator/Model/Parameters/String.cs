using System.Runtime.CompilerServices;

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

        private string defaultValue;
        private byte maxSize;
        private bool isPersistant;
        private bool isReadOnly;
        public bool IsPersistant { get => isPersistant; set => Set(ref isPersistant, value); }
        public bool IsReadOnly { get => isReadOnly; set => Set(ref isReadOnly, value); }


        public byte MaxSize { get => maxSize; set => Set(ref maxSize, value); }
        public string DefaultValue { get => defaultValue; set => Set(ref defaultValue, value); }
    }
}
