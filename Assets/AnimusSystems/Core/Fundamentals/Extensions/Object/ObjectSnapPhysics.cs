using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObjectCollisionPhysics;

public class ObjectSnapPhysics : MonoBehaviour
{
    private Dictionary<Collider, Transform> children = new Dictionary<Collider, Transform>();

    public void Attach(CollisionData data)
    {
        var rigidbodies = data.OtherCollider.GetComponentsInParent<Rigidbody>();

        children.Add(data.OtherCollider, rigidbodies[rigidbodies.Length - 1].transform);
        rigidbodies[rigidbodies.Length - 1].transform.SetParent(transform);

    }
    public void Detach(CollisionData data)
    {
        if (children[data.OtherCollider] && children[data.OtherCollider].parent == transform)
        {
            children[data.OtherCollider].SetParent(transform.parent);
        }
        children.Remove(data.OtherCollider);
    }
}
