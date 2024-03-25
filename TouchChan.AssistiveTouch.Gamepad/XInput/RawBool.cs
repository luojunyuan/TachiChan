// SharpDX, Version=4.2.0.0, Culture=neutral, PublicKeyToken=b4dcf0f35e5521f1
// SharpDX.Mathematics.Interop.RawBool
using System;
using System.Runtime.InteropServices;

namespace SharpDX.Mathematics.Interop;

[StructLayout(LayoutKind.Sequential, Size = 4)]
public struct RawBool : IEquatable<RawBool>
{
    private readonly int boolValue;

    public RawBool(bool boolValue)
    {
        this.boolValue = (boolValue ? 1 : 0);
    }

    public bool Equals(RawBool other)
    {
        return boolValue == other.boolValue;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }
        if (obj is RawBool)
        {
            return Equals((RawBool)obj);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return boolValue;
    }

    public static bool operator ==(RawBool left, RawBool right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(RawBool left, RawBool right)
    {
        return !left.Equals(right);
    }

    public static implicit operator bool(RawBool booleanValue)
    {
        return booleanValue.boolValue != 0;
    }

    public static implicit operator RawBool(bool boolValue)
    {
        return new RawBool(boolValue);
    }

    public override string ToString()
    {
        return $"{boolValue != 0}";
    }
}
