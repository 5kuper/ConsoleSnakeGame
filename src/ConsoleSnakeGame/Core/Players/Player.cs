using System.ComponentModel;
using ConsoleSnakeGame.Core.Entities;
using ConsoleSnakeGame.Core.Scenes;
using Utilities.Numerics;
using Utilities.Terminal;

namespace ConsoleSnakeGame.Core.Players
{
    internal abstract class Player
    {
        public record struct Environment(Scene Scene, ConsoleInput Input);

        protected Controller CharacterController { get; private set; } = null!;

        public void Activate(Environment env, Controller charCtrl)
        {
            if (env.Scene is null)
                throw new ArgumentException("Scene cannot be null.", nameof(env));

            if (env.Input is null)
                throw new ArgumentException("Input cannot be null.", nameof(env));

            CharacterController = charCtrl ?? throw new ArgumentNullException(nameof(charCtrl));
            OnActivated(env);
        }

        protected abstract void OnActivated(Environment env);

        public class Controller
        {
            public enum Direction { Up, Down, Left, Right }

            public record DirectedEventArgs(IntVector2 Direction);

            public static IReadOnlyDictionary<IntVector2, Direction?> Directions
                = new Dictionary<IntVector2, Direction?>()
            {
                { IntVector2.Up, Direction.Up }, { IntVector2.Down, Direction.Down },
                { IntVector2.Left, Direction.Left }, { IntVector2.Right, Direction.Right }
            };

            public Controller(IUnit subject, EventHandler<DirectedEventArgs> directedHandler)
            {
                Subject = subject ?? throw new ArgumentNullException(nameof(subject));
                Directed = directedHandler ?? throw new ArgumentNullException(nameof(directedHandler));
            }

            public event EventHandler<DirectedEventArgs> Directed;

            public IUnit Subject { get; }

            public void Direct(Direction dir)
            {
                var vector = Directions.FirstOrDefault(d => d.Value == dir).Key;

                if (vector == default)
                {
                    throw new InvalidEnumArgumentException(nameof(dir), (int)dir, dir.GetType());
                }

                OnDirected(new(vector));
            }

            protected virtual void OnDirected(DirectedEventArgs e)
            {
                Directed?.Invoke(this, e);
            }
        }
    }
}
