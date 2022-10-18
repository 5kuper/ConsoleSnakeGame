using Utilities.Numerics;

namespace ConsoleSnakeGame
{
    internal class Grassland : Scene<Grassland.Conclusion>
    {
        public enum Conclusion { SnakeSatisfied, SnakeCrashed }

        public record CtorArgs(int TickRate, Grid Grid, Snake Snake);

        private readonly Snake _snake;
        private IntVector2 _snakeDirection = IntVector2.Up;

        public Grassland(CtorArgs args, out Controller snakeController)
            : base(args.TickRate, args.Grid)
        {
            _snake = args.Snake ?? throw new ArgumentException("Snake cannot be null.", nameof(args));

            _snake.AteFood += Snake_AteFood;
            _snake.Crashed += Snake_Crashed;

            snakeController = new(_snake.Head, Grid, (_, e) => _snakeDirection = e.Direction);

            EditableGrid.AddEntity(_snake);
            SpawnFood();
        }

        protected override void Update() => MoveSnake(_snakeDirection);

        private void MoveSnake(IntVector2 direction)
        {
            var position = Grid.GetNextPosition(direction, _snake.Head.Position);
            var unit = Grid[position];

            if (unit is not null)
            {
                _snake.MoveTo(unit);
            }
            else
            {
                _snake.MoveTo(position);
            }
        }

        private void SpawnFood()
        {
            var pos = new IntVector2();
            var rnd = new Random();

            pos.X = rnd.Next(Grid.Width);
            pos.Y = rnd.Next(Grid.Height);

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
                SpawnFood();
            }
        }

        private void Snake_Crashed(object? sender, EventArgs e)
        {
            Conclude(Conclusion.SnakeCrashed);
        }
    }
}
