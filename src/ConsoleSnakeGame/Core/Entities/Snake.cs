using Utilities.Numerics;

namespace ConsoleSnakeGame.Core.Entities
{
    internal class Snake : Entity
    {
        public record AteFoodEventArgs(bool IsSnakeSatisfied);

        public const string HeadTag = "head";
        public const string CrashTag = "crash";

        private const int BodyIndex = 1; // Units next to head

        public static readonly IReadOnlyDictionary<IntVector2, string> TailTags
            = new Dictionary<IntVector2, string>()
        {
            { IntVector2.Up, "tail-up" }, { IntVector2.Down, "tail-down" },
            { IntVector2.Left, "tail-left" }, { IntVector2.Right, "tail-right" }
        };

        private readonly int? _finalGrowth;
        private readonly Unit _head;

        public event EventHandler<AteFoodEventArgs>? AteFood;
        public event EventHandler? Crashed;

        public IUnit Head => _head;
        public int Growth => Units.Count;

        public Snake(IntVector2 position, int initialGrowth, int? finalGrowth = null)
        {
            if (initialGrowth < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(initialGrowth), "Growth cannot be less than two (head and tail).");
            }
            if (finalGrowth != null)
            {
                _finalGrowth = finalGrowth > initialGrowth ? finalGrowth : throw
                    new ArgumentOutOfRangeException(nameof(finalGrowth), "Final growth must be greater than the initial one.");
            }

            _head = CreateUnit(UnitKind.Snake, position, HeadTag);

            for (var i = 1; i < initialGrowth; i++)
            {
                CreateUnit(UnitKind.Snake, new IntVector2(position.X, ++position.Y));
            }

            DefineTail();
        }

        public void MoveTo(IUnit target)
        {
            ArgumentNullException.ThrowIfNull(target, nameof(target));
            var unit = (Unit)target;

            if (unit.Kind is UnitKind.Food)
            {
                Eat(unit);
            }
            else
            {
                CrashInto(unit);
            }
        }

        public void MoveTo(IntVector2 position)
        {
            var tail = Units.Last();
            tail.Tags.RemoveAll(t => TailTags.Any(p => p.Value == t));
            
            tail.Position = _head.Position;
            ChangeUnitIndex(tail, BodyIndex);
            _head.Position = position;

            DefineTail();
        }

        private void Eat(Unit food)
        {
            var newUnit = CreateUnit(UnitKind.Snake, _head.Position);
            ChangeUnitIndex(newUnit, BodyIndex);
            _head.Position = food.Position;

            food.Destroy();

            bool isSatisfied = Growth == _finalGrowth;
            OnAteFood(new(isSatisfied));
        }

        private void CrashInto(Unit causeOfPain)
        {
            causeOfPain.Tags.Add(CrashTag);
            OnCrashed(EventArgs.Empty);
        }

        private void DefineTail()
        {
            var nowTail = Units.Last();
            var nextPos = Units[^2].Position;
            var tailDirection = new IntVector2();

            if (nowTail.Position.X == nextPos.X)
            {
                tailDirection = nowTail.Position.Y < nextPos.Y ? IntVector2.Up : IntVector2.Down;
            }
            else if (nowTail.Position.Y == nextPos.Y)
            {
                tailDirection = nowTail.Position.X < nextPos.X ? IntVector2.Left : IntVector2.Right;
            }

            if (IntVector2.DistanceSquared(nowTail.Position, nextPos) > 1)
            {
                // The tail and the unit next to it are on opposite edges of a grid
                tailDirection = -tailDirection;
            }

            nowTail.Tags.Add(TailTags[tailDirection]);
        }

        private void ChangeUnitIndex(Unit unit, int index)
        {
            Units.Remove(unit);
            Units.Insert(index, unit);
        }

        protected virtual void OnAteFood(AteFoodEventArgs e)
        {
            AteFood?.Invoke(this, e);
        }

        protected virtual void OnCrashed(EventArgs e)
        {
            Crashed?.Invoke(this, e);
        }
    }
}
