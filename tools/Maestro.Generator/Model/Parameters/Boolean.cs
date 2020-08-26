namespace Maestro.Generator.Model.Parameters
{
    public class Boolean : ParameterBase
    {
        public Boolean() : base()
        {

        }

        public Boolean(Boolean boolean): base(boolean)
        {
            defaultValue = boolean.defaultValue;
            isPersistant = boolean.isPersistant;
            isReadOnly = boolean.isReadOnly;
        }


        private bool defaultValue;
        private bool isPersistant;
        private bool isReadOnly;
        public bool IsPersistant { get => isPersistant; set => Set(ref isPersistant, value); }
        public bool IsReadOnly { get => isReadOnly; set => Set(ref isReadOnly, value); }

        public bool DefaultValue { get => defaultValue; set => Set(ref defaultValue, value); }
    }
}
