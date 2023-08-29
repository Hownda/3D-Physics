using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CapsuleShell : Shell
{
    public Vector3 end;
    public float radius;

    public override bool TestCollision(Transform otherTransform, Shell otherShell)
    {
        if (otherTransform.GetComponent<SphereShell>())
        {
            return TestCollision(otherTransform, otherTransform.GetComponent<SphereShell>());
        }
        else if (otherTransform.GetComponent<CapsuleShell>())
        {
            return TestCollision(otherTransform, otherTransform.GetComponent<CapsuleShell>());
        }
        else      
        { return false; }
    }

    public override bool TestCollision(Transform otherTransform, SphereShell otherShell)
    {
        Vector3 closestPoint = ClosestPointOnSegment(transform.position, end + transform.position, otherTransform.position);

        float distance = Mathf.Sqrt(
            Mathf.Pow(otherTransform.position.x - closestPoint.x, 2) +
            Mathf.Pow(otherTransform.position.y - closestPoint.y, 2) +
            Mathf.Pow(otherTransform.position.z - closestPoint.z, 2)
        );

        return distance <= radius + otherShell.radius;
    }

    public override bool TestCollision(Transform otherTransform, PlaneShell otherShell)
    {
        return base.TestCollision(otherTransform, otherShell);
    }

    public override bool TestCollision(Transform otherTransform, CapsuleShell otherShell)
    {
        Vector3 globalEnd = end + transform.position;
        Vector3 globalOtherEnd = otherShell.end + otherTransform.position;

        // Capsule A
        Vector3 aNormal = Vector3.Normalize(globalEnd - transform.position);
        Vector3 aLineEndOffset = aNormal * radius;
        Vector3 aA = transform.position + aLineEndOffset;
        Vector3 aB = globalEnd - aLineEndOffset;

        // Capsule B
        Vector3 bNormal = Vector3.Normalize(globalOtherEnd - otherShell.transform.position);
        Vector3 bLineEndOffset = bNormal * otherShell.radius;
        Vector3 bA = otherTransform.position + bLineEndOffset;
        Vector3 bB = globalOtherEnd - bLineEndOffset;

        // Vectors between line endpoints:
        Vector3 v0 = bA - aA;
        Vector3 v1 = bB - aA;
        Vector3 v2 = bA - aB;
        Vector3 v3 = bB - aB;

        // Squared Distances
        float d0 = Vector3.Dot(v0, v0);
        float d1 = Vector3.Dot(v1, v1);
        float d2 = Vector3.Dot(v2, v2);
        float d3 = Vector3.Dot(v3, v3);

        // Select best potential endpoint on capsule A:
        Vector3 bestA;
        if (d2 < d0 || d2 < d1 || d3 < d0 || d3 < d1)
        {
            bestA = aB;
        }
        else
        {
            bestA = aA;
        }

        // Select point on capsule B line segment nearest to best potential endpoint on A capsule:
        Vector3 bestB = ClosestPointOnLineSegment(bA, bB, bestA);

        // Now do the same for capsule A segment:
        bestA = ClosestPointOnLineSegment(aA, aB, bestB);

        Vector3 penetration_normal = bestA - bestB;
        float len = penetration_normal.magnitude;
        penetration_normal /= len;
        float penetration_depth = radius + otherShell.radius - len;
        return penetration_depth > 0;
    }

    private Vector3 ClosestPointOnLineSegment(Vector3 a, Vector3 b, Vector3 point)
    {
        Vector3 AB = b - a;
        float t = Vector3.Dot(point - a, AB) / Vector3.Dot(AB, AB);
        t = Mathf.Clamp(t, 0f, 1f);
        return a + t * AB;
    }

    private Vector3 ClosestPointOnSegment(Vector3 start, Vector3 end, Vector3 point)
    {
        Vector3 segment = end - start;

        float segmentLengthSquared = Mathf.Pow(segment.x, 2) + Mathf.Pow(segment.y, 2) + Mathf.Pow(segment.z, 2);
        float t = ((point.x - start.x) * segment.x + (point.y - start.y) * segment.y + (point.z - start.z) * segment.z) / segmentLengthSquared;

        t = Mathf.Clamp(t, 0f, 1f);

        return start + (t * segment);
    }

    private void OnValidate()
    {
        if (!GetComponent<CapsuleCollider>())
        {
            CapsuleCollider collider = transform.AddComponent<CapsuleCollider>();
            collider.radius = radius;
            collider.height = CalculateHeight();
        }
        else
        {
            CapsuleCollider collider = GetComponent<CapsuleCollider>();
            collider.radius = radius;
            collider.height = CalculateHeight();
        }
    }

    private float CalculateHeight()
    {
        float dx = end.x;
        float dy = end.y;
        float dz = end.z;

        return Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
    }
}
