using ConsoleSnakeGame.Core.ObstaclePlacements;
using ConsoleSnakeGame.Core.Rendering;
using Utilities.Numerics;

namespace ConsoleSnakeGame.Core
{
    internal enum SnakeColor { Green, Cyan, Yellow }

    internal partial class SnakeGame
    {
        internal struct Settings
        {
            private SnakeColor? _snakeColor = null;

            public Settings() => FinalSnakeGrowth = GridWidth * GridHeight;

            public int TickRate { get; init; } = 5;

            public int GridWidth { get; init; } = 21;
            public int GridHeight { get; init; } = 15;

            public int InitialSnakeGrowth { get; init; } = 3;

            public int? FinalSnakeGrowth { get; init; }

            public ObstaclePlacement? ObstaclePlacement { get; init; } = null;

            public SnakeColor SnakeColor
            {
                get
                {
                    _snakeColor ??= Enum.GetValues<SnakeColor>().OrderBy(_ => Guid.NewGuid()).First();
                    return _snakeColor.Value;
                }
                init => _snakeColor = value;
            }
        }

        protected IEnumerable<IntVector2>? GetObstaclePositions()
        {
            var gridLength = new IntVector2(Sets.GridWidth, Sets.GridHeight);
            return Sets.ObstaclePlacement?.GetPositions(gridLength);
        }

        protected RenderingRule<ConsoleColor>[] GetSnakeColorRules()
        {
            return Sets.SnakeColor switch
            {
                SnakeColor.Green => RenderingRules.GreenSnakeColorRules,
                SnakeColor.Cyan => RenderingRules.CyanSnakeColorRules,
                SnakeColor.Yellow => RenderingRules.YellowSnakeColorRules,

                _ => throw new InvalidOperationException($"Value {(int)Sets.SnakeColor} "
                                                        + $"of {typeof(SnakeColor)} is invalid.")
            };
        }
    }
}
