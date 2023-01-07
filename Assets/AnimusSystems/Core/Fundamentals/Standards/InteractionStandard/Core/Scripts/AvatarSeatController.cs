using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AvatarSeatController : MonoBehaviour
{
    [Header("Input")]
    public string ExitButton = "Exit";

    [Header("Parameters")]
    public InteractionReceiverController InteractionReceiver;

    [Header("Events")]
    public UnityEvent OnEnter;
    public UnityEvent OnExit;
    // Start is called before the first frame update
    void Start()
    {
        InteractionReceiver.OnInteractionPerform.AddListener(delegate (InteractionSenderController sender)
        {
            if (transform.childCount==0)
            {
                OnEnter.Invoke();
                SetSenderSeatState(sender.transform, true);
            } else
            {
                //send message that seat is engaged
            }
        });
        GetComponentInParent<AvatarOverallInput>().inputs[ExitButton].OnPress.AddListener(delegate
        {
            if (transform.childCount == 0) return;
            OnExit.Invoke();
            var sender = transform.GetChild(0);
            SetSenderSeatState(sender, false);
        });
    }

    void SetSenderSeatState(Transform sender, bool isUsingSeat)
    {
        var avatarRootTransform = GetComponentInParent<CameraCollisionController>().transform;
        sender.SetParent(isUsingSeat ? transform : avatarRootTransform.parent);
        var rigidbody = sender.GetComponentInParent<Rigidbody>();
        if (rigidbody == null) return;
        rigidbody.isKinematic = isUsingSeat;
        rigidbody.detectCollisions = !isUsingSeat;
        rigidbody.velocity = isUsingSeat ? Vector3.zero : avatarRootTransform.GetComponentInChildren<Rigidbody>().velocity;
        rigidbody.Sleep();
    }
}
