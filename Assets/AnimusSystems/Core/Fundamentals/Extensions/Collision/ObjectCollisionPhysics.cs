using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectCollisionPhysics : MonoBehaviour
{
    public CollisionEvent OnCollisionEnterEvent;
    public CollisionEvent OnCollisionExitEvent;
    public Dictionary<Collider, CollisionData> Collisions = new Dictionary<Collider, CollisionData>();
    public bool IsColliding { get => Collisions.Count > 0; }

    [System.Serializable]
    public class CollisionEvent : UnityEvent<CollisionData> { }

    [System.Serializable]
    public class CollisionData
    {
        public Vector3 Point;
        public float Distance;
        public Vector3 Normal;
        public Collider OtherCollider;
    }
}
