using ConsoleSnakeGame.Core.Gridwork.ObstaclePlacements;
using Utilities.Numerics;

namespace ConsoleSnakeGame.Core.Gridwork.ObstaclePlacements.Symmetrical
{
    /*
        ╔═══════════════════════════════════════════╗
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · # # # # # · · · · · # # # # # · · · ║
        ║ · · · # # # # # · · · · · # # # # # · · · ║
        ║ · · · # # # # # · · · · · # # # # # · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · # # # # # · · · · · # # # # # · · · ║
        ║ · · · # # # # # · · · · · # # # # # · · · ║
        ║ · · · # # # # # · · · · · # # # # # · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ╚═══════════════════════════════════════════╝
    */

    internal class QuadsOP : ObstaclePlacement
    {
        protected override IEnumerable<IntVector2> CompoutePositions(GridInfo gi)
        {
            var start = gi.Mid / 3;
            var end = start + gi.Mid / 2;

            for (int x1 = start.X; x1 < end.X; x1++)
            {
                int x2 = gi.Max.X - x1;

                for (int y1 = start.Y; y1 < end.Y; y1++)
                {
                    int y2 = gi.Max.Y - y1;

                    yield return new(x1, y1);
                    yield return new(x1, y2);

                    yield return new(x2, y1);
                    yield return new(x2, y2);
                }
            }
        }
    }
}
