using System.Diagnostics;
using ConsoleSnakeGame.Core.Gridwork;

namespace ConsoleSnakeGame.Core.Scenes
{
    internal abstract class Scene<TConclusion> : Scene, IDisposable, IAsyncDisposable
        where TConclusion : struct
    {
        private readonly CancellationTokenSource _cts = new();
        private readonly Stopwatch _stopwatch = new();
        private readonly int _tickRate;

        private Task<TConclusion>? _task;
        private TConclusion? _conclusion;
        private bool _isDisposed;

        protected Scene(int tickRate, Grid grid) : base(grid)
        {
            _tickRate = tickRate > 0 ? tickRate : throw
                new ArgumentOutOfRangeException(nameof(tickRate), "Tick rate must be greater than zero.");
        }

        public async Task<TConclusion> PlayAsync()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().FullName);

            _task ??= Task.Run(Loop, _cts.Token);
            return await _task;
        }

        public override void Terminate()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().FullName);

            _cts.Cancel();
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _cts.Cancel();

            try
            {
                _task?.Wait();
            }
            catch (AggregateException a)
            {
                a.Handle(e => e is OperationCanceledException);
            }
            finally
            {
                _cts.Dispose();
                _task?.Dispose();
                _isDisposed = true;
            }
        }

        public async ValueTask DisposeAsync()
        {
            await Task.Run(() => Dispose());
        }

        protected void Conclude(TConclusion conclusion)
        {
            _conclusion = conclusion;
        }

        private TConclusion Loop()
        {
            var period = TimeSpan.FromSeconds(1.0 / _tickRate);

            Notify();

            while (_conclusion is null)
            {
                _cts.Token.ThrowIfCancellationRequested();
                if (IsPaused) continue;

                _stopwatch.Restart();

                Update();
                Notify();
            }

            return _conclusion.Value;

            void Notify()
            {
                OnUpdated(EventArgs.Empty);

                var delay = period - _stopwatch.Elapsed;

                if (delay > TimeSpan.Zero)
                    Thread.Sleep(delay);
            }
        }
    }
}
