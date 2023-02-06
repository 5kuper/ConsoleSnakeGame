using System.ComponentModel;
using ConsoleSnakeGame.Core.Gridwork.Entities;
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

            public record DirectedEventArgs(IntVector2 Direction, bool IsPauseIgnoring);

            public static IReadOnlyDictionary<IntVector2, Direction?> Directions
                = new Dictionary<IntVector2, Direction?>()
            {
                { IntVector2.Up, Direction.Up }, { IntVector2.Down, Direction.Down },
                { IntVector2.Left, Direction.Left }, { IntVector2.Right, Direction.Right }
            };

            private readonly List<EventHandler> _subjectMovedHandlers = new();

            public Controller(IUnit subject, out Action subjectMovedInvokator,
                EventHandler<DirectedEventArgs> directedHandler)
            {
                Subject = subject ?? throw new ArgumentNullException(nameof(subject));
                Directed = directedHandler ?? throw new ArgumentNullException(nameof(directedHandler));

                subjectMovedInvokator = () =>
                {
                    foreach (var handler in _subjectMovedHandlers)
                        handler?.Invoke(this, EventArgs.Empty);
                };
            }

            public event EventHandler<DirectedEventArgs> Directed;

            public event EventHandler SubjectMoved
            {
                add => _subjectMovedHandlers.Add(value);
                remove => _subjectMovedHandlers.Remove(value);
            }

            public IUnit Subject { get; }

            public void Direct(Direction dir, bool ignorePause = false)
            {
                var vector = Directions.FirstOrDefault(d => d.Value == dir).Key;

                if (vector == default)
                {
                    throw new InvalidEnumArgumentException(nameof(dir), (int)dir, dir.GetType());
                }

                OnDirected(new(vector, ignorePause));
            }

            protected virtual void OnDirected(DirectedEventArgs e)
            {
                Directed?.Invoke(this, e);
            }
        }
    }
}
