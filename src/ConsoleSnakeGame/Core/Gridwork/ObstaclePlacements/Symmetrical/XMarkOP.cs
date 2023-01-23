using ConsoleSnakeGame.Core.Gridwork.ObstaclePlacements;
using Utilities.Numerics;

namespace ConsoleSnakeGame.Core.Gridwork.ObstaclePlacements.Symmetrical
{
    /*
        ╔═══════════════════════════════════════════╗
        ║ # · · · · · · · · · · · · · · · · · · · # ║
        ║ · # · · · · · · · · · · · · · · · · · # · ║
        ║ · · # · · · · · · · · · · · · · · · # · · ║
        ║ · · · # · · · · · · · · · · · · · # · · · ║
        ║ · · · · # · · · · · · · · · · · # · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · # · · · · · · · · · · · # · · · · ║
        ║ · · · # · · · · · · · · · · · · · # · · · ║
        ║ · · # · · · · · · · · · · · · · · · # · · ║
        ║ · # · · · · · · · · · · · · · · · · · # · ║
        ║ # · · · · · · · · · · · · · · · · · · · # ║
        ╚═══════════════════════════════════════════╝
    */

    internal class XMarkOP : ObstaclePlacement
    {
        protected override IEnumerable<IntVector2> CompoutePositions(GridInfo gi)
        {
            for (var pos = gi.Min; pos.X < Math.Min(gi.Width, gi.Height) / 3; pos += IntVector2.One)
            {
                yield return pos;

                yield return new(gi.Max.X - pos.X, pos.Y);
                yield return new(pos.X, gi.Max.Y - pos.Y);

                yield return gi.Max - pos;
            }
        }
    }
}
