namespace ConsoleSnakeGame
{
    internal interface IRenderable
    {
        public event EventHandler? Updated;
        public IReadOnlyGrid Grid { get; }
    }

    internal abstract class Scene<TConclusion> : IRenderable, IDisposable
    {
        private readonly CancellationTokenSource _cts = new();
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
            _cts.Dispose();
            _isDisposed = true;
        }

        protected abstract void Update();

        protected void Conclude(TConclusion conclusion)
        {
            _conclusion = conclusion ?? throw new ArgumentNullException(nameof(conclusion));
        }

        private TConclusion Loop()
        {
            var delay = TimeSpan.FromSeconds(1 / _tickRate);

            while (_conclusion is null)
            {
                _cts.Token.ThrowIfCancellationRequested();
                if (IsPaused) continue;

                Update();
                OnUpdated(EventArgs.Empty);

                Thread.Sleep(delay);
            }

            return _conclusion;
        }

        protected virtual void OnUpdated(EventArgs e)
        {
            Updated?.Invoke(this, e);
        }
    }
}
