﻿using static System.ConsoleColor;
using static ConsoleSnakeGame.Core.Entities.UnitKind;
using static ConsoleSnakeGame.Core.Entities.Snake;
using static Utilities.Numerics.IntVector2;

using ColorRule = ConsoleSnakeGame.Core.Rendering.RenderingRule<System.ConsoleColor>;
using CharacterRule = ConsoleSnakeGame.Core.Rendering.RenderingRule<char>;

namespace ConsoleSnakeGame.Core.Rendering
{
    internal static class RenderingRules
    {
        public static ColorRule CrashColorRule => new(Red, RequiredTags: CrashTag);

        public static CharacterRule ObstacleCharacterRule => new('#', Obstacle);
        public static ColorRule ObstacleColorRule => new(Blue, Obstacle);

        public static CharacterRule FoodCharacterRule => new('@', Food);
        public static ColorRule FoodColorRule => new(Magenta, Food);

        public static CharacterRule[] SnakeCharacterRules => new[]
        {
            new('▲', Snake, TailTags[Up]), new('◄', Snake, TailTags[Left]),
            new('▼', Snake, TailTags[Down]), new('►', Snake, TailTags[Right]),

            new CharacterRule('■', Snake)
        };

        public static ColorRule[] GreenSnakeColorRules => new ColorRule[]
        {
            new(DarkGreen, Snake, HeadTag), new(Green, Snake)
        };

        public static ColorRule[] CyanSnakeColorRules => new ColorRule[]
        {
            new(DarkCyan, Snake, HeadTag), new(Cyan, Snake)
        };

        public static ColorRule[] YellowSnakeColorRules => new ColorRule[]
        {
            new(DarkYellow, Snake, HeadTag), new(Yellow, Snake)
        };
    }
}