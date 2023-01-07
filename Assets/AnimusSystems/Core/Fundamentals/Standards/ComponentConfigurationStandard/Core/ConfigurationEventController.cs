using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class ConfigurationEventController : MonoBehaviour
{
    public ComponentBaseConfiguration Configuration;
    public ConfigurationEvent[] ModeEvents;

    private void Awake()
    {
        Configuration.OnActiveModeSwitched.AddListener(delegate (string mode)
        {
            Debug.Log("Active mode = " + mode);
            ModeEvents.FirstOrDefault(m => m.name.Equals(mode))?.Event.Invoke();
        });
    }

    [System.Serializable]
    public class ConfigurationEvent
    {
        public string name;
        public UnityEvent Event;
    }
}
