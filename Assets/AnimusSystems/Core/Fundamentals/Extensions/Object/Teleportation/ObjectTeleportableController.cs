using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectTeleportableController : MonoBehaviour
{
    public TeleportMode Mode;
    public Transform[] Teleports;
    [SerializeField, ReadOnlyField] private Transform activeTeleport;
    public UnityEvent OnTeleport;

    private Vector3 InitialPosition;
    private Quaternion InitialRotation;

    private void Awake()
    {
        InitialPosition = transform.position;
        InitialRotation = transform.rotation;
    }

    public void Teleport()
    {
        var position = InitialPosition;
        var rotation = InitialRotation;
        switch (Mode)
        {
            case TeleportMode.ActiveTeleport:
                if (activeTeleport)
                {
                    position = activeTeleport.position;
                    rotation = activeTeleport.rotation;
                }
                break;
            case TeleportMode.Random:
                if (Teleports.Length == 0) break;
                var teleport = Teleports[Random.Range(0, Teleports.Length)];
                position = teleport.position;
                rotation = teleport.rotation;
                break;
        }
        transform.SetPositionAndRotation(position, rotation);
        transform.GetComponent<RigidbodyInterpolationPhysics>()?.ResetTransform();
        OnTeleport.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ObjectTeleportController>()) activeTeleport = other.transform;
    }

    public enum TeleportMode { InitialPosition, ActiveTeleport, Random }
}
