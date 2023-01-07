using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObjectCollisionPhysics;

public class CollisionHealthController : MonoBehaviour
{
    public float HealthDelta;

    private ObjectHealthController cachedController;
    private Collider cachedCollider;

    private Dictionary<Collider, ObjectHealthController> healthControllers = new Dictionary<Collider, ObjectHealthController>();


    public void SetHealth(CollisionData collision)
    {
        if (collision.OtherCollider==cachedCollider)
        {
            cachedController.Health += HealthDelta;
        } else
        {
            var healthController = collision.OtherCollider.GetComponentInParent<ObjectHealthController>();
            if (healthController)
            {
                cachedCollider = collision.OtherCollider;
                cachedController = healthController;
                cachedController.Health += HealthDelta;
            }
        }
    }

    public void AddCollision(CollisionData collision)
    {
        if (healthControllers.ContainsKey(collision.OtherCollider)) return;
        var healthController = collision.OtherCollider.GetComponentInParent<ObjectHealthController>();
        if (healthController) healthControllers.Add(collision.OtherCollider, healthController);
    }
    public void RemoveCollision(CollisionData collision)
    {
        healthControllers.Remove(collision.OtherCollider);
    }
    private void FixedUpdate()
    {
        foreach (var value in healthControllers.Values) value.Health += HealthDelta*Time.timeScale;
    }
}
