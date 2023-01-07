using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerSimulationPhysics : MonoBehaviour
{
    public static ManagerSimulationPhysics Instance;
    [HideInInspector] public List<RigidbodyInterpolationPhysics> Interpolators = new List<RigidbodyInterpolationPhysics>();

    private void Awake()
    {
        Instance = this;
        Physics.autoSimulation = false;
    }
    private void FixedUpdate()
    {
        //set last positions/rotations
        foreach (var i in Interpolators) i.RevertTransform();
        Physics.Simulate(Time.fixedDeltaTime);
        foreach (var i in Interpolators) i.UpdateTransform();
    }
    void Update()
    {
        //perform interpolation
        var lerp = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
        foreach (var i in Interpolators) i.Interpolate(lerp);
    }
}
