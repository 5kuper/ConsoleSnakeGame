using Utilities.Numerics;

namespace ConsoleSnakeGame.Core.Gridwork.ObstaclePlacements.Symmetrical
{
    /*
        ╔═══════════════════════════════════════════╗
        ║ # # # # # # # # # # # # # # # # # # # # # ║
        ║ # · · · · · · · · · · · · · · · · · · · # ║
        ║ # · · · · · · · · · · · · · · · · · · · # ║
        ║ # · · · · · · · · · · · · · · · · · · · # ║
        ║ # · · · · · · · · · · · · · · · · · · · # ║
        ║ # · · · · · · · · · · · · · · · · · · · # ║
        ║ # · · · · · · · · · · · · · · · · · · · # ║
        ║ # · · · · · · · · · · · · · · · · · · · # ║
        ║ # · · · · · · · · · · · · · · · · · · · # ║
        ║ # · · · · · · · · · · · · · · · · · · · # ║
        ║ # · · · · · · · · · · · · · · · · · · · # ║
        ║ # · · · · · · · · · · · · · · · · · · · # ║
        ║ # · · · · · · · · · · · · · · · · · · · # ║
        ║ # · · · · · · · · · · · · · · · · · · · # ║
        ║ # # # # # # # # # # # # # # # # # # # # # ║
        ╚═══════════════════════════════════════════╝
     */

    [OP("enclosure")]
    internal class EnclosureOP : ObstaclePlacement
    {
        protected override IEnumerable<IntVector2> CompoutePositions(GridInfo gi)
        {
            for (int x = 0; x < gi.Width; x++)
            {
                yield return new(x, gi.Min.Y);
                yield return new(x, gi.Max.Y);
            }

            for (int y = 0; y < gi.Height; y++)
            {
                yield return new(gi.Min.X, y);
                yield return new(gi.Max.X, y);
            }
        }
    }
}
