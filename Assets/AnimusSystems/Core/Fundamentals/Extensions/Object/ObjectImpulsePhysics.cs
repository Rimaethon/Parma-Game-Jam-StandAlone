using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectImpulsePhysics : MonoBehaviour
{
    public Vector3 Direction;
    public float Power;

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody) other.attachedRigidbody.velocity = transform.TransformDirection(Direction.normalized) * Power;
    }
}
