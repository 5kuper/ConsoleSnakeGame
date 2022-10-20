using ConsoleSnakeGame.Core;

var rules = new Rules();
var game = new Game(rules);

game.Start();
await game;