using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneShell : Shell
{
    public Vector3 plane;
    public float Distance;

    public override bool TestCollision(Transform otherTransform, Shell otherShell)
    {
        return base.TestCollision(otherTransform, otherShell);
    }

    public override bool TestCollision(Transform otherTransform, SphereShell otherShell)
    {
        return base.TestCollision(otherTransform, otherShell);
    }

    public override bool TestCollision(Transform otherTransform, PlaneShell otherShell)
    {
        return base.TestCollision(otherTransform, otherShell);
    }
}
