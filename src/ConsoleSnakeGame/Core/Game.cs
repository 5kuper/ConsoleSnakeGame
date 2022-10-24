using System.Runtime.CompilerServices;
using ConsoleSnakeGame.Core.Entities;
using ConsoleSnakeGame.Core.Rendering;
using ConsoleSnakeGame.Core.Scenes;
using Utilities.Numerics;

namespace ConsoleSnakeGame.Core
{
    internal record Game(Settings Settings)
    {
        public enum Status { Win, Loss }

        public record Result(Status Status, int Score);

        private Action? _sceneTerminator;
        private Task<Result>? _task;

        public void Start()
        {
            if (_task is { IsCompleted: false })
            {
                throw new InvalidOperationException("The game is already running.");
            }

            var grid = new Grid(Settings.GridWidth, Settings.GridHeight);
            var position = new IntVector2(grid.Width / 2, grid.Height / 2);
            var snake = new Snake(position, Settings.InitialSnakeGrowth, Settings.FinalSnakeGrowth);

            var scene = new Grassland(new(Settings.TickRate, grid, snake), out var snakeController);
            _sceneTerminator = scene.Terminate;
            InitiateRendering(scene);

            var input = new UserInput(snakeController);
            var cts = new CancellationTokenSource();

            var inputTask = input.HandleAsync(cts.Token);
            _task = ProcessAsync(scene, snake);

            _task.ContinueWith(_ => cts.Cancel());
            inputTask.ContinueWith(_ => cts.Dispose());
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
        }

        private void InitiateRendering(IRenderable target)
        {
            Console.CursorVisible = false;

            List<RenderingRule<ConsoleColor>> colorRules = new(Settings.SnakeColorRules)
            {
                RenderingRules.FoodColorRule, RenderingRules.ObstacleColorRule
            };

            colorRules.Insert(0, RenderingRules.CrashColorRule);

            List<RenderingRule<char>> characterRules = new(RenderingRules.SnakeCharacterRules)
            {
                RenderingRules.FoodCharacterRule, RenderingRules.ObstacleCharacterRule
            };

            var renderer = new TextRenderer(characterRules, colorRules);
            renderer.SetTarget(target);
        }

        private async Task<Result> ProcessAsync(Grassland scene, Snake snake)
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
            var score = snake.Growth - Settings.InitialSnakeGrowth;

            return new(status, score);
        }
    }
}
