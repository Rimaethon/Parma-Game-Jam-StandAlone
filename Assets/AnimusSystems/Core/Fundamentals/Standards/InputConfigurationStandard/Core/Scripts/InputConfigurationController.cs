using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputConfigurationController : MonoBehaviour
{
    void Start()
    {
        var playerIndex = GetComponentInParent<PlayerCameraController>().PlayerIndex;
        foreach (var inputComponent in GetComponentsInChildren<InputComponentController>())
        {
            if (!ManagerInputController.Instance.ApplyInputConfiguration(inputComponent, playerIndex, inputComponent.transform.parent.name, inputComponent.name) &&
                inputComponent.name!="Touchscreen")
            {

                Debug.Log("Input config not found! playerIndex = " + playerIndex + " input name = " + inputComponent.transform.parent.name + " device name = " + inputComponent.name);
            }
        }
    }
}
