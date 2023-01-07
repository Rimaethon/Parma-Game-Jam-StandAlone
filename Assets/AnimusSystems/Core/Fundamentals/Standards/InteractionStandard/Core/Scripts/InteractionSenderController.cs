using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionSenderController : MonoBehaviour
{
    [Header("Input")]
    public string InteractButton = "Interact";
    private AvatarOverallInput input;

    [Header("Parameters")]
    public Transform RayOrigin;
    public float RayLength;
    public float RayThickness = 0;

    public bool HasReceiver { get => receiver != null; }
    private Collider receiverCollider;
    [SerializeField, ReadOnlyField]private InteractionReceiverController receiver;
    private InteractionReceiverController Receiver
    {
        get { return receiver; }
        set
        {
            if (receiver == value) return;
            if (receiver) receiver.OnSenderUnfocused.Invoke(this);
            if (value) value.OnSenderFocused.Invoke(this);
            receiver = value;
        }
    }

    void Start()
    {
        input = GetComponentInParent<AvatarOverallInput>();
        input.inputs[InteractButton].OnPress.AddListener(delegate
        {
            if (Receiver) Receiver.OnInteractionPerform.Invoke(this);
        });
    }
    private void OnDisable()
    {
        Receiver = null;
        receiverCollider = null;
    }
    void Update()
    {
        RaycastHit hit;
        bool castResult = false;
        if (RayThickness>0)
        {
            castResult = Physics.SphereCast(RayOrigin.position, RayThickness, input.LookPosition - RayOrigin.position, out hit, RayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        } else
        {
            castResult = Physics.Raycast(RayOrigin.position, input.LookPosition - RayOrigin.position, out hit, RayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        }
        if (castResult)
        {
            if (receiverCollider!=hit.collider)
            {
                receiverCollider = hit.collider;
                Receiver = receiverCollider.GetComponentInParent<InteractionReceiverController>();
            }
        } else
        {
            Receiver = null;
            receiverCollider = null;
        }
    }
}
