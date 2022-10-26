namespace Utilities.Text;

internal record struct ConsoleColors(ConsoleColor? Foreground = null, ConsoleColor? Background = null)
{
    public static ConsoleColors Current = new(Console.ForegroundColor, Console.BackgroundColor);

    public void Apply()
    {
        if (Foreground is not null && Console.ForegroundColor != Foreground)
            Console.ForegroundColor = Foreground.Value;

        if (Background is not null && Console.BackgroundColor != Background)
            Console.BackgroundColor = Background.Value;
    }
}

internal class ConsoleCanvas
{
    private record struct ColoredCharacter(char? Value, ConsoleColors Colors = default);

    private ColoredCharacter[,] _buffer;
    private (int Col, int Row) _pos;

    public ConsoleCanvas(int initialWidth = 0, int initialHeight = 0,
        (int Cols, int Rows) offset = default)
    {
        if (offset.Cols < 0 || offset.Rows < 0)
            throw new ArgumentOutOfRangeException(nameof(offset), "Offset values cannot be less than zero.");

        Offset = offset;

        CheckSize(initialWidth, initialHeight);
        _buffer = new ColoredCharacter[initialWidth, initialHeight];
    }

    public (int Cols, int Rows) Offset { get; }

    public int Width => _buffer.GetLength(0);
    public int Height => _buffer.GetLength(1);

    public void Display()
    {
        for (int row = 0; row < Height; row++)
        {
            for (int col = 0; col < Width; col++)
            {
                try
                {
                    Console.SetCursorPosition(col + Offset.Cols, row + Offset.Rows);
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw new ConsoleBufferException("Console buffer size is too small.");
                }

                var chr = _buffer[col, row];
                var prevColors = ConsoleColors.Current;

                chr.Colors.Apply();
                Console.Write(chr.Value);
                prevColors.Apply();
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

    private void CheckSize(int width, int height)
    {
        var str = Offset != default ? " + offset" : string.Empty;

        if (width + Offset.Cols > Console.BufferWidth)
            throw new InvalidOperationException($"Canvas width{str} cannot be greater than console buffer width.");

        if (height + Offset.Rows > Console.BufferHeight)
            throw new InvalidOperationException($"Canvas height{str} cannot be greater than console buffer height.");
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

internal class ConsoleBufferException : Exception
{
    public ConsoleBufferException() { }

    public ConsoleBufferException(string? message) : base(message) { }

    public ConsoleBufferException(string? message, Exception? innerException) : base(message, innerException) { }
}
