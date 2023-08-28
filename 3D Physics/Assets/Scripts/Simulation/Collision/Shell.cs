using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public Transform shell;

    private void Awake()
    {
        shell = Instantiate(new GameObject("Empty"), transform).transform;
    }

    public virtual bool TestCollision(Transform otherTransform, Shell otherShell, Transform otherShellTransform)
    {
        return new bool();
    }

    public virtual bool TestCollision(Transform otherTransform, SphereShell otherShell, Transform otherShellTransform)
    {
        return new bool();
    }

    public virtual bool TestCollision(Transform otherTransform, PlaneShell otherShell, Transform otherShellTransform)
    {
        return new bool();
    }
}

