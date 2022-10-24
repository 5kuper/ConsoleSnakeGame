using ConsoleSnakeGame.Core.Entities;
using ConsoleSnakeGame.Core.Scenes;
using Utilities.Text;

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
        private ConsoleCanvas _canvas = null!;

        public void SetTarget(IRenderable value)
        {
            void Target_Updated(object? sender, EventArgs e) => Render();

            if (_target != null) _target.Updated -= Target_Updated;

            _target = value;

            // +2 because of frame around the grid
            var canvasWidth = _target.Grid.Width + SpacesPerLine + 2;
            _canvas = new ConsoleCanvas(canvasWidth, _target.Grid.Height + 2);

            _target.Updated += Target_Updated;
        }

        private int SpacesPerLine => _target.Grid.Width + 1;
        private string VerticalLine => new('═', _target.Grid.Width + SpacesPerLine);

        private string UpperBorder => '╔' + VerticalLine + '╗';
        private string LowerBorder => '╚' + VerticalLine + '╝';

        private void Render()
        {
            _canvas.Clear();
            _canvas.WriteLine(UpperBorder);

            for (int y = 0; y < _target.Grid.Height; y++)
            {
                _canvas.Write(HorizontalElement + " ");

                for (int x = 0; x < _target.Grid.Width; x++)
                {
                    var unit = _target.Grid[new(x, y)];

                    if (unit != null) WriteUnit(unit);
                    else _canvas.Write(VoidElement, new(ConsoleColor.DarkGray));

                    _canvas.Write(" ");
                }

                _canvas.WriteLine(HorizontalElement);
            }

            _canvas.Write(LowerBorder);
            _canvas.Display();
        }

        private void WriteUnit(IUnit value)
        {
            var characterRule = CharacterRules.First(r => r.IsSuitableFor(value));
            var colorRule = ColorRules.First(r => r.IsSuitableFor(value));

            _canvas.Write(characterRule.Value, new(colorRule.Value));
        }
    }
}
