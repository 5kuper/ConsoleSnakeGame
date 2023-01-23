using ConsoleSnakeGame.Core.Gridwork.ObstaclePlacements;
using Utilities.Numerics;

namespace ConsoleSnakeGame.Core.Gridwork.ObstaclePlacements.Rotatable
{
    /*
        ╔═══════════════════════════════════════════╗
        ║ · · · · · · · · · · # · · · · · · · · · · ║
        ║ · · · · · · · · · · # · · · · · · · · · · ║
        ║ · · · · · · · · · · # · · · · · · · · · · ║
        ║ · · · · · · · · · · # · · · · · · · · · · ║
        ║ · · · · · · · · · · # · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · # · · · · · · · · · # · · · · · ║
        ║ · · · · · # · · · · · · · · · # · · · · · ║
        ║ · · · · · # · · · · · · · · · # · · · · · ║
        ║ · · · · · # · · · · · · · · · # · · · · · ║
        ║ · · · · · # · · · · · · · · · # · · · · · ║
        ╚═══════════════════════════════════════════╝
    */

    internal class WallsOP : ObstaclePlacement
    {
        private readonly bool _rotate, _mirror;

        public WallsOP(bool rotate, bool mirror)
        {
            (_rotate, _mirror) = (rotate, mirror);
        }

        protected override IEnumerable<IntVector2> CompoutePositions(GridInfo gi)
        {
            IntVector2 New(int a, int b) => _rotate ? new(b, a) : new(a, b);

            var (maxA, maxB) = _rotate ? (gi.Max.Y, gi.Max.X) : (gi.Max.X, gi.Max.Y);

            var wallLen = (int)(maxB / 3.5f);

            var unary = ComputeUnary();
            var binary = ComputeBinary();

            return unary.Union(binary);

            IEnumerable<IntVector2> ComputeUnary()
            {
                var (startB, endB) = _mirror ? (maxB - wallLen, maxB) : (0, wallLen);

                for (int b = startB; b <= endB; b++)
                    yield return New(maxA / 2, b);
            }

            IEnumerable<IntVector2> ComputeBinary()
            {
                int a1 = maxA / 4, a2 = maxA - a1;

                var (startB, endB) = _mirror ? (0, wallLen) : (maxB - wallLen, maxB);

                for (int b = startB; b <= endB; b++)
                {
                    yield return New(a1, b);
                    yield return New(a2, b);
                }
            }
        }
    }
}
