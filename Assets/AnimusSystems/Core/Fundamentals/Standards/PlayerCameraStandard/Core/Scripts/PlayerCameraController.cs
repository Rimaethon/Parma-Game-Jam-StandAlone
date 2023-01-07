using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCameraController : MonoBehaviour
{
    [ReadOnlyField]public AvatarOverallInput ControllableAvatar;
    public Transform AvatarLayoutParent;
    public int PlayerIndex = 0;

    private GameObject lastAvatarLayout;
    private CameraViewController camController;

    private void Awake()
    {
        camController = GetComponent<CameraViewController>();
        UpdateControllableAvatar();
    }
    private void OnTransformParentChanged()
    {
        UpdateControllableAvatar();
    }

    void UpdateControllableAvatar()
    {
        //1. Find top AvatarOverallInput in player camera's parents
        var InputsHierarchy = GetComponentsInParent<AvatarOverallInput>();
        var newAvatar = InputsHierarchy[InputsHierarchy.Length - 1];
        //2. Skip update if new avatar equals to current
        if (newAvatar == ControllableAvatar) return;

        //3. Update avatar layout UI
        if (lastAvatarLayout)
        {
            lastAvatarLayout.SetActive(false);
            Destroy(lastAvatarLayout.gameObject);
        }
        lastAvatarLayout = Instantiate(newAvatar.AvatarLayoutPrefab, AvatarLayoutParent);
        //4. Transit camera view mode from old avatar to new
        newAvatar.GetComponent<CameraViewConfiguration>().activeMode = 
            ControllableAvatar ? 
            ControllableAvatar.GetComponent<CameraViewConfiguration>().activeMode:
            string.Empty;
        //5. Update avatar collision data for camera controller
        camController.Collision = newAvatar.GetComponentInParent<CameraCollisionController>();
        camController.constantForce = camController.Collision.GetComponent<ConstantForce>();
        //6. Set new avatar as current controllable avatar
        if (ControllableAvatar!=null) ControllableAvatar.HasPlayer = false;
        if (newAvatar != null) newAvatar.HasPlayer = true;
        ControllableAvatar = newAvatar;
    }
}
