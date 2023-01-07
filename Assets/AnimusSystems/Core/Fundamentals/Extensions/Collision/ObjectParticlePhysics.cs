using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ObjectParticlePhysics : ObjectCollisionPhysics
{
    private ParticleSystem particle;
    private List<ParticleCollisionEvent> collisionEvents;

    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }


    private void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = particle.GetCollisionEvents(other, collisionEvents);

        for (int i=0; i<numCollisionEvents;i++)
        {
            OnCollisionEnterEvent.Invoke(
                new CollisionData {
                    OtherCollider = (Collider)collisionEvents[i].colliderComponent,
                    Point = collisionEvents[i].intersection
                });
        }
    }
}
