using System.Runtime.CompilerServices;
using ConsoleSnakeGame.Core.Entities;
using ConsoleSnakeGame.Core.Rendering;
using ConsoleSnakeGame.Core.Scenes;
using Utilities.Numerics;
using Timer = System.Timers.Timer;

namespace ConsoleSnakeGame.Core
{
    internal record Game(Settings Settings)
    {
        public enum Status { Win, Loss }

        public record Result(Status Status, int Score);

        private readonly Timer _timer = new(1000) { AutoReset = true };

        private Action? _sceneTerminator;
        private Task<Result>? _task;

        public TimeSpan Time { get; private set; } = new();

        public void Start()
        {
            if (_task is { IsCompleted: false })
            {
                throw new InvalidOperationException("The game is already running.");
            }

            _timer.Start();
            _timer.Elapsed += (_, _) => Time += TimeSpan.FromMilliseconds(_timer.Interval);

            var grid = new Grid(Settings.GridWidth, Settings.GridHeight);
            var position = new IntVector2(grid.Width / 2, grid.Height / 2);
            var snake = new Snake(position, Settings.InitialSnakeGrowth, Settings.FinalSnakeGrowth);

            var scene = new Grassland(new(Settings.TickRate, grid, snake), out var snakeController);
            _sceneTerminator = scene.Terminate;

            InitiateRendering(scene, TogglePause);
            UpdateTitle();

            var input = new UserInput(snakeController, TogglePause);
            var cts = new CancellationTokenSource();

            var inputTask = input.HandleAsync(cts.Token);
            _task = ProcessAsync(scene);

            _task.ContinueWith(_ => cts.Cancel());
            inputTask.ContinueWith(_ => cts.Dispose());

            void TogglePause()
            {
                scene.IsPaused = !scene.IsPaused;
                UpdateTitle();
            }

            void UpdateTitle()
            {
                Console.Title = "ConsoleSnakeGame"
                    + (scene.IsPaused ? " | Paused (press enter to continue)" : string.Empty);
            }
        }

        public TaskAwaiter<Result> GetAwaiter()
        {
            if (_task is null)
            {
                var exception = new InvalidOperationException("The game is not started.");
                return Task.FromException<Result>(exception).GetAwaiter();
            }

            return _task.GetAwaiter();
        }

        public void Stop()
        {
            _sceneTerminator?.Invoke();

            _timer.Stop();
            Time = default;
        }

        private void InitiateRendering(Grassland scene, Action pauseToggle)
        {
            List<RenderingRule<ConsoleColor>> colorRules = new(Settings.SnakeColorRules)
            { RenderingRules.FoodColorRule, RenderingRules.ObstacleColorRule };

            colorRules.Insert(0, RenderingRules.CrashColorRule);

            List<RenderingRule<char>> characterRules = new(RenderingRules.SnakeCharacterRules)
            { RenderingRules.FoodCharacterRule, RenderingRules.ObstacleCharacterRule };

            var timeInfo = new InfoPanel.Item { Value = Time };
            _timer.Elapsed += (_, _) => timeInfo.Value = Time;

            var growthInfo = new InfoPanel.NamedItem("Growth") { Value = GetGrowthStr() };
            scene.Snake.AteFood += (_, _) => growthInfo.Value = GetGrowthStr();

            var renderer = new TextRenderer(characterRules, colorRules) { InfoPanel = new(timeInfo, growthInfo) };

            renderer.ErrorOccurred += (_, _) => pauseToggle();
            renderer.SetTarget(scene);

            string GetGrowthStr() => Settings.FinalSnakeGrowth is not null
                ? scene.Snake.Growth.ToString() + $"/{Settings.FinalSnakeGrowth}"
                : scene.Snake.Growth.ToString();
        }

        private async Task<Result> ProcessAsync(Grassland scene)
        {
            Grassland.Conclusion conclusion;

            try
            {
                conclusion = await scene.PlayAsync();
            }
            catch
            {
                throw;
            }
            finally
            {
                await scene.DisposeAsync();
            }

            var status = conclusion is Grassland.Conclusion.SnakeSatisfied ? Status.Win : Status.Loss;
            var score = scene.Snake.Growth - Settings.InitialSnakeGrowth;

            return new(status, score);
        }
    }
}
