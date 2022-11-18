using ConsoleSnakeGame.Core.Rendering;

namespace ConsoleSnakeGame.Core
{
    internal enum SnakeColor { Green, Cyan, Yellow }

    internal struct Settings : IGame.ISettings
    {
        private SnakeColor? _snakeColor = null;

        public Settings() => FinalSnakeGrowth = GridWidth * GridHeight;

        public int TickRate { get; init; } = 5;

        public int GridWidth { get; init; } = 20;
        public int GridHeight { get; init; } = 15;

        public int InitialSnakeGrowth { get; init; } = 3;

        public int? FinalSnakeGrowth { get; init; }

        public SnakeColor SnakeColor
        {
            get
            {
                _snakeColor ??= Enum.GetValues<SnakeColor>().OrderBy(_ => Guid.NewGuid()).First();
                return _snakeColor.Value;
            }
            init => _snakeColor = value;
        }

        public RenderingRule<ConsoleColor>[] SnakeColorRules => SnakeColor switch
        {
            SnakeColor.Green => RenderingRules.GreenSnakeColorRules,
            SnakeColor.Cyan => RenderingRules.CyanSnakeColorRules,
            SnakeColor.Yellow => RenderingRules.YellowSnakeColorRules,
            _ => throw new InvalidOperationException($"Value {(int)SnakeColor} of {typeof(SnakeColor)} is invalid.")
        };
    }
}
