using Utilities.Numerics;

namespace ConsoleSnakeGame.Core.Gridwork.ObstaclePlacements.Rotatable
{
    /*
        ╔═══════════════════════════════════════════╗
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ # # # # # # # # # # # # # # # # # # # # # ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ║ · · · · · · · · · · · · · · · · · · · · · ║
        ╚═══════════════════════════════════════════╝
    */

    [OP("separator1", true, false)]
    [OP("separator2", true, true)]
    [OP("separator3", false, true)]
    internal class SeparatorOP : ObstaclePlacement
    {
        private readonly bool _rotate, _mirror;

        public SeparatorOP(bool rotate, bool mirror)
        {
            (_rotate, _mirror) = (rotate, mirror);
        }

        protected override IEnumerable<IntVector2> CompoutePositions(GridInfo gi)
        {
            var (a, b) = _rotate ? (gi.Height, gi.Width) : (gi.Width, gi.Height);

            int len = a, pos = b / 4;

            if (_mirror) pos = b - pos - 1;

            for (int i = 0; i < len; i++)
                yield return _rotate ? new(pos, i) : new(i, pos);
        }
    }
}
