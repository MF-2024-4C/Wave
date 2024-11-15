using System;

namespace Quantum;

public partial struct Vector3Byte : IEquatable<Vector3Byte>
{
    public Vector3Byte(byte x, byte y, byte z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    public bool Equals(Vector3Byte other)
    {
        return X == other.X && Y == other.Y && Z == other.Z;
    }

    public override bool Equals(object obj)
    {
        return obj is Vector3Byte other && Equals(other);
    }

    public static bool operator ==(Vector3Byte left, Vector3Byte right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Vector3Byte left, Vector3Byte right)
    {
        return !left.Equals(right);
    }
}