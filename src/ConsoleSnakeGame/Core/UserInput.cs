using System.Diagnostics;

namespace ConsoleSnakeGame.Core
{
    public record KeyEventArgs(ConsoleKeyInfo Info);

    internal class UserInput
    {
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        private readonly Action _pauseToggle;
        private readonly Action<bool> _pauseSetter;

        private readonly Action _sceneTerminator;

        private bool _isTaskRunnig;
        private bool _isGameCancelling;

        public event EventHandler<KeyEventArgs>? KeyUnhandled;

        public UserInput(Action pauseTogle, Action<bool> pauseSetter, Action sceneTerminator)
        {
            _pauseToggle = pauseTogle ?? throw new ArgumentNullException(nameof(pauseTogle));
            _pauseSetter = pauseSetter ?? throw new ArgumentNullException(nameof(pauseSetter));

            _sceneTerminator = sceneTerminator ?? throw new ArgumentNullException(nameof(sceneTerminator));
        }

        public async Task HandleAsync(CancellationToken cancellationToken = default)
        {
            if (_isTaskRunnig)
            {
                throw new InvalidOperationException("Cannot run a new task while an existing task is running.");
            }

            var task = Task.Run(() => Handle(cancellationToken), cancellationToken);
            _isTaskRunnig = true;
            await task;
        }

        private void Handle(CancellationToken cancellationToken)
        {
            Console.TreatControlCAsInput = true;

            while (!cancellationToken.IsCancellationRequested)
            {
                var ki = Console.ReadKey(!_isGameCancelling);

                switch (ki)
                {
                    case { Key: ConsoleKey.Y } when _isGameCancelling:
                        _sceneTerminator();
                        Console.WriteLine();
                        break;

                    case { Key: not ConsoleKey.Y } when _isGameCancelling:
                        AvoidGameCancellation();
                        break;

                    case { Key: ConsoleKey.Enter or ConsoleKey.Spacebar }
                            when _stopwatch.ElapsedMilliseconds > 500:
                        _pauseToggle();
                        _stopwatch.Restart();
                        break;

                    case { Key: ConsoleKey.Escape }:
                    case { Modifiers: ConsoleModifiers.Control, Key: ConsoleKey.C }:
                        AskConfirmationOfGameCancellation();
                        break;

                    default:
                        OnKeyUnhandled(new(ki));
                        break;
                }
            }

            _isTaskRunnig = false;
            Console.TreatControlCAsInput = false;
            cancellationToken.ThrowIfCancellationRequested();
        }

        private void AskConfirmationOfGameCancellation()
        {
            lock (Console.Out)
            {
                _pauseSetter(true);

                Console.Clear();
                Console.CursorVisible = true;
                Console.Write("Are you sure you want to cancel the game? [y/N]: ");

                _isGameCancelling = true;
            }
        }

        private void AvoidGameCancellation()
        {
            Thread.Sleep(1000);
            Console.Clear();

            _isGameCancelling = false;
            _pauseSetter(false);
        }

        protected virtual void OnKeyUnhandled(KeyEventArgs e)
        {
            KeyUnhandled?.Invoke(this, e);
        }
    }
}
