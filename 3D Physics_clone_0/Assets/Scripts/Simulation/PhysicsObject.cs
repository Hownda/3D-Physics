using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    public int mass = 1;
    public Vector3 velocity;
    public FVec3 angularVelocity;
    public Vector3 force = Vector3.zero;

    private float gravity = -10;
    private float movementSpeed = 5;
    private int jumpForce = 100;

    public void Step(Vector2 moveInput, float deltaTime)
    {
        force += new Vector3(moveInput.x * mass * movementSpeed, 0, moveInput.y * mass * movementSpeed) ;
        force += new Vector3(0, 0/*gravity*/, 0);

        Vector3 roundedForce = new Vector3(RoundValue(force.x * deltaTime), RoundValue(force.y * deltaTime), RoundValue(force.z * deltaTime));
        velocity += roundedForce;
        Vector3 roundedVelocity  = new Vector3(RoundValue(velocity.x * deltaTime), RoundValue(velocity.y * deltaTime), RoundValue(velocity.z * deltaTime));
        transform.position += roundedVelocity;

        force = Vector3.zero;

        DetectCollisions();
    }

    private float RoundValue(float value)
    {
        return Mathf.Round(value * 1000) / 1000;
    }

    private void DetectCollisions()
    {
        for (int i = 0; i < PhysicsWorld.Instance.physicsObjects.Count ; i++)
        {
            PhysicsObject otherObject = PhysicsWorld.Instance.physicsObjects[i];
            if (otherObject != this)
            {
                if (GetComponent<Shell>().TestCollision(otherObject.transform, otherObject.GetComponent<Shell>()))
                {
                    
                }
            }
        }
    }   
}
