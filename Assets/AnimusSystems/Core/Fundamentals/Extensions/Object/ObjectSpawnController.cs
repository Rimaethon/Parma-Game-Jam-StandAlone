using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawnController : MonoBehaviour
{
    public void Instantiate(GameObject prefab)
    {
        Instantiate(prefab, transform);
    }
}
