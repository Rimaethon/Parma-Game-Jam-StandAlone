using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class ManagerInputController : MonoBehaviour
{
    public static ManagerInputController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    public bool ApplyInputConfiguration(InputComponentController target, int playerIndex, string inputName, string deviceName)
    {
        var configuration = transform.Find("Player"+playerIndex)?.Find(inputName)?.Find(deviceName)?.GetComponent<InputComponentController>();
        if (configuration == null) return false;
        foreach (FieldInfo field in target.GetType().GetFields())
        {
            field.SetValue(target, field.GetValue(configuration));
        }
        return true;
    }
}
