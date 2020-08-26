namespace Maestro.Generator.Model.Parameters
{
    public class Button : ParameterBase
    {
        public Button() : base()
        {

        }

        public Button(Button button) : base(button)
        {
            repetitionDelayMs = button.repetitionDelayMs;
            repetitionIntervalMs = button.repetitionIntervalMs;
        }

        private ushort repetitionDelayMs = 2000;
        private ushort repetitionIntervalMs = 500;

        public ushort RepetitionDelayMs { get => repetitionDelayMs; set => Set(ref repetitionDelayMs, value); }
        public ushort RepetitionIntervalMs { get => repetitionIntervalMs; set => Set(ref repetitionIntervalMs, value); }
    }
}
