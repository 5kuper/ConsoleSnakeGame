namespace Utilities.Numerics;

internal readonly struct Proportion : IEquatable<Proportion>, IComparable<Proportion>
{
    public Proportion(float val)
    {
        Value = val is >= 0 and <= 1 ? val : throw
            new ArgumentException("Value must be in range from 0 to 1.", nameof(val));
    }

    public Proportion(float number, Range<float> range)
    {
        if (!range.Includes(number))
            throw new ArgumentException("Number is not included in the range.");

        Value = (number - range.Bottom) / (range.Top - range.Bottom);
    }

    public float Value { get; }

    public float Percent => Value * 100;

    public bool IsMin => Value is 0;
    public bool IsMax => Value is 1;

    public static Proportion FromPercent(float pct) => new(pct / 100);
    public static Proportion OppositeOf(Proportion other) => new(1 - other);

    public float InRange(Range<float> range) => (Value * (range.Top - range.Bottom)) + range.Bottom;

    public bool Equals(Proportion other) => Value.Equals(other.Value);
    public int CompareTo(Proportion other) => Value.CompareTo(other.Value);

    public override bool Equals(object? obj) => obj is Proportion prop && Equals(prop);
    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator float(Proportion prop) => prop.Value;
    public static explicit operator Proportion(float f) => f.ToProportion();
}

internal static class ProportionExtensions
{
    public static Proportion ToProportion(this float value) => new(value);
}