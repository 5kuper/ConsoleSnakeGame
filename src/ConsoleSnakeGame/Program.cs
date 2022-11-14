using ConsoleSnakeGame.Core;

var settings = new Settings();
var game = new Game(settings);

game.Start();

try
{
    await game;
}
catch (OperationCanceledException)
{
    Console.WriteLine("The game has been canceled :C");
}