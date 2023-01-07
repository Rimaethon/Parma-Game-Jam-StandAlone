using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraViewConfiguration : ComponentBaseConfiguration
{
    public override string activeMode
    {
        get { return ActiveMode; }
        set
        {
            camController = GetComponentInChildren<CameraViewController>();
            if (camController == null) return;
            if (this[value] != null) ActiveMode = value;
            else if (this[ActiveMode] == null) ActiveMode = Configurations[0].key;

            camController.Mode = this[ActiveMode];
        }
    }

    [SerializeField, HideInInspector] private List<Entry> Configurations;
    protected CameraConfiguration this[string key] { get { return Configurations.Find(x => x.key == key)?.value; } }

    private CameraViewController camController;
    private int cycleActiveMode;

    public void SelectInCycle(bool scrollForward)
    {
        if (Configurations[cycleActiveMode] == null || !Configurations[cycleActiveMode].value.IsInSelectionCycle) return;
        do
        {
            if (scrollForward)
            {
                cycleActiveMode = (cycleActiveMode + 1) % Configurations.Count;
            } else
            {
                if (cycleActiveMode == 0) cycleActiveMode = Configurations.Count - 1;
                else cycleActiveMode--;
            }
        }
        while (!Configurations[cycleActiveMode].value.IsInSelectionCycle);

        activeMode = Configurations[cycleActiveMode].key;
    }
    public void ReturnToCycle()
    {
        if (Configurations[cycleActiveMode].value.IsInSelectionCycle) activeMode = Configurations[cycleActiveMode].key;
    }

    [System.Serializable]
    class Entry : ConfigurationBase<CameraConfiguration> { }

    [System.Serializable]
    public class CameraConfiguration
    {
        public bool IsInSelectionCycle;

        [Header("Origin parameters")]
        public Transform Origin;
        public UpAxisSource OriginUpSource;
        public ForwardAxisSource OriginForwardSource;
        [Range(0, 1)] public float OriginSlerp = 1;

        [Header("View parameters")]
        public float Distance;
        public float FocusDistance = 50;
        [Range(0, 1)] public float DistanceLerp;

        [Header("Autofollow parameters")]
        public AutofollowMode Autofollow;
        [Range(0, 1)] public float AutofollowLerp = 0;
        public float AutofollowTimeout = 3;

        [Header("Limits properties")]
        [MinMaxField(-180, 180)] public Vector2 XLimit = new Vector2(-180, 180);
        [MinMaxField(-180, 180)] public Vector2 YLimit = new Vector2(-70, 70);

        [Header("FOV parameters")]
        [Range(0,180)]public float FOV;
        [Range(0, 1)] public float FOVLerp;

        public enum UpAxisSource
        {
            GlobalUp,
            NegativeConstantForce,
            AvatarUp,
            NegativePhysicsGravity,
            OriginUp
        }
        public enum ForwardAxisSource
        {
            GlobalForward,
            TrackParentDelta,
            AvatarForward,
            AvatarParentForward,
            OriginForward
        }
        public enum AutofollowMode
        {
            None,
            AvatarVelocity,
            ZeroRotation
        }
    }
}
