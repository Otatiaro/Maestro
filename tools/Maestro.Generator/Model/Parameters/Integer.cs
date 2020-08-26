namespace Maestro.Generator.Model.Parameters
{
    public class Integer : ParameterBase
    {
        public Integer() : base()
        {

        }

        public Integer(Integer integer) : base(integer)
        {
            isPersistant = integer.isPersistant;
            isReadOnly = integer.isReadOnly;
            maxValue = integer.maxValue;
            minValue = integer.minValue;
            defaultValue = integer.defaultValue;
        }

        private bool isPersistant;
        private bool isReadOnly;
        private long maxValue;
        private long minValue;
        private long defaultValue;

        public bool IsPersistant { get => isPersistant; set => Set(ref isPersistant, value); }
        public bool IsReadOnly { get => isReadOnly; set => Set(ref isReadOnly, value); }

        public long MaxValue { get => maxValue; set => Set(ref maxValue, value); }
        public long MinValue { get => minValue; set => Set(ref minValue, value); }
        public long DefaultValue { get => defaultValue; set => Set(ref defaultValue, value); }
    }
}
