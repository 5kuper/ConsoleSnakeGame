namespace Utilities.Text;

public record struct ConsoleColors(ConsoleColor? Foreground = null, ConsoleColor? Background = null);

internal class ConsoleCanvas
{
    private record struct ColoredCharacter(char? Value, ConsoleColors Colors = default);

    private ColoredCharacter[,] _buffer;
    private (int Col, int Row) _pos;

    public ConsoleCanvas(int initialWidth = 0, int initialHeight = 0)
    {
        CheckSize(initialWidth, initialHeight);
        _buffer = new ColoredCharacter[initialWidth, initialHeight];
    }

    public int Width => _buffer.GetLength(0);
    public int Height => _buffer.GetLength(1);

    public void Display()
    {
        for (int row = 0; row < Height; row++)
        {
            for (int col = 0; col < Width; col++)
            {
                var chr = _buffer[col, row];

                var prevForegroundColor = Console.ForegroundColor;

                if (chr.Colors.Foreground != null)
                    Console.ForegroundColor = chr.Colors.Foreground.Value;

                var prevBackgroundColor = Console.BackgroundColor;

                if (chr.Colors.Background != null)
                    Console.BackgroundColor = chr.Colors.Background.Value;

                Console.SetCursorPosition(col, row);
                Console.Write(chr.Value);

                Console.ForegroundColor = prevForegroundColor;
                Console.BackgroundColor = prevBackgroundColor;
            }
        }
    }

    public void Clear()
    {
        _buffer = new ColoredCharacter[Width, Height];
        _pos = default;
    }

    public void Write(char character, ConsoleColors colors = default)
    {
        _buffer[_pos.Col, _pos.Row] = new(character, colors);

        if (++_pos.Col == Width)
            ExpandBuffer(Width + 1, Height);
    }

    public void Write(string characters, ConsoleColors colors = default)
    {
        foreach (var c in characters)
            Write(c, colors);
    }

    public void WriteLine(char character, ConsoleColors colors = default)
    {
        Write(character, colors);
        BreakLine();
    }

    public void WriteLine(string characters, ConsoleColors colors = default)
    {
        Write(characters, colors);
        BreakLine();
    }

    private static void CheckSize(int width, int height)
    {
        if (width > Console.BufferWidth)
            throw new InvalidOperationException("Canvas width cannot be greater than console buffer width.");

        if (height > Console.BufferHeight)
            throw new InvalidOperationException("Canvas height cannot be greater than console buffer height.");
    }

    private void BreakLine()
    {
        _pos.Col = 0;

        if (++_pos.Row == Height)
            ExpandBuffer(Width, Height + 1);
    }

    private void ExpandBuffer(int newWidth, int newHeight)
    {
        CheckSize(newWidth, newHeight);
        var newArray = new ColoredCharacter[newWidth, newHeight];

        for (int i = 0; i < Width; i++)
        {
            Array.Copy(_buffer, i * Height, newArray, i * newHeight, Height);
        }

        _buffer = newArray;
    }
}
