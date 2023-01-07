using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AvatarOverallInput))]
public class InputTransferController : MonoBehaviour
{
    [SerializeField, ReadOnlyField] private AvatarOverallInput OtherAvatar;
    public TransferMode mode = TransferMode.Send;

    public AvatarOverallInput otherAvatar
    {
        get { return OtherAvatar; }
        set
        {
            if (OtherAvatar == value) return;
            //1. Disconnect input with last avatar
            if (OtherAvatar != null && ConnectedInputs!=null)
            {
                for (int i = 0; i < ConnectedInputs.Length; i++)
                {
                    var input =
                        mode == TransferMode.Receive ?
                        thisAvatar.inputs[ConnectedInputs[i]] :
                        OtherAvatar.inputs[ConnectedInputs[i]];

                    input.Axis = 0;
                    input.Direction = Vector2.zero;
                    input.isPressed = false;
                }
            }
            //2. Update avatar and set enabled state
            OtherAvatar = value;
            enabled = OtherAvatar != null && OtherAvatar!=thisAvatar;
            //3. Find connected inputs for new avatar
            if (enabled)
            {
                List<string> ConnectedInputsList = new List<string>();
                foreach (var key in thisAvatar.inputs.Keys) if (OtherAvatar.inputs.ContainsKey(key)) ConnectedInputsList.Add(key);
                ConnectedInputs = ConnectedInputsList.ToArray();
            }
            
        }
    }
    private AvatarOverallInput thisAvatar;
    private string[] ConnectedInputs;

    private void Awake()
    {
        thisAvatar = GetComponent<AvatarOverallInput>();
    }

    void Update()
    {
        for (int i = 0; i < ConnectedInputs.Length; i++)
        {
            var receiver = mode == TransferMode.Receive ?
                        thisAvatar.inputs[ConnectedInputs[i]] :
                        OtherAvatar.inputs[ConnectedInputs[i]];
            var sender = mode == TransferMode.Send ?
                        thisAvatar.inputs[ConnectedInputs[i]] :
                        OtherAvatar.inputs[ConnectedInputs[i]];

            switch (receiver.Type)
            {
                case AvatarOverallInput.VirtualInputComponent.InputType.Axis: receiver.Axis = sender.Axis; break;
                case AvatarOverallInput.VirtualInputComponent.InputType.Button: receiver.isPressed = sender.isPressed; break;
                case AvatarOverallInput.VirtualInputComponent.InputType.Direction: receiver.Direction = sender.Direction; break;
            }
        }
    }
    public enum TransferMode { Send, Receive }
}
