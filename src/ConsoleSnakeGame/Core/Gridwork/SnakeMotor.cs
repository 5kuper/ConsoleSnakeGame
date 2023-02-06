using ConsoleSnakeGame.Core.Gridwork.Entities;
using Utilities.Numerics;

namespace ConsoleSnakeGame.Core.Gridwork
{
    internal class SnakeMotor
    {
        private const int MaxMoveSkips = 10;

        private readonly Snake _subject;
        private readonly IReadOnlyGrid _grid;

        private Proportion _speed;
        private int _needSkips, _doneSkips;

        public SnakeMotor(Snake subject, IReadOnlyGrid grid, Range<Proportion> speedRange)
        {
            _subject = subject ?? throw new ArgumentNullException(nameof(subject));
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));

            SpeedRange = speedRange.Bottom > 0 ? speedRange : throw
                new ArgumentException("Speed must be greater than zero.", nameof(speedRange));

            Speed = speedRange.Bottom;
        }

        public Range<Proportion> SpeedRange { get; }

        public Proportion Speed
        {
            get => _speed;
            set
            {
                _speed = SpeedRange.Includes(value) ? value : throw
                    new ArgumentException("Value doesn't match the speed range.", nameof(value));

                var slowness = Proportion.OppositeOf(_speed);
                _needSkips = (int)MathF.Round(MaxMoveSkips * slowness);
            }
        }

        public void Process(IntVector2 direction, Action? snakeMovedCallback = null)
        {
            if (_doneSkips != _needSkips)
            {
                _doneSkips++;
            }
            else
            {
                _doneSkips = 0;
                Move();
                snakeMovedCallback?.Invoke();
            }

            void Move()
            {
                var position = _grid.GetNextPosition(direction, _subject.Head.Position);
                var unit = _grid[position];

                if (unit is not null)
                {
                    _subject.MoveTo(unit);
                }
                else
                {
                    _subject.MoveTo(position);
                }
            }
        }
    }
}
