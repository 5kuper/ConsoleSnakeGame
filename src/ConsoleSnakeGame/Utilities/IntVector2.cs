using System.Numerics;

namespace Utilities.Numerics;

internal struct IntVector2 : IEquatable<IntVector2>
{
    public IntVector2(int value) : this(value, value) { }
    public IntVector2(int x, int y) => (X, Y) = (x, y);

    public static IntVector2 Zero => default;
    public static IntVector2 One => new(1);

    public static IntVector2 Up => new(0, -1);
    public static IntVector2 Down => new(0, 1);

    public static IntVector2 Left => new(-1, 0);
    public static IntVector2 Right => new(1, 0);

    public int X { get; set; }
    public int Y { get; set; }

    public float Lenght => MathF.Sqrt(LenghtSquared);
    public int LenghtSquared => Dot(this, this);

    public static IntVector2 Abs(IntVector2 value)
    {
        return new(Math.Abs(value.X), Math.Abs(value.Y));
    }

    public static float Distance(IntVector2 a, IntVector2 b)
    {
        var square = DistanceSquared(a, b);
        return MathF.Sqrt(square);
    }

    public static float DistanceSquared(IntVector2 a, IntVector2 b)
    {
        var diff = a - b;
        return Dot(diff, diff);
    }

    public static int Dot(IntVector2 a, IntVector2 b)
    {
        return (a.X * b.X) + (a.Y * b.Y);
    }

    public bool Equals(IntVector2 other)
    {
        return X.Equals(other.X) && Y.Equals(other.Y);
    }

    public override bool Equals(object? obj)
    {
        return obj is IntVector2 vector && Equals(vector);
    }

    public override int GetHashCode() => HashCode.Combine(X, Y);

    public static IntVector2 operator -(IntVector2 value) => Zero - value;

    public static IntVector2 operator +(IntVector2 a, IntVector2 b) => new(a.X + b.X, a.Y + b.Y);
    public static IntVector2 operator -(IntVector2 a, IntVector2 b) => new(a.X - b.X, a.Y - b.Y);
    public static IntVector2 operator *(IntVector2 a, IntVector2 b) => new(a.X * b.X, a.Y * b.Y);
    public static IntVector2 operator /(IntVector2 a, IntVector2 b) => new(a.X / b.X, a.Y / b.Y);

    public static IntVector2 operator *(IntVector2 vec, int num) => new IntVector2(num) * vec;
    public static IntVector2 operator *(int num, IntVector2 vec) => new IntVector2(num) * vec;
    public static IntVector2 operator /(IntVector2 vec, int num) => new IntVector2(num) / vec;

    public static bool operator ==(IntVector2 a, IntVector2 b) => a.X == b.X && a.Y == b.Y;
    public static bool operator !=(IntVector2 a, IntVector2 b) => !(a == b);

    public static implicit operator Vector2(IntVector2 value) => new(value.X, value.Y);
    public static explicit operator IntVector2(Vector2 value) => new((int)value.X, (int)value.Y);
}