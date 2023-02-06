namespace Utilities.Numerics;

internal readonly struct Range<T> : IEquatable<Range<T>>
    where T : IComparable<T>
{
    public readonly T Bottom, Top;

    public Range(T bottom, T top)
    {
        if (bottom.CompareTo(top) > 0)
            throw new ArgumentException("Bottom cannot be greater than top.");

        Bottom = bottom;
        Top = top;
    }

    public bool Includes(T value) => Bottom.CompareTo(value) <= 0 && value.CompareTo(Top) <= 0;

    public bool Equals(Range<T> other) => (Bottom, Top).Equals(other);
    public override bool Equals(object? obj) => obj is Range<T> range && Equals(range);
    public override int GetHashCode() => (Bottom, Top).GetHashCode();

    public static implicit operator (T Bottom, T Top)(Range<T> range) => new(range.Bottom, range.Top);
    public static implicit operator Range<T>((T Bottom, T Top) tuple) => new(tuple.Bottom, tuple.Top);
}
