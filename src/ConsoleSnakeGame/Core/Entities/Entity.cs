using System.Collections;
using Utilities.Numerics;

namespace ConsoleSnakeGame.Core.Entities
{
    internal enum UnitKind { Snake, Food, Obstacle }

    internal class Entity : IEnumerable<IUnit>
    {
        public Entity(UnitKind kind, params IntVector2[] positions)
        {
            ArgumentNullException.ThrowIfNull(positions, nameof(positions));
            Array.ForEach(positions, p => CreateUnit(kind, p));
        }

        protected Entity() { }

        public event EventHandler? Cleared;

        protected List<Unit> Units { get; } = new();

        public void Clear()
        {
            Units.Clear();
            OnCleared(EventArgs.Empty);
        }

        protected Unit CreateUnit(UnitKind kind, IntVector2 position, params string[] tags)
        {
            ArgumentNullException.ThrowIfNull(tags, nameof(tags));

            var unit = new Unit(kind, position, Unit_Destroying);
            unit.Tags.AddRange(tags);
            Units.Add(unit);

            return unit;
        }

        protected virtual void OnCleared(EventArgs e)
        {
            Cleared?.Invoke(this, e);
        }

        private void Unit_Destroying(object? sender, EventArgs e)
        {
            Units.Remove((Unit)sender!);
            if (Units.Count == 0) OnCleared(EventArgs.Empty);
        }

        public IEnumerator<IUnit> GetEnumerator() => Units.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        protected class Unit : IUnit
        {
            public Unit(UnitKind kind, IntVector2 position, EventHandler<EventArgs> destroyingHandler)
            {
                Kind = kind;
                Position = position;
                Destroying = destroyingHandler;
            }

            public event EventHandler<EventArgs> Destroying;

            public UnitKind Kind { get; set; }
            public List<string> Tags { get; } = new();
            public IntVector2 Position { get; set; }

            IReadOnlyCollection<string> IUnit.Tags => Tags;

            public void Destroy() => OnDestroying(EventArgs.Empty);

            public override string ToString()
            {
                var tags = Tags.Count > 0 ? $" ({string.Join(", ", Tags)})" : string.Empty;
                return Kind.ToString() + tags;
            }

            protected virtual void OnDestroying(EventArgs e)
            {
                Destroying?.Invoke(this, e);
            }
        }
    }

    internal interface IUnit
    {
        public UnitKind Kind { get; }
        public IReadOnlyCollection<string> Tags { get; }
        public IntVector2 Position { get; }
    }
}
