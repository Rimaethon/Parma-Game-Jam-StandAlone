using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static ObjectCollisionPhysics;

public class CollisionMaterialController : MonoBehaviour
{
    [SerializeField, ReadOnlyField] private string material;
    public string Material
    {
        get => material;
        private set
        {
            if (!InvokeEveryCollision && material == value) return;
            MaterialEvent materialEvent;
            if (eventMap.TryGetValue(value, out materialEvent)) materialEvent.OnCollide.Invoke();
            material = value;
        }
    }
    public bool InvokeEveryCollision;
    public MaterialEvent[] MaterialEvents;
    private Dictionary<string, MaterialEvent> eventMap = new Dictionary<string, MaterialEvent>();

    const string instanceSuffix = " (Instance)";

    private void Awake()
    {
        for (int i=0; i<MaterialEvents.Length;i++)
        {
            eventMap.Add(MaterialEvents[i].name, MaterialEvents[i]);
        }
    }

    public void UpdateMaterial(CollisionData data)
    {
        var name = data.OtherCollider.material.name;
        Material = name.EndsWith(instanceSuffix) ? name.Substring(0, name.Length - instanceSuffix.Length) : name;
    }
    [System.Serializable]
    public class MaterialEvent
    {
        public string name;
        public UnityEvent OnCollide;
    }
}
