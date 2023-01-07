using System.Collections.Generic;
using UnityEngine;

public class ComponentExampleConfiguration : ComponentBaseConfiguration
{
    [SerializeField, HideInInspector] protected List<Entry> Configurations;
    public Vector3 this[string key] { get { return Configurations.Find(x => x.key == key).value; } }


    [System.Serializable]
    public class Entry : ConfigurationBase<Vector3> { }
}
