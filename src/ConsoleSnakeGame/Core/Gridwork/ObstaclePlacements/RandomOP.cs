using ConsoleSnakeGame.Core.Gridwork.ObstaclePlacements.Rotatable;
using ConsoleSnakeGame.Core.Gridwork.ObstaclePlacements.Symmetrical;
using Utilities.Numerics;

namespace ConsoleSnakeGame.Core.Gridwork.ObstaclePlacements
{
    using OPs = List<ObstaclePlacement>;

    [OP("random")]
    internal class RandomOP : ObstaclePlacement
    {
        private static readonly Lazy<OPs> _variants = new(() => new()
        {
            new EnclosureOP(), new QuadsOP(), new CornersOP(), new CrossOP(), new XMarkOP(),

            new RoomOP(true, false), new RoomOP(true, true), new RoomOP(false, true),
            new WallsOP(true, false), new WallsOP(true, true), new WallsOP(false, true),
            new SeparatorOP(true, false), new SeparatorOP(true, true), new SeparatorOP(false, true),

            new EnclosureOP() { Next = new CrossOP() }, new QuadsOP() { Next = new CrossOP() },
            new CornersOP() { Next = new CrossOP() }, new XMarkOP() { Next = new CrossOP() },

            new EnclosureOP() { Next = new WallsOP(true, false) },
            new EnclosureOP() { Next = new WallsOP(true, true) },
            new EnclosureOP() { Next = new WallsOP(false, true) },
        });

        protected override IEnumerable<IntVector2> CompoutePositions(GridInfo gi)
        {
            return _variants.Value
                .OrderBy(_ => Guid.NewGuid())
                .First()
                .GetPositions(new(gi.Width, gi.Height));
        }
    }
}
