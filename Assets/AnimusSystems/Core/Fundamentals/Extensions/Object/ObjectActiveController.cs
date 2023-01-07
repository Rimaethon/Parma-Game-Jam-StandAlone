using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectActiveController : MonoBehaviour
{
    public bool Active
    {
        get => gameObject.activeSelf;
        set
        {
            if (value == gameObject.activeSelf) return;
            gameObject.SetActive(value);
        }
    }
}
