using ConsoleSnakeGame.Core.Entities;
using ConsoleSnakeGame.Core.Scenes;
using Utilities.Terminal;

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

        private IScene _target = null!;
        private ConsoleCanvas _canvas = null!;

        public void SetTarget(IScene value)
        {
            void Target_Updated(object? sender, EventArgs e) => Render();

            if (_target != null) _target.Updated -= Target_Updated;

            _target = value;
            _canvas = new ConsoleCanvas(ColumnsNumber, _target.Grid.Height, (3, 1));

            _target.Updated += Target_Updated;
        }

        public event EventHandler? ErrorOccurred;

        public InfoPanel? InfoPanel { get; set; }

        // +2 because of frame around the grid
        private int ColumnsNumber => _target.Grid.Width + SpacesPerLine + 2;

        private int SpacesPerLine => _target.Grid.Width + 1;

        private string VerticalLine => new('═', _target.Grid.Width + SpacesPerLine);

        private string UpperBorder => '╔' + VerticalLine + '╗';
        private string LowerBorder => '╚' + VerticalLine + '╝';

        private void Render()
        {
            _canvas.Clear();

            if (InfoPanel is not null)
            {
                foreach (var line in InfoPanel.GetText(ColumnsNumber))
                {
                    _canvas.WriteLine(line, new(ConsoleColor.White));
                }
            }

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

            try
            {
                _canvas.Display(true);
            }
            catch (ConsoleBufferException)
            {
                Console.Clear();
                OnErrorOccurred(EventArgs.Empty);
            }
        }

        private void WriteUnit(IUnit value)
        {
            var characterRule = CharacterRules.First(r => r.IsSuitableFor(value));
            var colorRule = ColorRules.First(r => r.IsSuitableFor(value));

            _canvas.Write(characterRule.Value, new(colorRule.Value));
        }

        protected virtual void OnErrorOccurred(EventArgs e)
        {
            ErrorOccurred?.Invoke(this, e);
        }
    }
}
