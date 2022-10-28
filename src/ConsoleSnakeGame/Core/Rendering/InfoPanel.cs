using System.Text;
using CommunityToolkit.Common;

namespace ConsoleSnakeGame.Core.Rendering
{
    internal class InfoPanel
    {
        private const int SpacesBetweenItems = 3;
        private readonly Item[] _items;
        private (int MaxLineLength, IEnumerable<string> Text)? _cached;

        public InfoPanel(params Item[] items)
        {
            _items = items ?? throw new ArgumentNullException(nameof(items));

            foreach (var item in items)
                item.Updated += (_, _) => _cached = null;
        }

        public IEnumerable<string> GetText(int maxLineLength)
        {
            if (maxLineLength <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxLineLength), "Length must be greater that zero.");

            return _cached?.MaxLineLength == maxLineLength ? _cached.Value.Text : BuildText(maxLineLength);
        }

        private IEnumerable<string> BuildText(int maxLineLength)
        {
            var result = new List<string>();
            var lineBuilder = new StringBuilder();

            foreach (var item in _items.Select(i => i.ToString()))
            {
                if (lineBuilder.Length > 0)
                {
                    if (item.Length + SpacesBetweenItems <= maxLineLength - lineBuilder.Length)
                    {
                        lineBuilder.Append(new string(' ', SpacesBetweenItems));
                    }
                    else
                    {
                        ProcessLine();
                    }
                }

                lineBuilder.Append(item.Truncate(maxLineLength - "...".Length, true));
            }

            ProcessLine();

            return result;

            void ProcessLine()
            {
                result.Add(lineBuilder.ToString());
                lineBuilder.Clear();
            }
        }

        public class Item
        {
            public event EventHandler? Updated;

            private object? _value;
            public object? Value
            {
                get => _value;
                set
                {
                    _value = value;
                    OnUpdated(EventArgs.Empty);
                }
            }

            public override string ToString()
            {
                return _value?.ToString() ?? string.Empty;
            }

            protected virtual void OnUpdated(EventArgs e)
            {
                Updated?.Invoke(this, e);
            }
        }

        public class NamedItem : Item
        {
            public string Name { get; }

            public NamedItem(string name)
            {
                Name = !string.IsNullOrWhiteSpace(name) ? name : throw
                    new ArgumentException("Name cannot be null or whitespace.", nameof(name));
            }

            public override string ToString()
            {
                return $"{Name}: " + base.ToString();
            }
        }
    }
}
