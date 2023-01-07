using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestroyController : MonoBehaviour
{
    public new void Destroy(Object unityObject)
    {
        Object.Destroy(unityObject);
    }
    public void DestroyChildren()
    {
        for (int i=0; i<transform.childCount;i++)
        {
            Object.Destroy(transform.GetChild(i).gameObject);
        }
    }
}
