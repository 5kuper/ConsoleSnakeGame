using ConsoleSnakeGame.Core.Gridwork;
using ConsoleSnakeGame.Core.Gridwork.Entities;
using ConsoleSnakeGame.Core.Players;
using Utilities.Numerics;

namespace ConsoleSnakeGame.Core.Scenes
{
    internal class Grassland : Scene<Grassland.Conclusion>
    {
        public enum Conclusion { SnakeSatisfied, SnakeCrashed }

        public record struct CtorArgs(int TickRate, Grid Grid, Snake Snake, SnakeSpeed SpeedInfo);
        public record struct SnakeSpeed(Range<Proportion> ValueRange, int GrowthForMaxValue);

        private readonly SnakeMotor _snakeMotor;
        private readonly Action _snakeMovedInvokator;
        private readonly int _growthForMaxSpeed;

        private IntVector2 _controllerDirection = IntVector2.Up;
        private IntVector2 _lastUsedDirectoin;

        public Grassland(CtorArgs args, out Player.Controller snakeController)
            : base(args.TickRate, args.Grid)
        {
            Snake = args.Snake ?? throw new ArgumentException("Snake cannot be null.", nameof(args));
            EditableGrid.AddEntity(Snake);

            Snake.AteFood += Snake_AteFood;
            Snake.Crashed += Snake_Crashed;

            snakeController = new(Snake.Head, out _snakeMovedInvokator, (_, e) =>
            {
                if (IsPaused && !e.IsPauseIgnoring) return;
                _controllerDirection = e.Direction;
            });

            _snakeMotor = new(Snake, Grid, args.SpeedInfo.ValueRange);
            _growthForMaxSpeed = args.SpeedInfo.GrowthForMaxValue;

            if (_growthForMaxSpeed < Snake.Growth)
            {
                throw new ArgumentException("Growth for max speed cannot be less than" +
                    " the current snake growth.", nameof(args));
            }

            SpawnFood();
        }

        public Snake Snake { get; }

        protected override void Update()
        {
            var conDir = _controllerDirection; // The field may change in another thread

            if (conDir + _lastUsedDirectoin == IntVector2.Zero)
            {
                // Should not move snake backwards
                _snakeMotor.Process(_lastUsedDirectoin, _snakeMovedInvokator);
            }
            else
            {
                _snakeMotor.Process(conDir, () =>
                {
                    _snakeMovedInvokator();
                    _lastUsedDirectoin = conDir;
                });
            }
        }

        private void SpawnFood()
        {
            var pos = new IntVector2();
            var rnd = new Random();

            do
            {
                pos.X = rnd.Next(Grid.Width);
                pos.Y = rnd.Next(Grid.Height);
            }
            while (Grid[pos] != null);

            var food = new Entity(UnitKind.Food, pos);
            EditableGrid.AddEntity(food);
        }

        private void Snake_AteFood(object? sender, Snake.AteFoodEventArgs e)
        {
            if (e.IsSnakeSatisfied)
            {
                Conclude(Conclusion.SnakeSatisfied);
            }
            else
            {
                if (Snake.Growth <= _growthForMaxSpeed)
                {
                    _snakeMotor.Speed = new Proportion((float) Snake.Growth / _growthForMaxSpeed)
                        .InRange(((float, float))_snakeMotor.SpeedRange).ToProportion();
                }

                SpawnFood();
            }
        }

        private void Snake_Crashed(object? sender, EventArgs e)
        {
            Conclude(Conclusion.SnakeCrashed);
        }
    }
}
