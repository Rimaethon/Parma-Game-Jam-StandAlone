using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TouchscreenButtonController : InputComponentController
{
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        var eventTrigger = gameObject.AddComponent<EventTrigger>();
        var entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener(delegate { VirtualInput.isPressed = true; });
        eventTrigger.triggers.Add(entry);
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;
        entry.callback.AddListener(delegate { StartCoroutine(ReleaseButton()); });
        eventTrigger.triggers.Add(entry);
    }
    IEnumerator ReleaseButton()
    {
        yield return null;
        VirtualInput.isPressed = false;
    }
}
