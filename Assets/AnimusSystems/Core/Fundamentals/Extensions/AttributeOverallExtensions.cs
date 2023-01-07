using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadOnlyField : PropertyAttribute {
    public bool RuntimeReadOnly = false;
    public ReadOnlyField(bool runtimeReadOnly=false)
    {
        RuntimeReadOnly = runtimeReadOnly;
    }
}
public class LockableField : PropertyAttribute { }
public class QuaternionField : PropertyAttribute { }
public class MinMaxField : PropertyAttribute
{
    public float MinLimit = 0;
    public float MaxLimit = 1;

    public MinMaxField(int min, int max)
    {
        MinLimit = min;
        MaxLimit = max;
    }
}