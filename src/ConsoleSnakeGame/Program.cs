using ConsoleSnakeGame.Core;

var settings = new Settings();
var game = new Game(settings);

game.Start();
await game;