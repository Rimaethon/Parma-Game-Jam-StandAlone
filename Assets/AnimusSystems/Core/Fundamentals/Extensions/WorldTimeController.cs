using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTimeController : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float timeScale = 1;
    public float TimeScale {
        get => timeScale;
        set
        {
            timeScale = Mathf.Clamp01(value);
        }
    }
    private float initialFixedDeltaTime = 0.02f;

    void Update()
    {
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = Mathf.Clamp(initialFixedDeltaTime * timeScale,0.004f,0.1f);
    }
}
