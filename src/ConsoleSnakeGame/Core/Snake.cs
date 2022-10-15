using Utilities.Numerics;

namespace ConsoleSnakeGame
{
    internal class Snake : Entity
    {
        public record AteFoodEventArgs(bool IsSnakeSatisfied);

        public const string CrashTag = "crash";

        public const string HeadTag = "head";
        public const string TailTag = "tail";

        private const int BodyIndex = 1; // Units next to head

        private readonly int _initialGrowth;
        private readonly int? _finalGrowth;

        private readonly Unit _head;

        public event EventHandler<AteFoodEventArgs>? AteFood;
        public event EventHandler<EventArgs>? Crashed;

        public IUnit Head => _head;

        public int Growth => Units.Count;
        public int Score => Growth - _initialGrowth;

        public Snake(IntVector2 position, int initialGrowth, int? finalGrowth = null)
        {
            _initialGrowth = initialGrowth >= 2 ? initialGrowth : throw
                new ArgumentOutOfRangeException(nameof(initialGrowth), "Growth cannot be less than two (head and tail).");

            _finalGrowth = finalGrowth > initialGrowth ? finalGrowth : throw
                new ArgumentOutOfRangeException(nameof(finalGrowth), "Final growth must be greater than the initial one.");

            _head = CreateUnit(UnitKind.Snake, position, HeadTag);

            for (var i = 1; i < initialGrowth; i++)
            {
                CreateUnit(UnitKind.Snake, new IntVector2(position.X, ++position.Y));
            }

            var tail = Units.Last();
            tail.Tags.Add(TailTag);
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
            var tail = Units.First(u => u.Tags.Contains(TailTag));
            tail.Tags.Remove(TailTag);

            tail.Position = _head.Position;
            ChangeUnitIndex(tail, BodyIndex);
            _head.Position = position;

            var nowTail = Units.Last();
            nowTail.Tags.Add(TailTag);
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
