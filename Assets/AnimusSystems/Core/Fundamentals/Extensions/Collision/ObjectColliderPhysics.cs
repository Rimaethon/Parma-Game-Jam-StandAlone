using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectColliderPhysics : ObjectCollisionPhysics
{
    void UpdateCollisionData(Collision collision)
    {
        var firstContact = collision.GetContact(0);
        var data = Collisions[collision.collider];
        data.Normal = firstContact.normal;
        data.OtherCollider = collision.collider;
        data.Point = firstContact.point;
        data.Distance = Vector3.Distance(transform.position, data.Point);
    }
    private void OnCollisionEnter(Collision collision)
    {
        Collisions.Add(collision.collider, new CollisionData { });
        UpdateCollisionData(collision);
        OnCollisionEnterEvent.Invoke(Collisions[collision.collider]);
    }
    private void OnCollisionStay(Collision collision)
    {
        UpdateCollisionData(collision);
    }
    private void OnCollisionExit(Collision collision)
    {
        OnCollisionExitEvent.Invoke(Collisions[collision.collider]);
        Collisions.Remove(collision.collider);
    }
}
