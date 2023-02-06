using ConsoleSnakeGame.Core.Gridwork.ObstaclePlacements;
using ConsoleSnakeGame.Core.Rendering;
using Utilities.Numerics;

namespace ConsoleSnakeGame.Core
{
    internal enum SnakeColor { Green, Cyan, Yellow }

    internal partial class SnakeGame
    {
        internal class Settings
        {
            public const int InitSnakeGrowth = 3,
                MinGridWidth = 9, MinGridHeight = 9;

            // -1 is calculate automatically
            // null is no final growth
            private int? _finalSnakeGrowth = -1;

            private SnakeColor? _snakeColor;
            private IEnumerable<IntVector2>? _obstaclePositions;

            public int TickRate { get; init; } = 60;

            public Proportion StartSpeed { get; init; } = new(0.4f);
            public Proportion LimitSpeed { get; init; } = new(0.8f);

            public int GrowthForMaxSpeed { get; init; } = 50;

            public int GridWidth { get; init; } = 21;
            public int GridHeight { get; init; } = 15;

            public ObstaclePlacement? ObstaclePlacement { get; init; }

            public int MaxFinalGrowth => GridWidth * GridHeight;

            public IntVector2 SpawnPosition => GetSpawnPosition(GridWidth, GridHeight);

            public int? FinalSnakeGrowth
            {
                get
                {
                    if (_finalSnakeGrowth is -1)
                        _finalSnakeGrowth = MaxFinalGrowth - (ObstaclePositions?.Count() ?? 0);

                    return _finalSnakeGrowth;
                }
                init => _finalSnakeGrowth = value;
            }

            public SnakeColor SnakeColor
            {
                get
                {
                    _snakeColor ??= Enum.GetValues<SnakeColor>().OrderBy(_ => Guid.NewGuid()).First();
                    return _snakeColor.Value;
                }
                init => _snakeColor = value;
            }

            public IEnumerable<IntVector2>? ObstaclePositions
            {
                get
                {
                    if (_obstaclePositions is null)
                    {
                        var gridLength = new IntVector2(GridWidth, GridHeight);
                        _obstaclePositions = ObstaclePlacement?.GetPositions(gridLength);
                    }

                    return _obstaclePositions;
                }
            }

            public static IntVector2 GetSpawnPosition(int gridWidth, int gridHeight)
            {
                return new(gridWidth / 2, gridHeight / 2);
            }

            public void Validate()
            {
                if (GridWidth < MinGridWidth)
                {
                    throw new InvalidOperationException("Grid width cannot be less " +
                        $"than the minimum value ({MinGridWidth}).");
                }
                if (GridHeight < MinGridHeight)
                {
                    throw new InvalidOperationException("Grid height cannot be less " +
                        $"than the minimum value ({MinGridHeight}).");
                }

                if (FinalSnakeGrowth < InitSnakeGrowth || FinalSnakeGrowth > MaxFinalGrowth)
                {
                    throw new InvalidOperationException("Final snake growth cannot be" +
                        $" less than the initial growth ({InitSnakeGrowth})" +
                        $" or greater than the maximum value ({MaxFinalGrowth}).");
                }
            }
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
