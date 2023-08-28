using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneShell : Shell
{
    public Vector3 plane;
    public float Distance;

    private void Start()
    {
        shell.position = plane;
    }

    public override bool TestCollision(Transform otherTransform, Shell otherShell, Transform otherShellTransform)
    {
        return base.TestCollision(otherTransform, otherShell, otherShellTransform);
    }

    public override bool TestCollision(Transform otherTransform, SphereShell otherShell, Transform otherShellTransform)
    {
        return base.TestCollision(otherTransform, otherShell, otherShellTransform);
    }

    public override bool TestCollision(Transform otherTransform, PlaneShell otherShell, Transform otherShellTransform)
    {
        return base.TestCollision(otherTransform, otherShell, otherShellTransform);
    }
}
