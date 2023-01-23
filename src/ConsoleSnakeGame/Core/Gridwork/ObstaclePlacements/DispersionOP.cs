using Utilities.Numerics;

namespace ConsoleSnakeGame.Core.Gridwork.ObstaclePlacements
{
    internal class DispersionOP : ObstaclePlacement
    {
        private static IEnumerable<IntVector2> GetSafeZone(GridInfo gi)
        {
            var spawn = SnakeGame.Settings.GetSpawnPosition(gi.Width, gi.Height);
            var posits = new List<IntVector2>() { new(spawn.X, spawn.Y - 1), new(spawn.X, spawn.Y - 2) };

            for (int i = 0; i <= SnakeGame.Settings.InitSnakeGrowth; i++)
                posits.Add(new(spawn.X, spawn.Y + i));

            posits.AddRange(posits.ConvertAll(p => new IntVector2(p.X - 1, p.Y)));
            posits.AddRange(posits.ConvertAll(p => new IntVector2(p.X + 1, p.Y)));

            return posits;
        }

        protected override IEnumerable<IntVector2> CompoutePositions(GridInfo gi)
        {
            var posits = new List<IntVector2>();
            var rnd = new Random();
            var safeZone = GetSafeZone(gi);

            for (int x = 1; x < gi.Max.X; x++)
            {
                for (int y = 1; y < gi.Max.Y; y++)
                {
                    if (safeZone.Contains(new(x, y))) continue;

                    if (posits.Contains(new(x - 1, y))) continue;
                    if (posits.Contains(new(x - 1, y - 1))) continue;
                    if (posits.Contains(new(x, y - 1))) continue;

                    if (rnd.NextDouble() < 0.2) posits.Add(new(x, y));
                }
            }

            return posits;
        }
    }
}
