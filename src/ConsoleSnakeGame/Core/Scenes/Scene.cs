using System.Diagnostics;

namespace ConsoleSnakeGame.Core.Scenes
{
    internal interface IRenderable
    {
        public event EventHandler? Updated;
        public IReadOnlyGrid Grid { get; }
    }

    internal abstract class Scene<TConclusion> : IRenderable, IDisposable, IAsyncDisposable
        where TConclusion : struct
    {
        private readonly CancellationTokenSource _cts = new();
        private readonly Stopwatch _stopwatch = new();
        private readonly int _tickRate;

        private Task<TConclusion>? _task;
        private TConclusion? _conclusion;
        private bool _isDisposed;

        protected Scene(int tickRate, Grid grid)
        {
            _tickRate = tickRate > 0 ? tickRate : throw
                new ArgumentOutOfRangeException(nameof(tickRate), "Tick rate must be greater than zero.");

            EditableGrid = grid ?? throw new ArgumentNullException(nameof(grid));
        }

        public event EventHandler? Updated;

        public bool IsPaused { get; set; }
        protected Grid EditableGrid { get; }

        public IReadOnlyGrid Grid => EditableGrid;

        public async Task<TConclusion> PlayAsync()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().FullName);

            _task ??= Task.Run(Loop, _cts.Token);
            return await _task;
        }

        public void Terminate()
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

        protected abstract void Update();

        protected void Conclude(TConclusion conclusion)
        {
            _conclusion = conclusion;
        }

        private TConclusion Loop()
        {
            var period = TimeSpan.FromSeconds(1.0 / _tickRate);

            while (_conclusion is null)
            {
                _cts.Token.ThrowIfCancellationRequested();
                if (IsPaused) continue;

                _stopwatch.Restart();

                Update();
                OnUpdated(EventArgs.Empty);

                var delay = period - _stopwatch.Elapsed;

                if (delay > TimeSpan.Zero)
                    Thread.Sleep(delay);
            }

            return _conclusion.Value;
        }

        protected virtual void OnUpdated(EventArgs e)
        {
            Updated?.Invoke(this, e);
        }
    }
}
