namespace ConsoleSnakeGame.Core
{
    internal record Rules
    {
        public int TickRate { get; set; } = 5;

        public int GridWidth { get; set; } = Console.WindowWidth;
        public int GridHeight { get; set; } = Console.WindowHeight;

        public int InitialSnakeGrowth { get; set; } = 3;

        public int? FinalSnakeGrowth { get; set; }

        public Rules()
        {
            FinalSnakeGrowth = GridWidth * GridHeight;
        }
    }
}
