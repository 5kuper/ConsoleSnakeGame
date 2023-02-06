using ConsoleSnakeGame.Core.Scenes;
using System.Runtime.CompilerServices;
using Timer = System.Timers.Timer;

namespace ConsoleSnakeGame.Core
{
    internal abstract class Game
    {
        public enum Status { Win, Loss }

        public record Result(Status Status, int Score);

        protected Game()
        {
            Timer.Elapsed += (_, _) => Time += TimeSpan.FromMilliseconds(Timer.Interval);
        }

        public TimeSpan Time { get; private set; }

        public bool IsPaused
        {
            get => Scene?.IsPaused == true;
            set
            {
                if (Scene is null) throw NotStartedException;
                Scene.IsPaused = value;
            }
        }

        protected Timer Timer { get; } = new(1000) { AutoReset = true };

        protected Scene? Scene { get; private set; }
        protected Task<Result>? Process { get; private set; }

        private static Exception NotStartedException =>
            new InvalidOperationException("The game is not started.");

        public void Start()
        {
            if (Process is { IsCompleted: false })
            {
                throw new InvalidOperationException("The game is already running.");
            }

            Timer.Start();
            Init(out var scene, out var process);

            Scene = scene ?? throw new InvalidOperationException("Scene cannot be null");
            Process = process ?? throw new InvalidOperationException("Process cannot be null");

            Scene.PauseToggled += (_, e) => Timer.Enabled = !e.Value;
        }

        public TaskAwaiter<Result> GetAwaiter()
        {
            return Process is not null ?
                Process.GetAwaiter() : Task.FromException<Result>(NotStartedException).GetAwaiter();
        }

        public void Stop()
        {
            Scene?.Terminate();
            Scene = null;

            Timer.Stop();
            Time = default;
        }

        protected abstract void Init(out Scene scene, out Task<Result> process);
    }
}
