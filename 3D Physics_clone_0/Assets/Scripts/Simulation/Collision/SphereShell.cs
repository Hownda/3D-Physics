using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SphereShell : Shell
{
    public Vector3 center;
    public float radius;

    public override bool TestCollision(Transform otherTransform, Shell otherShell)
    {
        if (otherTransform.GetComponent<SphereShell>())
        {
            return TestCollision(otherTransform, otherTransform.GetComponent<SphereShell>());
        }
        else
        { return false; }
    }

    public override bool TestCollision(Transform otherTransform, SphereShell otherShell)
    {
        float distance = Mathf.Sqrt(Mathf.Pow(otherTransform.position.x - transform.position.x, 2) + Mathf.Pow(otherTransform.position.y - transform.position.y, 2) + Mathf.Pow(otherTransform.position.z - transform.position.z, 2));

        if (distance < radius + otherShell.radius) 
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool TestCollision(Transform otherTransform, PlaneShell otherShell)
    {
        return base.TestCollision(otherTransform, otherShell);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
