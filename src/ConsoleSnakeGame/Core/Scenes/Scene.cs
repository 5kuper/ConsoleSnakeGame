namespace ConsoleSnakeGame.Core.Scenes
{
    internal interface IRenderable
    {
        public event EventHandler? Updated;

        public IReadOnlyGrid Grid { get; }
    }

    internal record PauseToggledEventArgs(bool Value);

    internal abstract class Scene : IRenderable
    {
        private bool _isPaused;

        protected Scene(Grid grid)
        {
            EditableGrid = grid ?? throw new ArgumentNullException(nameof(grid));
        }

        public event EventHandler? Updated;

        public event EventHandler<PauseToggledEventArgs>? PauseToggled;

        public bool IsPaused
        {
            get => _isPaused;
            set
            {
                bool isToggling = _isPaused != value;
                _isPaused = value;

                if (isToggling)
                    OnPauseToggled(new(value));
            }
        }

        public IReadOnlyGrid Grid => EditableGrid;
        protected Grid EditableGrid { get; }

        public void SetPause(bool value) => IsPaused = value;
        public void TogglePause() => IsPaused = !IsPaused;

        public abstract void Terminate();

        protected abstract void Update();

        protected virtual void OnUpdated(EventArgs e)
        {
            Updated?.Invoke(this, e);
        }

        protected virtual void OnPauseToggled(PauseToggledEventArgs e)
        {
            PauseToggled?.Invoke(this, e);
        }
    }
}
