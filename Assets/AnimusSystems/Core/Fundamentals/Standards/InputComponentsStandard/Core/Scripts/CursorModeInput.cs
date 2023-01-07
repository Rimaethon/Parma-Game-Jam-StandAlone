using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorModeInput : MonoBehaviour {
    public static CursorLockMode lockMode = CursorLockMode.Locked;
    private static WaitForSeconds refreshRate = new WaitForSeconds(0.3f);

    private void OnEnable()
    {
        StartCoroutine(CursorUpdater());
    }
    private void OnDisable()
    {
        StopCoroutine(CursorUpdater());
    }

    IEnumerator CursorUpdater()
    {
        while (true)
        {
            Cursor.lockState = (Cursor.lockState== CursorLockMode.Locked && lockMode== CursorLockMode.Confined) ? CursorLockMode.None: lockMode;
            Cursor.visible = Cursor.lockState != CursorLockMode.Locked;
            yield return refreshRate;
        }
    }
}
