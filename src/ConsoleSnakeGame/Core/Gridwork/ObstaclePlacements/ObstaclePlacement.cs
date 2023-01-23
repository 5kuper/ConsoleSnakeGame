using Utilities.Numerics;

namespace ConsoleSnakeGame.Core.Gridwork.ObstaclePlacements
{
    internal abstract class ObstaclePlacement
    {
        public ObstaclePlacement? Next { get; init; }

        public IEnumerable<IntVector2> GetPositions(IntVector2 gridLength)
        {
            var positions = new HashSet<IntVector2>(CompoutePositions(new(gridLength.X, gridLength.Y)));
            return positions.Union(Next?.GetPositions(gridLength) ?? Enumerable.Empty<IntVector2>());
        }

        protected abstract IEnumerable<IntVector2> CompoutePositions(GridInfo gi);

        protected record struct GridInfo(int Width, int Height)
        {
            public IntVector2 Min => new(0);
            public IntVector2 Mid => new(Width / 2, Height / 2);
            public IntVector2 Max => new(Width - 1, Height - 1);
        }
    }
}
