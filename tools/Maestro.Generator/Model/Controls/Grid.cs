using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Maestro.Generator.Model.Controls
{
    public class Grid : ControlBase
    {
        private byte columns = 1;
        private byte rows = 1;

        public class GridElement : ControlBase
        {
            private ControlBase control;
            private byte row = 0;
            private byte column = 0;
            private byte rowSpan = 1;
            private byte columnSpan = 1;

            public ControlBase Control { get => control; set => Set(ref control, value); }
            public byte Row { get => row; set => Set(ref row, value); }
            public byte Column { get => column; set => Set(ref column, value); }
            public byte RowSpan { get => rowSpan; set => Set(ref rowSpan, value); }
            public byte ColumnSpan { get => columnSpan; set => Set(ref columnSpan, value); }

            public override IEnumerable<string> TranslatableResources()
            {
                yield break;
            }
        }

        public ObservableCollection<GridElement> Children { get; } = new ObservableCollection<GridElement>();
        public byte Rows { get => rows; set => Set(ref rows, value); }
        public byte Columns { get => columns; set => Set(ref columns, value); }

        public override IEnumerable<string> TranslatableResources() =>  Children.SelectMany(c => c.TranslatableResources());
    }
}
