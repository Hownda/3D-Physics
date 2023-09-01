using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.Netcode;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    public int mass = 1;

    public Vector3 velocity;
    public Vector3 angularVelocity;

    public Vector3 force = Vector3.zero;
    public Vector3 torque = Vector3.zero;
    public Vector3 moveVector = Vector3.zero;

    private List<Collision> collisions = new List<Collision>();

    public void Step(float deltaTime)
    {
        ApplyVelocity(deltaTime);
        ApplyAngularVelocity(deltaTime);
        //ResolveCollisions();
    }

    private void ApplyVelocity(float deltaTime)
    {
        force += new Vector3(0, 0/*gravity*/, 0);

        Vector3 roundedForce = new Vector3(RoundValue(force.x * deltaTime), RoundValue(force.y * deltaTime), RoundValue(force.z * deltaTime));
        velocity += roundedForce;
        Vector3 roundedVelocity = new Vector3(RoundValue(velocity.x * deltaTime), RoundValue(velocity.y * deltaTime), RoundValue(velocity.z * deltaTime));
        transform.position += roundedVelocity;

        force = Vector3.zero;
    }

    private void ApplyAngularVelocity(float deltaTime)
    {
        Vector3 roundedTorque = new Vector3(RoundValue(torque.x * deltaTime), RoundValue(torque.y * deltaTime), RoundValue(torque.z * deltaTime));
        angularVelocity += roundedTorque;
        Vector3 roundedAngularVelocity = new Vector3(RoundValue(angularVelocity.x * deltaTime), RoundValue(angularVelocity.y * deltaTime), RoundValue(angularVelocity.z * deltaTime));
        //transform.Rotate(roundedAngularVelocity);
        torque = Vector3.zero;
    }

    private void ResolveCollisions()
    {
        if (collisions.Count > 0)
        {
            foreach (var collision in collisions)
            {
                ContactPoint contact = collision.contacts[0];
                PhysicsObject physicsObject = contact.otherCollider.GetComponent<PhysicsObject>();

                if (physicsObject != null)
                {
                    Vector3 relativeVelocity = GetPointVelocity(contact.point) - physicsObject.GetPointVelocity(contact.point);
                    Vector3 impulse = CalculateImpulse(relativeVelocity, contact.normal);

                    ApplyImpulse(physicsObject, impulse, contact.point);
                    ApplyImpulse(this, -impulse, contact.point);
                }
            }
        }
    }

    public void AddForce(Vector3 appliedForce)
    {
        force += appliedForce;
    }

    public void AddTorque(Vector3 appliedTorque)
    {
        torque += appliedTorque;
    }

    public Vector3 GetPointVelocity(Vector3 point)
    {
        Vector3 pointVelocity = velocity + Vector3.Cross(angularVelocity, point - transform.position);
        return pointVelocity;
    }    

    private Vector3 CalculateImpulse(Vector3 relativeVelocity, Vector3 contactNormal)
    {
        float restitution = 0.5f;
        float impulseMagnitude = -(1.0f + restitution) * Vector3.Dot(relativeVelocity, contactNormal);
        impulseMagnitude /= (1.0f / mass);
        return impulseMagnitude * contactNormal;
    }

    private void ApplyImpulse(PhysicsObject physicsObject, Vector3 impulse, Vector3 contactPoint)
    {
        impulse /= mass;
        Vector3 roundedImpulse = new Vector3(RoundValue(impulse.x / 5), RoundValue(impulse.y / 5), RoundValue(impulse.z / 5));
        physicsObject.velocity += roundedImpulse;

        Vector3 torqueFromImpulse = Vector3.Cross(contactPoint - transform.position, roundedImpulse);
        physicsObject.torque += torqueFromImpulse;

    }

    private float RoundValue(float value)
    {
        return Mathf.Round(value * 1000) / 1000;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        PhysicsObject physicsObject = contact.otherCollider.GetComponent<PhysicsObject>();

        if (physicsObject != null)
        {
            Vector3 relativeVelocity = GetPointVelocity(contact.point) - physicsObject.GetPointVelocity(contact.point);
            Vector3 impulse = CalculateImpulse(relativeVelocity, contact.normal);

            ApplyImpulse(physicsObject, -impulse, contact.point);
            ApplyImpulse(this, impulse, contact.point);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        PhysicsObject physicsObject = contact.otherCollider.GetComponent<PhysicsObject>();

        if (physicsObject != null)
        {
            // Calculate the penetration depth
            float penetrationDepth = Mathf.Max(0.0f, -contact.separation);

            // Move the objects out of each other by the penetration depth
            MoveObjectsOutOfCollision(this, physicsObject, contact.normal * penetrationDepth);
        }
    }

    private void MoveObjectsOutOfCollision(PhysicsObject obj1, PhysicsObject obj2, Vector3 separationVector)
    {
        float totalInverseMass = (1.0f / obj1.mass) + 1.0f / obj2.mass;

        // Calculate the movement proportions for each object based on their masses
        float obj1Proportion = (1.0f / obj1.mass) / totalInverseMass;
        float obj2Proportion = 1.0f - obj1Proportion;

        if (NetworkManager.Singleton.IsServer)
        {
            // Move the objects along the separation vector by their proportions
            obj1.transform.position += separationVector * obj1Proportion;
            obj2.transform.position -= separationVector * obj2Proportion;
        }
    }
}
