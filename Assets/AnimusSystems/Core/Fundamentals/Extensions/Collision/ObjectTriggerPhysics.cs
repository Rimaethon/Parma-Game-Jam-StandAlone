using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTriggerPhysics : ObjectCollisionPhysics
{
    public float RaycastDistance = 0;
    private List<Collider> UnusedColliders = new List<Collider>();

    void UpdateCollisionData(Collider collider)
    {
        var data = Collisions[collider];
        data.OtherCollider = collider;
        RaycastHit hit;
        if (RaycastDistance>0 && collider.Raycast(new Ray(transform.position, transform.forward), out hit, RaycastDistance))
        {
            data.Normal = hit.normal;
            data.Point = hit.point;
            data.Distance = hit.distance;
        } else
        {
            data.Normal = -transform.forward;
            data.Point = collider.ClosestPoint(transform.position);
            data.Distance = Vector3.Distance(transform.position, data.Point);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Collisions.Add(other, new CollisionData { });
        UpdateCollisionData(other);
        OnCollisionEnterEvent.Invoke(Collisions[other]);
    }
    private void FixedUpdate()
    {
        if (UnusedColliders.Count>0) UnusedColliders.Clear();
       
        foreach (var key in Collisions.Keys)
        {
            if (key == null || !key.enabled)
            {
                UnusedColliders.Add(key);
                continue;
            }
            UpdateCollisionData(key);
        }
        for (int i = 0; i < UnusedColliders.Count; i++) OnTriggerExit(UnusedColliders[i]);
    }

    private void OnTriggerExit(Collider other)
    {
        OnCollisionExitEvent.Invoke(Collisions[other]);
        Collisions.Remove(other);
    }
}
