using System.Diagnostics;

namespace ConsoleSnakeGame.Core
{
    internal class UserInput
    {
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        private readonly Controller _controller;
        private readonly Action _pauseToggle;

        private bool _isTaskRunnig;

        public UserInput(Controller controller, Action pauseTogle)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _pauseToggle = pauseTogle ?? throw new ArgumentNullException(nameof(pauseTogle));
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
            while (!cancellationToken.IsCancellationRequested)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        _controller.Direct(Direction.Up);
                        break;
                    case ConsoleKey.DownArrow:
                        _controller.Direct(Direction.Down);
                        break;
                    case ConsoleKey.LeftArrow:
                        _controller.Direct(Direction.Left);
                        break;
                    case ConsoleKey.RightArrow:
                        _controller.Direct(Direction.Right);
                        break;
                    case ConsoleKey.Enter:
                        if (_stopwatch.ElapsedMilliseconds > 500)
                        {
                            _pauseToggle();
                            _stopwatch.Restart();
                        }
                    break;
                }
            }

            _isTaskRunnig = false;
            cancellationToken.ThrowIfCancellationRequested();
        }
    }
}
