using System.Numerics;

namespace Utilities.Numerics;

internal record struct IntVector2(int X, int Y)
{
    public IntVector2(int value) : this(value, value) { }

    public static IntVector2 Zero => default;
    public static IntVector2 One => new(1);

    public static IntVector2 Up => new(0, -1);
    public static IntVector2 Down => new(0, 1);

    public static IntVector2 Left => new(-1, 0);
    public static IntVector2 Right => new(1, 0);

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

    public Vector2 GetDirectionTo(IntVector2 value)
    {
        return Vector2.Normalize(value - this);
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

    public static implicit operator Vector2(IntVector2 value) => new(value.X, value.Y);
    public static explicit operator IntVector2(Vector2 value) => new((int)value.X, (int)value.Y);
}