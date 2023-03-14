using ConsoleSnakeGame.Core.Gridwork.ObstaclePlacements;
using ConsoleSnakeGame.Core.Rendering;
using System.Text.Json;
using System.Text.Json.Serialization;
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

            public int GridWidth { get; init; } = 21;
            public int GridHeight { get; init; } = 15;

            public int TickRate { get; init; } = 60;

            public Proportion StartSpeed { get; init; } = new(0.4f);
            public Proportion LimitSpeed { get; init; } = new(0.8f);

            public int GrowthForMaxSpeed { get; init; } = 50;

            [JsonIgnore]
            public ObstaclePlacement? ObstaclePlacement { get; init; }

            [JsonIgnore]
            public int MaxFinalGrowth => GridWidth * GridHeight;

            [JsonIgnore]
            public IntVector2 SpawnPosition => GetSpawnPosition(GridWidth, GridHeight);

            [JsonIgnore]
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

            [JsonIgnore]
            public SnakeColor SnakeColor
            {
                get
                {
                    _snakeColor ??= Enum.GetValues<SnakeColor>().OrderBy(_ => Guid.NewGuid()).First();
                    return _snakeColor.Value;
                }
                init => _snakeColor = value;
            }

            [JsonIgnore]
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

            public static Settings Construct(string? jsonConfig, IEnumerable<string>? obstacles, SnakeColor? color)
            {
                var result = jsonConfig is not null ? JsonSerializer.Deserialize<Settings>(jsonConfig)! : new();

                if (obstacles?.Any() == true)
                {
                    var op = OPAttribute.Create(obstacles);
                    var gridLength = new IntVector2(result.GridWidth, result.GridHeight);
                    result._obstaclePositions = op.GetPositions(gridLength);
                }

                if (color != null) result._snakeColor = color;
                return result;
            }

            public static string GetDefaultJsonConfig()
            {
                return JsonSerializer.Serialize(new Settings(), new JsonSerializerOptions { WriteIndented = true });
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
