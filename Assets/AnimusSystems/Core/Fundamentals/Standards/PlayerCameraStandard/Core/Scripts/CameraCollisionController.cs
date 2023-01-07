using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollisionController : MonoBehaviour
{
    public Collider[] avatarColliders { get; private set; }

    private void Start()
    {
        UpdateAvatarColliders();
    }
    private void OnTransformChildrenChanged()
    {
        UpdateAvatarColliders();
    }

    public void UpdateAvatarColliders()
    {
        avatarColliders = GetComponentsInChildren<Collider>(true);
    }
}
