using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractionReceiverController : MonoBehaviour
{
    public InteractionEvent OnSenderFocused;
    public InteractionEvent OnSenderUnfocused;
    public UnityEvent OnInteractionActive;
    public UnityEvent OnInteractionInactive;
    public InteractionEvent OnInteractionPerform;

    private List<InteractionSenderController> focusedSenders = new List<InteractionSenderController>();

    private void OnDisable()
    {
        Debug.Log("Receiver has been disabled");
        focusedSenders.Clear();
    }

    private void Start()
    {
        OnSenderFocused.AddListener(delegate (InteractionSenderController sender) {
            if (focusedSenders.Contains(sender)) return;
            if (focusedSenders.Count == 0) OnInteractionActive.Invoke();
            focusedSenders.Add(sender);
            //Debug.Log(name + " focused by "+sender.name);
        });
        OnSenderUnfocused.AddListener(delegate (InteractionSenderController sender) {
            if (!focusedSenders.Contains(sender)) return;
            if (focusedSenders.Count == 1) OnInteractionInactive.Invoke();
            focusedSenders.Remove(sender);
            //Debug.Log(name + " unfocused by "+sender.name);
        });
    }


    [System.Serializable]
    public class InteractionEvent : BaseEvent<InteractionSenderController> { }
}
