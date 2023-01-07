using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Joint))]
public class JointDismemberPhysics : MonoBehaviour {
    [Header("Parameters")]
    [ReadOnlyField(true)]public float breakForce = 1000;
    [ReadOnlyField(true)]public float breakTorque = 1000;
    private RagdollDismembermentVisual visual;
    private Joint joint;
    private float sqrBreakForce;
    private float sqrBreakTorque;

    private void Awake()
    {
        sqrBreakForce = breakForce * breakForce;
        sqrBreakTorque = breakTorque * breakTorque;
        visual = GetComponentInParent<RagdollDismembermentVisual>();
        joint = GetComponent<Joint>();
        visual.OnDismemberCompleted.AddListener((string name) =>
        {
            if (name == this.name)
            {
                joint.breakForce = 0;
                Destroy(this);
            }
        });
    }
    private void FixedUpdate()
    {
        var sqrTimeScale = Time.timeScale * Time.timeScale;
        if (joint.currentForce.sqrMagnitude* sqrTimeScale > sqrBreakForce ||
            joint.currentTorque.sqrMagnitude * sqrTimeScale > sqrBreakTorque)
        {
            BreakJoint();
        }
    }
    public void BreakJoint()
    {
        visual.Dismember(name);
        enabled = false;
    }
}
