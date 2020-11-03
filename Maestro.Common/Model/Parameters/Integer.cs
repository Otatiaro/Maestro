using System.Collections.Generic;

namespace Maestro.Common.Model.Parameters
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
        private long maxValue = 100;
        private long minValue = 0;
        private long defaultValue = 50;

        public bool IsPersistant { get => isPersistant; set => Set(ref isPersistant, value); }
        public bool IsReadOnly { get => isReadOnly; set => Set(ref isReadOnly, value); }

        public long MaxValue { get => maxValue; set => Set(ref maxValue, value); }
        public long MinValue { get => minValue; set => Set(ref minValue, value); }
        public long DefaultValue { get => defaultValue; set => Set(ref defaultValue, value); }

        protected override IEnumerable<(string, object)> Errors()
        {
            foreach (var e in base.Errors())
                yield return e;

            if (MaxValue <= MinValue)
            {
                yield return (nameof(MinValue), "Minimum value must be below maximum value");
                yield return (nameof(MaxValue), "Maximum value must be above minimum value");
            }

            if(MaxValue < DefaultValue)
            {
                yield return (nameof(DefaultValue), "Default value must be below or equal to maximum value");
                yield return (nameof(MaxValue), "Maximum value must be above or equal to default value");
            }

            if (MinValue > DefaultValue)
            {
                yield return (nameof(DefaultValue), "Default value must be above or equal to minimum value");
                yield return (nameof(MinValue), "Minimum value must be below or equal to default value");
            }
        }
    }
}
