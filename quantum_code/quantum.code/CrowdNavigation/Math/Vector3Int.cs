using System;
using System.Runtime.CompilerServices;

namespace Quantum;

public partial struct Vector3Int : IEquatable<Vector3Int>
{
    public Vector3Int(int x, int y, int z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    public bool Equals(Vector3Int other)
    {
        return X == other.X && Y == other.Y && Z == other.Z;
    }

    public override bool Equals(object obj)
    {
        return obj is Vector3Int other && Equals(other);
    }

    public static bool operator ==(Vector3Int left, Vector3Int right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Vector3Int left, Vector3Int right)
    {
        return !left.Equals(right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3Int operator +(Vector3Int left, Vector3Int right)
    {
        return new Vector3Int(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3Int operator -(Vector3Int left, Vector3Int right)
    {
        return new Vector3Int(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3Int operator *(Vector3Int left, int right)
    {
        return new Vector3Int(left.X * right, left.Y * right, left.Z * right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3Int operator *(int left, Vector3Int right)
    {
        return new Vector3Int(left * right.X, left * right.Y, left * right.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3Int operator *(Vector3Int left, Vector3Int right)
    {
        return new Vector3Int(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3Int operator /(Vector3Int left, int right)
    {
        return new Vector3Int(left.X / right, left.Y / right, left.Z / right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3Int operator /(Vector3Int left, Vector3Int right)
    {
        return new Vector3Int(left.X / right.X, left.Y / right.Y, left.Z / right.Z);
    }
}