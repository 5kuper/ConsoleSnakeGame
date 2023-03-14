using Utilities.Numerics;

namespace ConsoleSnakeGame.Core.Gridwork.ObstaclePlacements.Symmetrical
{
    /*
        ╔═══════════════════════════════════════════╗
        ║ # # # # # # # · · · · · · · # # # # # # # ║
        ║ # · · · · · · · · · · · · · · · · · · · # ║
        ║ # · · · · · · · · · · · · · · · · · · · # ║
        ║ # · · · · · · · · · · · · · · · · · · · # ║
        ║ # · · · · · · · · · · · · · · · · · · · # ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ # · · · · · · · · · · · · · · · · · · · # ║
        ║ # · · · · · · · · · · · · · · · · · · · # ║
        ║ # · · · · · · · · · · · · · · · · · · · # ║
        ║ # · · · · · · · · · · · · · · · · · · · # ║
        ║ # # # # # # # · · · · · · · # # # # # # # ║
        ╚═══════════════════════════════════════════╝
    */

    [OP("corners")]
    internal class CornersOP : ObstaclePlacement
    {
        protected override IEnumerable<IntVector2> CompoutePositions(GridInfo gi)
        {
            for (int x1 = 0; x1 < gi.Width / 3; x1++)
            {
                yield return new(x1, gi.Min.Y);
                yield return new(x1, gi.Max.Y);

                int x2 = gi.Max.X - x1;

                yield return new(x2, gi.Min.Y);
                yield return new(x2, gi.Max.Y);
            }

            for (int y1 = 0; y1 < gi.Height / 3; y1++)
            {
                yield return new(gi.Min.X, y1);
                yield return new(gi.Max.X, y1);

                int y2 = gi.Max.Y - y1;

                yield return new(gi.Min.X, y2);
                yield return new(gi.Max.X, y2);
            }
        }
    }
}
