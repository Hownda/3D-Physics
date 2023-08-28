using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CollisionPoints
{
    public readonly Vector3 A; // Furthest point of A into B
    public readonly Vector3 B; // Furthest point of B into A
    public readonly Vector3 Normal;
    public readonly float depth;
    public bool HasCollision;

    public CollisionPoints(Vector3 a, Vector3 b, Vector3 normal, float depth, bool hasCollision)
    {
        this.A = a; 
        this.B = b;
        this.Normal = normal;
        this.depth = depth;
        this.HasCollision = hasCollision;
    }
}
