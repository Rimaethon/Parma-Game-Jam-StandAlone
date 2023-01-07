using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPickupController : MonoBehaviour
{
    void Start()
    {
        GetComponent<InteractionReceiverController>().OnInteractionPerform.AddListener(delegate (InteractionSenderController sender)
        {
            transform.SetParent(sender.transform);
        });
    }
}
