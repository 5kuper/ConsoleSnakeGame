using ConsoleSnakeGame.Core.Entities;
using ConsoleSnakeGame.Core.Players;
using ConsoleSnakeGame.Core.Rendering;
using ConsoleSnakeGame.Core.Scenes;
using Utilities.Numerics;
using Utilities.Terminal;

namespace ConsoleSnakeGame.Core
{
    internal abstract partial class SnakeGame : Game { }

    internal class SnakeGame<TPlayer> : SnakeGame where TPlayer : Player, new()
    {
        private readonly Settings _settings;

        public SnakeGame(Settings settings) => _settings = settings;

        protected override void Init(out Scene scene, out Task<Result> process)
        {
            var grid = new Grid(_settings.GridWidth, _settings.GridHeight);
            var position = new IntVector2(grid.Width / 2, grid.Height / 2);
            var snake = new Snake(position, _settings.InitialSnakeGrowth, _settings.FinalSnakeGrowth);

            var grassland = new Grassland(new(_settings.TickRate, grid, snake), out var snakeController);
            InitiateRendering(grassland);

            scene = grassland;

            scene.PauseToggled += (_, _) => UpdateTitle();
            UpdateTitle();

            var input = new ConsoleInput(scene.SetPause, scene.TogglePause, scene.Terminate)
                                                { ProgramName = "snake game" };

            var player = new TPlayer();
            player.Activate(new(scene, input), snakeController);

            var cts = new CancellationTokenSource();

            var inputTask = input.HandleAsync(cts.Token);
            process = ProcessAsync(grassland);

            process.ContinueWith(_ => cts.Cancel());
            inputTask.ContinueWith(_ => cts.Dispose());
        }

        private void InitiateRendering(Grassland scene)
        {
            Console.Clear();

            List<RenderingRule<ConsoleColor>> colorRules = new(_settings.SnakeColorRules)
                { RenderingRules.FoodColorRule, RenderingRules.ObstacleColorRule };

            colorRules.Insert(0, RenderingRules.CrashColorRule);

            List<RenderingRule<char>> characterRules = new(RenderingRules.SnakeCharacterRules)
                { RenderingRules.FoodCharacterRule, RenderingRules.ObstacleCharacterRule };

            var timeInfo = new InfoPanel.Item { Value = Time };
            Timer.Elapsed += (_, _) => timeInfo.Value = Time;

            var growthInfo = new InfoPanel.NamedItem("Growth") { Value = GetGrowthStr() };
            scene.Snake.AteFood += (_, _) => growthInfo.Value = GetGrowthStr();

            var renderer = new TextRenderer(characterRules, colorRules)
                { InfoPanel = new(timeInfo, growthInfo) };

            renderer.ErrorOccurred += (_, _) => IsPaused = true;
            renderer.SetTarget(scene);

            string GetGrowthStr() => _settings.FinalSnakeGrowth is not null
                ? scene.Snake.Growth.ToString() + $"/{_settings.FinalSnakeGrowth}"
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

            var status = conclusion is Grassland.Conclusion.SnakeSatisfied ?
                                            Status.Win : Status.Loss;

            var score = scene.Snake.Growth - _settings.InitialSnakeGrowth;

            return new(status, score);
        }

        private void UpdateTitle()
        {
            Console.Title = "Snake Game" + (IsPaused ? " | Paused (press enter to continue)" : string.Empty);
        }
    }
}
