using System.ComponentModel;
using ConsoleSnakeGame.Core.Entities;
using Utilities.Numerics;

namespace ConsoleSnakeGame.Core.Players
{
    internal class Player
    {
        protected Controller CharacterController { get; }

        public Player(Controller charCtrl)
        {
            CharacterController = charCtrl ?? throw new ArgumentNullException(nameof(charCtrl));
        }

        public class Controller
        {
            public enum Direction { Up, Down, Left, Right }

            public record DirectedEventArgs(IntVector2 Direction);

            public Controller(IUnit subject, IReadOnlyGrid grid, EventHandler<DirectedEventArgs> directedHandler)
            {
                Subject = subject ?? throw new ArgumentNullException(nameof(subject));
                Grid = grid ?? throw new ArgumentNullException(nameof(grid));
                Directed = directedHandler ?? throw new ArgumentNullException(nameof(directedHandler));
            }

            public event EventHandler<DirectedEventArgs> Directed;

            public IUnit Subject { get; }
            public IReadOnlyGrid Grid { get; }

            public void Direct(Direction dir)
            {
                var vector = dir switch
                {
                    Direction.Up => IntVector2.Up, Direction.Down => IntVector2.Down,
                    Direction.Left => IntVector2.Left, Direction.Right => IntVector2.Right,
                    _ => throw new InvalidEnumArgumentException(nameof(dir), (int)dir, dir.GetType())
                };
                OnDirected(new(vector));
            }

            protected virtual void OnDirected(DirectedEventArgs e)
            {
                Directed?.Invoke(this, e);
            }
        }
    }
}
