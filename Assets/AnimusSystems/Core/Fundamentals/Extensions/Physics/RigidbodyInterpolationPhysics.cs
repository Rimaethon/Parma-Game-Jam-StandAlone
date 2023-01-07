using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyInterpolationPhysics : MonoBehaviour
{
    public bool InterpolatePosition = true;
    public bool InterpolateRotation = false;
    private Vector3 currentLocalPosition;
    private Vector3 previousLocalPosition;
    private Quaternion currentLocalRotation;
    private Quaternion previousLocalRotation;

    public void UpdateTransform()
    {
        previousLocalPosition = currentLocalPosition;
        currentLocalPosition = transform.localPosition;
        previousLocalRotation = currentLocalRotation;
        currentLocalRotation = transform.localRotation;
    }
    public void Interpolate(float lerp)
    {
        if (InterpolatePosition) transform.localPosition = Vector3.Lerp(previousLocalPosition, currentLocalPosition, lerp);
        if (InterpolateRotation) transform.localRotation = Quaternion.Slerp(previousLocalRotation, currentLocalRotation, lerp);
    }
    public void RevertTransform()
    {
        if (InterpolatePosition) transform.localPosition = currentLocalPosition;
        if (InterpolateRotation) transform.localRotation = currentLocalRotation;
    }

    public void ResetTransform()
    {
        previousLocalPosition = transform.localPosition;
        currentLocalPosition = transform.localPosition;
        previousLocalRotation = transform.localRotation;
        currentLocalRotation = transform.localRotation;
    }
    private void Start()
    {
        ResetTransform();
        ManagerSimulationPhysics.Instance.Interpolators.Add(this);
    }
    private void OnTransformParentChanged()
    {
        UpdateTransform();
    }

    private void OnDestroy()
    {
        ManagerSimulationPhysics.Instance.Interpolators.Remove(this);
    }
}
