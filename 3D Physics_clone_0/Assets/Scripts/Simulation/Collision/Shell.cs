using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public virtual bool TestCollision(Transform otherTransform, Shell otherShell)
    {
        return new bool();
    }

    public virtual bool TestCollision(Transform otherTransform, SphereShell otherShell)
    {
        return new bool();
    }

    public virtual bool TestCollision(Transform otherTransform, PlaneShell otherShell)
    {
        return new bool();
    }

    public virtual bool TestCollision(Transform otherTransform, CapsuleShell otherShell)
    {
        return new bool();
    }
}

