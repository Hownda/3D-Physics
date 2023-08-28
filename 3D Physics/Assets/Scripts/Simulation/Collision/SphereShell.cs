using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereShell : Shell
{
    public Vector3 center;
    public float radius;

    private void Start()
    {
        shell.position = center;
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
