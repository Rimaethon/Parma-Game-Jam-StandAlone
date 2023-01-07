using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationFramerateController : MonoBehaviour
{
    public int MaxFPS = 30;

    void Awake()
    {
        Application.targetFrameRate = MaxFPS;
    }
}
