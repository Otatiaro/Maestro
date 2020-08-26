namespace Maestro.Generator.Model.Parameters
{
    public class Enumeration : ParameterBase
    {
        public Enumeration() : base()
        {

        }

        public Enumeration(Enumeration enumeration) : base(enumeration)
        {
            isPersistant = enumeration.isPersistant;
            IsReadOnly = enumeration.isReadOnly;
        }

        private bool isPersistant;
        private bool isReadOnly;
        public bool IsPersistant { get => isPersistant; set => Set(ref isPersistant, value); }
        public bool IsReadOnly { get => isReadOnly; set => Set(ref isReadOnly, value); }

    }
}
