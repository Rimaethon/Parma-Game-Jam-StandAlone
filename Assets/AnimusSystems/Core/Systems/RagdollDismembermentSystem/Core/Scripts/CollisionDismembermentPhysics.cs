using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObjectCollisionPhysics;

public class CollisionDismembermentPhysics : MonoBehaviour
{
    public void Dismember(CollisionData data)
    {
        var joint = data.OtherCollider.GetComponent<JointDismemberPhysics>();
        if (joint)
        {
            joint.BreakJoint();
            data.OtherCollider.GetComponentInParent<CharacterRagdollPhysics>()?.DetachCollider(data.OtherCollider);
        }
    }
}
