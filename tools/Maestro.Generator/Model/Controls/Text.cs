using System.Collections.Generic;

namespace Maestro.Generator.Model.Controls
{
    public class Text : ControlBase
    {
        private string content = "Sample Text";

        public string Content { get => content; set => Set(ref content, value); }

        public override IEnumerable<string> TranslatableResources() => new[] { Content };
    }
}
