using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using static CameraViewConfiguration.CameraConfiguration;

[RequireComponent(typeof(PlayerCameraController))]
public class CameraViewController : MonoBehaviour
{
    [Header("Input")]
    public string ViewDeltaDirection = "ViewDelta";
    private Vector2 viewDeltaDirection {
        get { return playerCamera.ControllableAvatar.inputs[ViewDeltaDirection].Direction; }
    }

    [Header("General")]
    public CameraViewConfiguration.CameraConfiguration Mode;
    [ReadOnlyField] public CameraCollisionController Collision;
    private Transform avatarRoot { get => Collision.transform; }
    [ReadOnlyField] public new ConstantForce constantForce;
    const float camRadius = 0.1f;
    const float sqrDeadzone = 0.1f * 0.1f;
    float viewResetTimer;
    float camDistance;

    private Quaternion originRotation;
    private bool hasAxisUp { get {
            switch (Mode.OriginUpSource) {
                case UpAxisSource.NegativeConstantForce: return constantForce && constantForce.force != Vector3.zero;
                case UpAxisSource.NegativePhysicsGravity: return Physics.gravity != Vector3.zero;
                default: return true;
            }
        }
    }
    private Transform lastParent;
    private Quaternion parentRotation { get => avatarRoot?.parent ? avatarRoot.parent.rotation : Quaternion.identity; }
    private Vector3 originUp
    {
        get
        {
            switch (Mode.OriginUpSource)
            {
                case UpAxisSource.OriginUp: return Mode.Origin.up;
                case UpAxisSource.AvatarUp: return avatarRoot.up;
                case UpAxisSource.NegativeConstantForce: return hasAxisUp ? -constantForce.force.normalized : Vector3.zero;
                case UpAxisSource.NegativePhysicsGravity: return hasAxisUp ? -Physics.gravity.normalized : Vector3.zero;
                default: return Vector3.up;
            }
        }
    }
    private Vector3 originForward
    {
        get
        {
            switch (Mode.OriginForwardSource)
            {
                case ForwardAxisSource.OriginForward: return Mode.Origin.forward;
                case ForwardAxisSource.TrackParentDelta: return parentRotation * Quaternion.Inverse(oldParentRotation) * originRotation * Vector3.forward;
                case ForwardAxisSource.AvatarForward: return avatarRoot.forward;
                case ForwardAxisSource.AvatarParentForward: return avatarRoot.transform.parent ? avatarRoot.transform.parent.forward : Vector3.forward;
                default: return Vector3.forward;
            }
        }
    }

    private Quaternion viewAngles;
    private Quaternion oldParentRotation;
    private Vector3 lastAvatarLocalPosition;
    private Vector3 avatarVelocity;

    private PlayerCameraController playerCamera;
    private Camera[] cameras;

    [HideInInspector] public Vector3 PositionOffset;
    [HideInInspector] public Quaternion RotationOffset;
    [HideInInspector] public float FOVOffset;
    const float OffsetLerp = 0.3f;

    void Start()
    {
        cameras = GetComponentsInChildren<Camera>();
        playerCamera = GetComponent<PlayerCameraController>();

        originRotation = avatarRoot.rotation;
        oldParentRotation = parentRotation;
        lastAvatarLocalPosition = avatarRoot.localPosition;
        avatarVelocity = avatarRoot.transform.forward;
    }

