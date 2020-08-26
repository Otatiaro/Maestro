using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Maestro.Generator.Model.Controls
{

    public class ControlBase : ModelBase
    {
        private HorizontalAlignment horizontalAlignment;
        private VerticalAlignment verticalAlignment;

        public HorizontalAlignment HorizontalAlignment { get => horizontalAlignment; set => Set(ref horizontalAlignment, value); }
        public VerticalAlignment VerticalAlignment { get => verticalAlignment; set => Set(ref verticalAlignment, value); }
    }
}
