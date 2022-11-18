using System.Runtime.CompilerServices;
using ConsoleSnakeGame.Core.Entities;
using ConsoleSnakeGame.Core.Players;
using ConsoleSnakeGame.Core.Rendering;
using ConsoleSnakeGame.Core.Scenes;
using Utilities.Numerics;
using Utilities.Terminal;
using Timer = System.Timers.Timer;

namespace ConsoleSnakeGame.Core
{
    internal class SnakeGame<TPlayer> : IGame where TPlayer : Player, new()
    {
        private readonly Timer _timer = new(1000) { AutoReset = true };

        private Grassland? _scene;
        private Task<IGame.Result>? _task;

        public SnakeGame(Settings settings)
        {
            Settings = settings;

            _timer.Elapsed += (_, _)
                => Time += TimeSpan.FromMilliseconds(_timer.Interval);
        }

        public Settings Settings { get; init; }
        public TimeSpan Time { get; private set; }

        IGame.ISettings IGame.Settings => Settings;

        public bool IsPaused
        {
            get => _scene is not null ? _scene.IsPaused : throw NotStartedException;
            set
            {
                if (_scene is null) throw NotStartedException;

                _scene.IsPaused = value;
                _timer.Enabled = !value;

                UpdateTitle();
            }
        }

        private static Exception NotStartedException =>
            new InvalidOperationException("The game is not started.");

        public void Start()
        {
            if (_task is { IsCompleted: false })
            {
                throw new InvalidOperationException("The game is already running.");
            }

            _timer.Start();

            var grid = new Grid(Settings.GridWidth, Settings.GridHeight);
            var position = new IntVector2(grid.Width / 2, grid.Height / 2);
            var snake = new Snake(position, Settings.InitialSnakeGrowth, Settings.FinalSnakeGrowth);

            _scene = new Grassland(new(Settings.TickRate, grid, snake), out var snakeController);
            InitiateRendering(_scene);
            UpdateTitle();

            var input = GetInputHandler();
            var player = new TPlayer();
            player.Activate(new(_scene, input), snakeController);
            var cts = new CancellationTokenSource();

            var inputTask = input.HandleAsync(cts.Token);
            _task = ProcessAsync(_scene);

            _task.ContinueWith(_ => cts.Cancel());
            inputTask.ContinueWith(_ => cts.Dispose());
        }

        public TaskAwaiter<IGame.Result> GetAwaiter()
        {
            if (_task is null)
            {
                return Task.FromException<IGame.Result>(NotStartedException).GetAwaiter();
            }

            return _task.GetAwaiter();
        }

        public void Stop()
        {
            _scene?.Terminate();
            _scene = null;

            _timer.Stop();
            Time = default;
        }

        private void InitiateRendering(Grassland scene)
        {
            Console.Clear();

            List<RenderingRule<ConsoleColor>> colorRules = new(Settings.SnakeColorRules)
            { RenderingRules.FoodColorRule, RenderingRules.ObstacleColorRule };

            colorRules.Insert(0, RenderingRules.CrashColorRule);

            List<RenderingRule<char>> characterRules = new(RenderingRules.SnakeCharacterRules)
            { RenderingRules.FoodCharacterRule, RenderingRules.ObstacleCharacterRule };

            var timeInfo = new InfoPanel.Item { Value = Time };
            _timer.Elapsed += (_, _) => timeInfo.Value = Time;

            var growthInfo = new InfoPanel.NamedItem("Growth") { Value = GetGrowthStr() };
            scene.Snake.AteFood += (_, _) => growthInfo.Value = GetGrowthStr();

            var renderer = new TextRenderer(characterRules, colorRules)
            { InfoPanel = new(timeInfo, growthInfo) };

            renderer.ErrorOccurred += (_, _) => IsPaused = true;
            renderer.SetTarget(scene);

            string GetGrowthStr() => Settings.FinalSnakeGrowth is not null
                ? scene.Snake.Growth.ToString() + $"/{Settings.FinalSnakeGrowth}"
                : scene.Snake.Growth.ToString();
        }

        private async Task<IGame.Result> ProcessAsync(Grassland scene)
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

            var status = conclusion is Grassland.Conclusion.SnakeSatisfied ?
                            IGame.Status.Win : IGame.Status.Loss;

            var score = scene.Snake.Growth - Settings.InitialSnakeGrowth;

            return new(status, score);
        }

        private void UpdateTitle()
        {
            Console.Title = "ConsoleSnakeGame"
                + (IsPaused ? " | Paused (press enter to continue)" : string.Empty);
        }

        private ConsoleInput GetInputHandler()
        {
            void SetPause(bool value) => IsPaused = value;

            void TogglePause() => IsPaused = !IsPaused;

            return new ConsoleInput(SetPause, TogglePause, _scene!.Terminate)
            {
                ProgramName = "snake game"
            };
        }
    }
}
