using Utilities.Numerics;

namespace ConsoleSnakeGame.Core.ObstaclePlacements.Symmetrical
{
    /*
       ╔═══════════════════════════════════════════╗
       ║ · · · · · · · · · · # · · · · · · · · · · ║
       ║ · · · · · · · · · · # · · · · · · · · · · ║
       ║ · · · · · · · · · · # · · · · · · · · · · ║
       ║ · · · · · · · · · · · · · · · · · · · · · ║
       ║ · · · · · · · · · · · · · · · · · · · · · ║
       ║ · · · · · · · · · · · · · · · · · · · · · ║
       ║ · · · · · · · · · · · · · · · · · · · · · ║
       ║ # # # # # · · · · · · · · · · · # # # # # ║
       ║ · · · · · · · · · · · · · · · · · · · · · ║
       ║ · · · · · · · · · · · · · · · · · · · · · ║
       ║ · · · · · · · · · · · · · · · · · · · · · ║
       ║ · · · · · · · · · · · · · · · · · · · · · ║
       ║ · · · · · · · · · · # · · · · · · · · · · ║
       ║ · · · · · · · · · · # · · · · · · · · · · ║
       ║ · · · · · · · · · · # · · · · · · · · · · ║
       ╚═══════════════════════════════════════════╝
    */

    internal class CrossOP : ObstaclePlacement
    {
        protected override IEnumerable<IntVector2> CompoutePositions(GridInfo gi)
        {
            for (int x1 = 0; x1 < gi.Width / 4; x1++)
            {
                yield return new(x1, gi.Mid.Y);
                int x2 = gi.Max.X - x1;
                yield return new(x2, gi.Mid.Y);
            }

            for (int y1 = 0; y1 < gi.Height / 4; y1++)
            {
                yield return new(gi.Mid.X, y1);
                int y2 = gi.Max.Y - y1;
                yield return new(gi.Mid.X, y2);
            }
        }
    }
}
