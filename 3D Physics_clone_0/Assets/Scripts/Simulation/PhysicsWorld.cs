using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System.Linq;

public class PhysicsWorld : NetworkBehaviour
{
    public static PhysicsWorld Instance;
    public List<PhysicsObject> physicsObjects = new();

    private void Start()
    {
        Instance = this;
    }

    public void FindPhysicsObjects()
    {
        physicsObjects.Clear();
        physicsObjects = FindObjectsOfType<PhysicsObject>().ToList();
    }

    public void AddPhysicsObject(PhysicsObject physicsObject)
    {
        physicsObjects.Add(physicsObject);
    }

    public void RemovePhysicsObject(PhysicsObject physicsObject)
    {
        physicsObjects.Remove(physicsObject);
        Destroy(physicsObject);
    }
}
