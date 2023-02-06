using ConsoleSnakeGame.Core.Gridwork;
using ConsoleSnakeGame.Core.Gridwork.Entities;
using Utilities.Numerics;

namespace ConsoleSnakeGame.Core.Players
{
    internal class BotPlayer : Player
    {
        private IReadOnlyGrid _grid = null!;
        private Stack<IntVector2>? _route;

        protected override void OnActivated(Environment env)
        {
            _grid = env.Scene.Grid;
            CharacterController.SubjectMoved += Character_Moved;
        }

        private IntVector2 SubjPos => CharacterController.Subject.Position;

        private void Character_Moved(object? sender, EventArgs e)
        {
            bool isRouteBuilt = _route is { Count: > 0 } || TryBuildRoute(out _route);

            if (!isRouteBuilt) return;

            var target = _route.Pop();
            var vectorDir = (IntVector2)SubjPos.GetDirectionTo(target);

            if (IntVector2.DistanceSquared(SubjPos, target) > 1)
                vectorDir = -vectorDir; // The subject and the target are on opposite edges of the grid

            var enumDir = Controller.Directions.GetValueOrDefault(vectorDir)
                          ?? throw new InvalidOperationException("Bot couldn't compute a direction correctly.");

            CharacterController.Direct(enumDir, true);
        }

        private bool TryBuildRoute(out Stack<IntVector2> steps)
        {
            steps = new();

            var pos = SearchFoodPosition(out var tree) ?? new(-1);

            while (tree.TryGetValue(pos, out var next))
            {
                // Backtrace
                steps.Push(pos);
                pos = next;
            }

            return steps.Count > 0;
        }

        private IntVector2? SearchFoodPosition(out Dictionary<IntVector2, IntVector2> tree)
        {
            var nodes = tree = new();

            var uncheckedPositions = new Queue<IntVector2>();

            var checkedPositions = new HashSet<IntVector2>()
                { CharacterController.Subject.Position };

            ExpandWith(CharacterController.Subject.Position);

            while (uncheckedPositions.Count > 0)
            {
                var checkingPosition = uncheckedPositions.Dequeue();
                checkedPositions.Add(checkingPosition);

                switch (_grid[checkingPosition])
                {
                    case { Kind: UnitKind.Food }: return checkingPosition;

                    case not null: continue; // Snake couldn't move through it
                }

                ExpandWith(checkingPosition);
            }

            return null;

            void ExpandWith(IntVector2 position)
            {
                var nabrs = _grid.GetNearPositions(position);

                foreach (var pos in nabrs.Where(p => !checkedPositions.Contains(p)))
                {
                    uncheckedPositions.Enqueue(pos);
                    nodes[pos] = position;
                }
            }
        }
    }
}
