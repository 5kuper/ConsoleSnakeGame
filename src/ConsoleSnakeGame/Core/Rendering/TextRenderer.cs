using ConsoleSnakeGame.Core.Entities;
using ConsoleSnakeGame.Core.Scenes;

namespace ConsoleSnakeGame.Core.Rendering
{
    internal record RenderingRule<T>(T Value, UnitKind? UnitKind = null, params string[] RequiredTags)
    {
        public bool IsSuitableFor(IUnit unit)
        {
            ArgumentNullException.ThrowIfNull(unit, nameof(unit));

            var tags = RequiredTags ?? Array.Empty<string>();
            bool result = tags.All(t => unit.Tags.Contains(t));

            if (UnitKind is not null)
                result = result && unit.Kind == UnitKind;

            return result;
        }
    }

    internal record TextRenderer(IEnumerable<RenderingRule<char>> CharacterRules,
                                 IEnumerable<RenderingRule<ConsoleColor>> ColorRules)
    {
        private const char VoidElement = '·';
        private const char HorizontalElement = '║';

        private IRenderable _target = null!;

        public void SetTarget(IRenderable value)
        {
            void Target_Updated(object? sender, EventArgs e) => Render();

            if (_target != null) _target.Updated -= Target_Updated;

            _target = value;
            _target.Updated += Target_Updated;
        }

        private string VerticalLine => new('═', _target.Grid.Width * 2 + 1);

        private string UpperBorder => '╔' + VerticalLine + '╗';
        private string LowerBorder => '╚' + VerticalLine + '╝';

        private static void WriteColored(char character, ConsoleColor color)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = color;

            Console.Write(character);

            Console.ForegroundColor = previousColor;
        }

        private void Render()
        {
            Console.Clear();
            Console.WriteLine(UpperBorder);

            for (int y = 0; y < _target.Grid.Height; y++)
            {
                Console.Write(HorizontalElement + " ");

                for (int x = 0; x < _target.Grid.Width; x++)
                {
                    var unit = _target.Grid[new(x, y)];

                    if (unit != null) WriteUnit(unit);
                    else WriteColored(VoidElement, ConsoleColor.DarkGray);

                    Console.Write(" ");
                }

                Console.WriteLine(HorizontalElement);
            }

            Console.Write(LowerBorder);
        }

        private void WriteUnit(IUnit value)
        {
            var characterRule = CharacterRules.First(r => r.IsSuitableFor(value));
            var colorRule = ColorRules.First(r => r.IsSuitableFor(value));

            WriteColored(characterRule.Value, colorRule.Value);
        }
    }
}
