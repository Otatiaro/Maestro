using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Maestro.Generator.Model.Parameters
{
    public class Enumeration : ParameterBase
    {
        public class EnumerationValue : ModelBase
        {
            private string tooltip = "Sample tooltip";
            private string displayName = "Display Name";
            private string tag = "display_name";

            public string Tooltip { get => tooltip; set => Set(ref tooltip, value); }
            public string DisplayName
            {
                get => displayName; set
                {
                    if (Tag == displayName.ToC())
                        Tag = value.ToC();
                    Set(ref displayName, value);
                }
            }
            public string Tag { get => tag; set => Set(ref tag, value); }

            protected override IEnumerable<(string, object)> Errors()
            {
                if (string.IsNullOrWhiteSpace(DisplayName))
                    yield return (nameof(Tooltip), "Display name cannot be empty");
                foreach (var e in Tag.IsC())
                    yield return (nameof(Tag), $"{nameof(Tag)} {e}");
            }

            public override string ToString()
            {
                return $"Enumeration value {displayName} ({tag})";
            }

            public override IEnumerable<string> TranslatableResources()
            {
                yield return Tooltip;
                yield return DisplayName;
            }
        }

        public Enumeration() : base()
        {
            Values.CollectionChanged += ValuesChanged;
        }


        public Enumeration(Enumeration enumeration) : base(enumeration)
        {
            isPersistant = enumeration.isPersistant;
            IsReadOnly = enumeration.isReadOnly;
            Values.CollectionChanged += ValuesChanged;
        }

        private bool isPersistant;
        private bool isReadOnly;
        private int defaultValue;

        public bool IsPersistant { get => isPersistant; set => Set(ref isPersistant, value); }
        public bool IsReadOnly { get => isReadOnly; set => Set(ref isReadOnly, value); }

        public ObservableCollection<EnumerationValue> Values { get; } = new ObservableCollection<EnumerationValue>();
        public int DefaultValue { get => defaultValue; set => Set(ref defaultValue, value); }

        protected override IEnumerable<(string, object)> Errors()
        {
            foreach (var e in base.Errors())
                yield return e;

            if (defaultValue < 0 || defaultValue >= Values.Count)
                yield return (nameof(DefaultValue), "Default value out of bounds");

            if (Values.Select(v => v.Tag).Distinct().Count() != Values.Select(v => v.Tag).Count())
                yield return (nameof(Values), "Every tag must be different");
        }

        private void ValuesChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyChange();
        }

        public override IEnumerable<string> TranslatableResources()
        {
            return Values.SelectMany(v => v.TranslatableResources());
        }

    }
}
