using Utilities.Numerics;

namespace ConsoleSnakeGame
{
    internal interface IReadOnlyGrid
    {
        public int Width { get; }
        public int Height { get; }

        public IUnit? this[IntVector2 position] { get; }

        public IntVector2 GetNextPosition(IntVector2 direction, IntVector2 point);
    }

    internal class Grid : IReadOnlyGrid
    {
        private readonly List<Entity> _entities = new();

        public Grid(int width, int height)
        {
            Width = width > 0 ? width : throw
                new ArgumentOutOfRangeException(nameof(width), "Width must be greater than zero.");

            Height = height > 0 ? height : throw
                new ArgumentOutOfRangeException(nameof(height), "Height must be greater than zero.");
        }

        public int Width { get; }
        public int Height { get; }

        public IReadOnlyCollection<Entity> Entities => _entities;

        public IUnit? this[IntVector2 position]
        {
            get
            {
                bool IsMatching(IUnit u) => u.Position == position;
                var entity = _entities.Find(e => e.Any(IsMatching));
                return entity?.First(IsMatching);
            }
        }

        public void AddEntity(Entity entity)
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));
            entity.Cleared += (sender, _) => _entities.Remove((Entity)sender!);
            _entities.Add(entity);
        }

        public IntVector2 GetNextPosition(IntVector2 direction, IntVector2 point)
        {
            var absDir = IntVector2.Abs(direction);

            if (absDir.X > 1 || absDir.Y > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(direction), "Direction values must be in range from -1 to 1.");
            }

            if (point.X < 0 || point.X >= Width)
            {
                throw new ArgumentOutOfRangeException(nameof(point),
                    $"The X must be greater than zero and less than the width of the grid ({Width}).");
            }
            if (point.Y < 0 || point.Y >= Height)
            {
                throw new ArgumentOutOfRangeException(nameof(point),
                    $"The Y must be greater than zero and less than the height of the grid ({Height}).");
            }

            var position = direction + point;

            if (position.X > Width) position.X -= Width;
            if (position.Y > Height) position.Y -= Height;

            return position;
        }
    }
}