    static void UpdateOriginRotation(ref Quaternion rotation, Vector3 forward, Vector3 up, float slerp)
    {
        if (up == Vector3.zero) return;
        if (forward == up || forward==-up) forward = Vector3.Slerp(up, -up, 0.5f);
        rotation = Quaternion.Slerp(rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(forward, up), up),slerp);
    }
    static void UpdateViewAngles(ref Quaternion viewAngles, Vector2 input, bool isLimited, Vector2 XLimit, Vector2 YLimit)
    {
        var isLimitedX = XLimit.x > -180 || XLimit.y < 180;
        var isLimitedY = YLimit.x > -180 || YLimit.y < 180;
        if (!isLimited || (!isLimitedX && !isLimitedY))
        {
            viewAngles *= Quaternion.Euler(input.y, input.x, 0);
        }
        else
        {
            var direction = viewAngles * Vector3.forward;
            var Pitch = Vector3.Angle(direction, Vector3.up)-90;
            var Yaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            
            Pitch = isLimitedY ? Mathf.Clamp(Pitch + input.y, YLimit.x, YLimit.y) : (Pitch + input.y) % 360;
            Yaw = isLimitedX ? Mathf.Clamp(Yaw + input.x, XLimit.x, XLimit.y) : (Yaw + input.x) % 360;

            viewAngles = Quaternion.Euler(Pitch, Yaw, 0);
        }
    }
    static float GetRaycastDistance(Vector3 origin, Vector3 direction, float maxDistance, float radius, Collider[] ignoreColliders)
    {
        if (maxDistance == 0) return 0;
        RaycastHit[] Hits;
        if (radius > 0)
        {
            Hits = Physics.SphereCastAll(origin, radius, direction, maxDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        }
        else
        {
            Hits = Physics.RaycastAll(origin, direction, maxDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        }

        if (Hits.Length > 1) Hits = Hits.OrderBy(hit => hit.distance).ToArray();
        for (int i = 0; i < Hits.Length; i++)
        {
            if (Array.IndexOf(ignoreColliders, Hits[i].collider) < 0) return Hits[i].distance - radius;
        }
        return maxDistance;
    }
    static Vector3 GetRaycastPoint(Vector3 origin, Vector3 direction, float maxDistance, float radius, Collider[] ignoreColliders)
    {
        var distance = GetRaycastDistance(origin, direction, maxDistance, radius, ignoreColliders);
        return origin + direction * distance;
    }
    static void UpdateFOV(Camera[] cameras, float fov, float lerp)
    {
        for (int i = 0; i < cameras.Length; i++)
            cameras[i].fieldOfView = Mathf.Lerp(cameras[i].fieldOfView, fov, lerp);
    }


    void Update()
    {
        //0. Reset oldParentRotation when parent is changed (for TrackParentDelta mode)
        if (avatarRoot?.parent != lastParent) oldParentRotation = parentRotation;

        //1. Get view rotation
        UpdateOriginRotation(ref originRotation, originForward, originUp, Mode.OriginSlerp);
        if (viewResetTimer>=0) UpdateViewAngles(ref viewAngles, viewDeltaDirection, hasAxisUp, Mode.XLimit, Mode.YLimit);
        
        //2. Update autofollow
        if (Mode.Autofollow != AutofollowMode.None && viewDeltaDirection.sqrMagnitude < sqrDeadzone)
        {
            if (viewResetTimer < 0)
            {
                switch (Mode.Autofollow)
                {
                    case AutofollowMode.AvatarVelocity:
                        var velocity = avatarRoot.localPosition - lastAvatarLocalPosition;
                        if (velocity.sqrMagnitude > sqrDeadzone) avatarVelocity = velocity;
                        var direction = avatarRoot.transform.parent ? avatarRoot.transform.TransformDirection(avatarVelocity) : avatarVelocity;
                        var targetViewRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(direction,originUp), originUp) * Quaternion.Inverse(originRotation);
                        viewAngles = Quaternion.Slerp(viewAngles, targetViewRotation, Mode.AutofollowLerp);
                        break;
                    case AutofollowMode.ZeroRotation:
                        viewAngles = Quaternion.Slerp(viewAngles, Quaternion.identity, Mode.AutofollowLerp);
                        break;
                }
            }
            else viewResetTimer -= Time.deltaTime;
        }
        else viewResetTimer = Mode.AutofollowTimeout;


        var viewRotation = originRotation * viewAngles;

        //2. Update LookPoint position
        playerCamera.ControllableAvatar.LookPosition = GetRaycastPoint(Mode.Origin.position, viewRotation * Vector3.forward, Mode.FocusDistance, 0, Collision.avatarColliders);
        playerCamera.ControllableAvatar.LookRotation = viewRotation;

        //3. Update previous tick variables
        oldParentRotation = parentRotation;
        lastAvatarLocalPosition = avatarRoot.localPosition;
        lastParent = avatarRoot?.parent;
    }
    private void LateUpdate()
    {
        var viewRotation = originRotation * viewAngles;
        //0. Update CameraPoint transform
        var distance = GetRaycastDistance(Mode.Origin.position, viewRotation * Vector3.back, Mode.Distance, camRadius, Collision.avatarColliders);
        if (camDistance > distance) camDistance = distance;
        else camDistance = Mathf.Lerp(camDistance, distance, Mode.DistanceLerp);
        playerCamera.ControllableAvatar.CameraPosition = Mode.Origin.position + viewRotation * Vector3.back * camDistance;
        
        transform.SetPositionAndRotation(playerCamera.ControllableAvatar.CameraPosition+ playerCamera.ControllableAvatar.LookRotation*PositionOffset, playerCamera.ControllableAvatar.LookRotation*RotationOffset);
        
        //1. Update FOV
        UpdateFOV(cameras, Mode.FOV+FOVOffset, Mode.FOVLerp);
    
        //2. Lerp offsets to zero
        PositionOffset = Vector3.Lerp(PositionOffset, Vector3.zero, OffsetLerp);
        RotationOffset = Quaternion.Slerp(RotationOffset, Quaternion.identity, OffsetLerp);
        FOVOffset = Mathf.Lerp(FOVOffset, 0, OffsetLerp);
    }
}
