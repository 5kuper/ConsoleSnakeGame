using ConsoleSnakeGame.Core;

var settings = new Settings() { GridWidth = 10, GridHeight = 10 };
var game = new Game(settings);

game.Start();
await game;