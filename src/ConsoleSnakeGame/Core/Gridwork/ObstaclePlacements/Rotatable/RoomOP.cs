using Utilities.Numerics;

namespace ConsoleSnakeGame.Core.Gridwork.ObstaclePlacements.Rotatable
{
    /*
        ╔═══════════════════════════════════════════╗
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · # # # # # # # # # · · · · · · ║
        ║ · · · · · · # · · · · · · · # · · · · · · ║
        ║ · · · · · · # · · · · · · · # · · · · · · ║
        ║ · · · · · · # · · · · · · · # · · · · · · ║
        ║ · · · · · · # · · · · · · · # · · · · · · ║
        ║ · · · · · · # · · · · · · · # · · · · · · ║
        ║ · · · · · · # · · · · · · · # · · · · · · ║
        ║ · · · · · · # · · · · · · · # · · · · · · ║
        ║ · · · · · · # · · · · · · · # · · · · · · ║
        ║ · · · · · · # · · · · · · · # · · · · · · ║
        ║ · · · · · · # · · · · · · · # · · · · · · ║
        ╚═══════════════════════════════════════════╝
    */

    [OP("room1", true, false)]
    [OP("room2", true, true)]
    [OP("room3", false, true)]
    internal class RoomOP : ObstaclePlacement
    {
        private readonly bool _rotate, _mirror;

        public RoomOP(bool rotate, bool mirror)
        {
            (_rotate, _mirror) = (rotate, mirror);
        }

        protected override IEnumerable<IntVector2> CompoutePositions(GridInfo gi)
        {
            var (maxA, maxB) = _rotate ? (gi.Max.Y, gi.Max.X) : (gi.Max.X, gi.Max.Y);

            var start = maxB / 3;

            if (_mirror)
                start = maxB - start;

            int firstA = maxA / 3, lastA = maxA - firstA;

            for (int a = firstA; a <= lastA; a++)
            {
                yield return New(a, start);
            }

            var (firstB, lastB) = _mirror ? (0, start) : (start, maxB);

            for (int b = firstB; b <= lastB; b++)
            {
                yield return New(firstA, b);
                yield return New(lastA, b);
            }

            IntVector2 New(int a, int b) => _rotate ? new(b, a) : new(a, b);
        }
    }
}
