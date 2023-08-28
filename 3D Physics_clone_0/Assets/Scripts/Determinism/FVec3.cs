using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FVec3
{
    public FInt32 x, y, z;
    public static FVec3 zero = new FVec3(0, 0, 0);

    public FVec3(FInt32 xpos, FInt32 ypos, FInt32 zpos)
    {
        x = xpos;
        y = ypos;
        z = zpos;
    }

    public Vector3 ToVec3() => new Vector3(x.ToFloat, y.ToFloat, z.ToFloat);
    public FInt32 Magnitude => FInt32.Sqrt(FInt32.Pow(x, 2) + FInt32.Pow(y, 2) + FInt32.Pow(z, 2));

    public static FInt32 Distance(FVec3 a, FVec3 b) => FInt32.Sqrt(FInt32.Pow(a.x - b.x, 2) + FInt32.Pow(a.y - b.y, 2) + FInt32.Pow(a.z - b.z, 2));

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((FVec3)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return this.x.ToInt;
        }
    }

    public static FVec3 operator +(FVec3 a, FVec3 b) => new FVec3(a.x + b.x, a.y + b.y, a.z + b.z);
    public static FVec3 operator -(FVec3 a, FVec3 b) => new FVec3(a.x - b.x, a.y - b.y, a.z - b.z);
    public static FVec3 operator /(FVec3 a, FInt32 f) => new FVec3(a.x / f, a.y / f, a.z / f);
    public static FVec3 operator *(FVec3 a, FInt32 f) => new FVec3(a.x * f, a.y * f, a.z * f);
    public static FVec3 operator *(FInt32 f, FVec3 a) => new FVec3(a.x * f, a.y * f, a.z * f);
    public static FVec3 operator *(int i, FVec3 a) => new FVec3(a.x * i, a.y * i, a.z * i);
    public static FVec3 operator *(FVec3 a, int i) => new FVec3(a.x * i, a.y * i, a.z * i);

    public static bool operator ==(FVec3 a, FVec3 b) => a.x == b.x && a.y == b.y && a.z == b.z;
    public static bool operator !=(FVec3 a, FVec3 b) => a.x != b.x || a.y != b.y || a.z != b.z;

}