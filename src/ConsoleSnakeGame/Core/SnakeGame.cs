using ConsoleSnakeGame.Core.Entities;
using ConsoleSnakeGame.Core.Players;
using ConsoleSnakeGame.Core.Rendering;
using ConsoleSnakeGame.Core.Scenes;
using Utilities.Numerics;
using Utilities.Terminal;

namespace ConsoleSnakeGame.Core
{
    internal abstract partial class SnakeGame : Game
    {
        public Settings Sets { get; init; }
    }

    internal class SnakeGame<TPlayer> : SnakeGame where TPlayer : Player, new()
    {
        public SnakeGame(Settings settings) => Sets = settings;

        protected override void Init(out Scene scene, out Task<Result> process)
        {
            var grassland = new Grassland(GetGrasslandCtorArgs(), out var snakeController);
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

        private Grassland.CtorArgs GetGrasslandCtorArgs()
        {
            var grid = new Grid(Sets.GridWidth, Sets.GridHeight);
            var obstaclePositions = GetObstaclePositions();

            if (obstaclePositions is not null)
            {
                var obstacles = new Entity(UnitKind.Obstacle, obstaclePositions.ToArray());
                grid.AddEntity(obstacles);
            }

            var snakePosition = new IntVector2(grid.Width / 2, grid.Height / 2);
            var snake = new Snake(snakePosition, Sets.InitialSnakeGrowth, Sets.FinalSnakeGrowth);

            return new(Sets.TickRate, grid, snake);
        }

        private void InitiateRendering(Grassland scene)
        {
            Console.Clear();

            List<RenderingRule<ConsoleColor>> colorRules = new(GetSnakeColorRules())
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

            string GetGrowthStr() => Sets.FinalSnakeGrowth is not null
                ? scene.Snake.Growth.ToString() + $"/{Sets.FinalSnakeGrowth}"
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

            var score = scene.Snake.Growth - Sets.InitialSnakeGrowth;

            return new(status, score);
        }

        private void UpdateTitle()
        {
            Console.Title = "Snake Game" + (IsPaused ? " | Paused (press enter to continue)" : string.Empty);
        }
    }
}
