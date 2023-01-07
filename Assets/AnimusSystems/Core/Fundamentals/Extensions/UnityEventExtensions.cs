using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class BaseEvent: UnityEvent
{
    protected bool Locked;
    public new void Invoke()
    {
        if (!Locked) base.Invoke();
    }
}
public class BaseEvent<T0>:UnityEvent<T0>
{
    protected bool Locked;
    public new void Invoke(T0 arg0)
    {
        if (!Locked) base.Invoke(arg0);
    }
}
public class BaseEvent<T0, T1> : UnityEvent<T0, T1>
{
    protected bool Locked;
    public new void Invoke(T0 arg0, T1 arg1)
    {
        if (!Locked) base.Invoke(arg0, arg1);
    }
}
public class BaseEvent<T0, T1, T2> : UnityEvent<T0, T1, T2>
{
    protected bool Locked;
    public new void Invoke(T0 arg0,T1 arg1, T2 arg2)
    {
        if (!Locked) base.Invoke(arg0, arg1, arg2);
    }
}
public class BaseEvent<T0,T1,T2,T3> : UnityEvent<T0,T1,T2,T3>
{
    protected bool Locked;
    public new void Invoke(T0 arg0, T1 arg1, T2 arg2, T3 arg3)
    {
        if (!Locked) base.Invoke(arg0, arg1, arg2, arg3);
    }
}
