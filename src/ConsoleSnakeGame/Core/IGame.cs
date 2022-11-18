using System.Runtime.CompilerServices;

namespace ConsoleSnakeGame.Core
{
    internal interface IGame
    {
        internal interface ISettings { }

        public enum Status { Win, Loss }
        public record Result(Status Status, int Score);

        public ISettings Settings { get; }
        public TimeSpan Time { get; }

        public void Start();
        public TaskAwaiter<Result> GetAwaiter();
        public void Stop();
    }
}
