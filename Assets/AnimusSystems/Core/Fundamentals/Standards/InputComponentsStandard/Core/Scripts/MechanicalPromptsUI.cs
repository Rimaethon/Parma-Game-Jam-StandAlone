using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class MechanicalPromptsUI : MonoBehaviour
{
    private InputPostprocessController[] postprocessors;
    private Text label;

    private void Awake()
    {
        label = GetComponent<Text>();
        var layoutSwitch = GetComponentInParent<LayoutSwitchUI>();
        postprocessors = layoutSwitch.GetComponentsInChildren<InputPostprocessController>();
        layoutSwitch.OnGamepadInput.AddListener(delegate
        {
            UpdatePrompts("Gamepad");
        });
        layoutSwitch.OnKeyboardMouseInput.AddListener(delegate
        {
            UpdatePrompts("Keyboard", "Mouse");
        });
    }
    void UpdatePrompts(params string[] filters)
    {
        var output = string.Empty;
        foreach (var postprocessor in postprocessors)
        {
            output += postprocessor.AvatarInputName + " =";
            foreach (var filter in filters)
            {
                var child = postprocessor.transform.Find(filter);
                if (child == null) continue;
                var input = child.GetComponent<InputComponentController>();
                var inputType = input.GetType();
                if (inputType==typeof(MechanicalButtonController))
                {
                    var button = input as MechanicalButtonController;
                    if (string.IsNullOrEmpty(button.Name)) continue;
                    output += " " + button.Name;
                } else if (inputType==typeof(MechanicalAxisController))
                {
                    var axis = input as MechanicalAxisController;
                    if (string.IsNullOrEmpty(axis.PositiveName) || string.IsNullOrEmpty(axis.NegativeName)) continue;
                    if (axis.PositiveName == axis.NegativeName)
                    {
                        output += " " + axis.PositiveName;
                    }
                    else
                    {
                        output += " " + axis.NegativeName + "/" + axis.PositiveName;
                    }
                } else if (inputType==typeof(MechanicalDirection2DController))
                {
                    var direction2D = input as MechanicalDirection2DController;
                    if (string.IsNullOrEmpty(direction2D.HorizontalNegativeName) ||
                        string.IsNullOrEmpty(direction2D.HorizontalPositiveName) ||
                        string.IsNullOrEmpty(direction2D.VerticalNegativeName) ||
                        string.IsNullOrEmpty(direction2D.VerticalPositiveName)) continue;

                    if (direction2D.HorizontalNegativeName == direction2D.HorizontalPositiveName)
                    {
                        output += " " + direction2D.HorizontalPositiveName + "/";
                    } else
                    {
                        output += " " + direction2D.HorizontalNegativeName + "/"
                                  + direction2D.HorizontalPositiveName + "/";
                    }
                    if (direction2D.VerticalNegativeName==direction2D.VerticalPositiveName)
                    {
                        output += direction2D.VerticalPositiveName;
                    } else
                    {
                        output += direction2D.VerticalNegativeName + "/"
                                + direction2D.VerticalPositiveName;
                    }
                }
            }
            output += "\n";
            label.text = output;
        }
    }
}
