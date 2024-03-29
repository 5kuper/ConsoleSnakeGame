﻿using Utilities.Terminal;

namespace ConsoleSnakeGame.Core.Players
{
    internal class UserPlayer : Player
    {
        protected override void OnActivated(Environment env)
        {
            env.Input.KeyUnhandled += Input_KeyUnhandled;
        }

        private void Input_KeyUnhandled(object? sender, KeyEventArgs e)
        {
            switch (e.Info.Key)
            {
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    CharacterController.Direct(Controller.Direction.Up);
                    break;
                case ConsoleKey.A:
                case ConsoleKey.LeftArrow:
                    CharacterController.Direct(Controller.Direction.Left);
                    break;
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    CharacterController.Direct(Controller.Direction.Down);
                    break;
                case ConsoleKey.D:
                case ConsoleKey.RightArrow:
                    CharacterController.Direct(Controller.Direction.Right);
                    break;
            }
        }
    }
}
