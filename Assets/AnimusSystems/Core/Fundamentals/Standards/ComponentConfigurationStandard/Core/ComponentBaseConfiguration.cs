using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ComponentBaseConfiguration : MonoBehaviour
{
    public ModeSwitchedEvent OnActiveModeSwitched;
    [SerializeField, ReadOnlyField] protected string ActiveMode;
    public virtual string activeMode { get; set; }

    //[SerializeField, HideInInspector] protected List<ConfigurationBase<T>> Configurations;
    //protected T this[string key] { get { return Configurations.Find(x => x.key == key)?.value; } }

    public class ConfigurationBase<T>
    {
        public string key;
        public T value;
    }
    [System.Serializable]
    public class ModeSwitchedEvent : UnityEvent<string> { }
}

