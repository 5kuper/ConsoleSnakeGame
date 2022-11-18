using ConsoleSnakeGame.Core;
using ConsoleSnakeGame.Core.Players;

var settings = new Settings();

Console.Write("Let a bot play? [y/N]: ");

IGame game = Console.ReadKey().Key switch
{
    ConsoleKey.Y => new SnakeGame<BotPlayer>(settings),
    _ => new SnakeGame<UserPlayer>(settings)
};

game.Start();

try
{
    await game;
}
catch (OperationCanceledException)
{
    Console.WriteLine("The game has been canceled :C");
}