using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;
using UnityEditor.Events;
using UnityEngine.Events;

public class AvatarLayoutWizardEditor : ScriptableWizard
{
    public string LayoutName = "GeneratedLayout";
    public InputElement[] InputElements = new InputElement[0];
    public OnLayoutCreatedEvent OnLayoutCreated = new OnLayoutCreatedEvent();
    [MenuItem("Instarion tools/Avatar layout wizard")]
    [MenuItem("Assets/Core extensions/Avatar layout wizard")]
    static void CreateWizard()
    {
        DisplayWizard<AvatarLayoutWizardEditor>("Avatar layout wizard");
    }
    RectTransform CreatePanel(string name, Transform parent)
    {
        var panel = new GameObject(name).AddComponent<RectTransform>();
        panel.SetParent(parent);
        panel.anchorMin = Vector2.zero;
        panel.anchorMax = Vector2.one;
        panel.sizeDelta = Vector2.zero;
        return panel;
    }
    private void OnWizardCreate()
    {
        var canvas = new GameObject("Canvas").AddComponent<Canvas>();
        var avatarLayout = CreatePanel(LayoutName, canvas.transform);
        avatarLayout.gameObject.AddComponent<InputConfigurationController>();
        {
            var overallUI = CreatePanel("OverallUI", avatarLayout);
            {
                var layoutHeaderLabel = new GameObject("Header").AddComponent<RectTransform>();
                layoutHeaderLabel.SetParent(overallUI);
                layoutHeaderLabel.anchorMin = layoutHeaderLabel.anchorMax = Vector2.up;
                layoutHeaderLabel.sizeDelta = new Vector2(250, 30);
                layoutHeaderLabel.anchoredPosition = new Vector2(150, -45);

                var label = layoutHeaderLabel.gameObject.AddComponent<Text>();
                label.text = LayoutName;
                label.resizeTextForBestFit = true;
                label.color = Color.cyan;
            }
            var mechanicalUI = CreatePanel("MechanicalUI", avatarLayout);
            {
                var mechanicalPromptsLabel = new GameObject("Prompts").AddComponent<RectTransform>();
                mechanicalPromptsLabel.SetParent(mechanicalUI);
                mechanicalPromptsLabel.anchorMin = mechanicalPromptsLabel.anchorMax = Vector2.right;
                mechanicalPromptsLabel.sizeDelta = new Vector2(250,150);
                mechanicalPromptsLabel.pivot = Vector2.right;
                mechanicalPromptsLabel.anchoredPosition = Vector2.zero;

                var label = mechanicalPromptsLabel.gameObject.AddComponent<Text>();
                label.text = "Prompts";
                label.color = Color.cyan;

                mechanicalPromptsLabel.gameObject.AddComponent<MechanicalPromptsUI>();
            }
            var touchscreenUI = CreatePanel("TouchscreenUI", avatarLayout);
            var inputs = CreatePanel("Inputs", avatarLayout);

            int leftButtons = 0;
            int rightButtons = 0;
            foreach (var element in InputElements)
            {
                var panel = CreatePanel(element.name, inputs);
                var hub = panel.gameObject.AddComponent<InputHubController>();
                var keyboard = new GameObject("Keyboard").transform;
                keyboard.SetParent(hub.transform);
                var mouse = new GameObject("Mouse").transform;
                mouse.SetParent(hub.transform);
                var gamepad = new GameObject("Gamepad").transform;
                gamepad.SetParent(hub.transform);
                var touchscreen = new GameObject("Touchscreen").AddComponent<RectTransform>();
                touchscreen.SetParent(hub.transform);

                keyboard.gameObject.AddComponent(element.mechanicalMap[element.type]);
                mouse.gameObject.AddComponent(element.mechanicalMap[element.type]);
                gamepad.gameObject.AddComponent(element.mechanicalMap[element.type]);
                touchscreen.gameObject.AddComponent(element.touchscreenMap[element.type]);
                if (element.type== InputElement.PresetType.Button)
                {
                    touchscreen.gameObject.AddComponent<Image>().color = Color.clear;
                    touchscreen.sizeDelta = new Vector2(120, 80);
                    bool isRight = element.alignment == InputElement.ElementAlignment.Right;
                    if (isRight) rightButtons++; else leftButtons++;

                    touchscreen.anchorMin = touchscreen.anchorMax = isRight ? Vector2.right : Vector2.zero;
                    touchscreen.anchoredPosition = new Vector2(90 * (isRight ? -1 : 1), (touchscreen.sizeDelta.y + 20) * (isRight ? rightButtons : leftButtons));
                    panel.gameObject.AddComponent<InputPostprocessController>().AvatarInputName = element.name;


                    var buttonImage = new GameObject(element.name).AddComponent<Image>();
                    buttonImage.transform.SetParent(touchscreenUI);
                    touchscreen.GetComponent<Button>().targetGraphic = buttonImage;
                    buttonImage.color = Color.grey;
                    buttonImage.gameObject.AddComponent<ConstraintRectUI>().Source = touchscreen;
                    buttonImage.GetComponent<ConstraintRectUI>().Update();

                    var text = new GameObject("Text").AddComponent<Text>();
                    text.rectTransform.SetParent(buttonImage.rectTransform);
                    text.text = element.name;
                    text.rectTransform.anchorMin = Vector2.zero;
                    text.rectTransform.anchorMax = Vector2.one;
                    text.rectTransform.anchoredPosition = Vector2.zero;
                    text.rectTransform.sizeDelta = Vector2.zero;
                    text.alignment = TextAnchor.MiddleCenter;

                } else
                {
                    if (element.alignment == InputElement.ElementAlignment.Left)
                    {
                        touchscreen.anchorMin = Vector2.zero;
                        touchscreen.anchorMax = new Vector2(0.5f, 1);
                    }
                    else
                    {
                        touchscreen.anchorMin = new Vector2(0.5f, 0);
                        touchscreen.anchorMax = Vector2.one;
                    }
                    touchscreen.sizeDelta = Vector2.zero;
                    panel.gameObject.AddComponent<InputPostprocessController>().AvatarInputName = element.name;
                }
                var hubSerializedObject = new SerializedObject(hub);
                hubSerializedObject.Update();
                hubSerializedObject.FindProperty("Keyboard").objectReferenceValue = keyboard.GetComponent<InputComponentController>();
                hubSerializedObject.FindProperty("Mouse").objectReferenceValue = mouse.GetComponent<InputComponentController>();
                hubSerializedObject.FindProperty("Gamepad").objectReferenceValue = gamepad.GetComponent<InputComponentController>();
                hubSerializedObject.FindProperty("Touchscreen").objectReferenceValue = touchscreen.GetComponent<InputComponentController>();
                hubSerializedObject.ApplyModifiedProperties();
            }
            var layoutSwitch = avatarLayout.gameObject.AddComponent<LayoutSwitchUI>();
            layoutSwitch.OnGamepadInput = new UnityEngine.Events.UnityEvent();
            UnityEventTools.AddBoolPersistentListener(layoutSwitch.OnGamepadInput, mechanicalUI.gameObject.SetActive, true);
            UnityEventTools.AddBoolPersistentListener(layoutSwitch.OnGamepadInput, touchscreenUI.gameObject.SetActive, false);
            layoutSwitch.OnKeyboardMouseInput = new UnityEngine.Events.UnityEvent();
            UnityEventTools.AddBoolPersistentListener(layoutSwitch.OnKeyboardMouseInput, mechanicalUI.gameObject.SetActive, true);
            UnityEventTools.AddBoolPersistentListener(layoutSwitch.OnKeyboardMouseInput, touchscreenUI.gameObject.SetActive, false);
            layoutSwitch.OnTouchscreenInput = new UnityEngine.Events.UnityEvent();
            UnityEventTools.AddBoolPersistentListener(layoutSwitch.OnTouchscreenInput, mechanicalUI.gameObject.SetActive, false);
            UnityEventTools.AddBoolPersistentListener(layoutSwitch.OnTouchscreenInput, touchscreenUI.gameObject.SetActive, true);
        }

        OnLayoutCreated.Invoke(PrefabUtility.SaveAsPrefabAsset(avatarLayout.gameObject, CustomEditorExtension.GetProjectWindowPath() + LayoutName + ".prefab"));
        DestroyImmediate(canvas.gameObject);
    }

    [System.Serializable]
    public class InputElement
    {
        public string name;
        public PresetType type;
        public ElementAlignment alignment;

        public Dictionary<PresetType, Type> mechanicalMap { get; private set; } = new Dictionary<PresetType, Type>
            {
                {PresetType.Button, typeof(MechanicalButtonController) },
                {PresetType.Joystick, typeof(MechanicalDirection2DController) },
                {PresetType.Trackpad, typeof(MechanicalDirection2DController) },
                {PresetType.Steer, typeof(MechanicalAxisController) }
            };
        public Dictionary<PresetType, Type> touchscreenMap { get; private set; } = new Dictionary<PresetType, Type>
            {
                {PresetType.Button, typeof(TouchscreenButtonController) },
                {PresetType.Joystick, typeof(TouchscreenJoystickController) },
                {PresetType.Trackpad, typeof(TouchscreenTrackpadController) },
                {PresetType.Steer, typeof(TouchscreenSteerController) }
            };
        public enum PresetType { Joystick, Trackpad, Button, Steer }
        public enum ElementAlignment { Left, Right }
    }
    public class OnLayoutCreatedEvent : UnityEvent<GameObject> { }
    [MenuItem("CONTEXT/AvatarOverallInput/Generate input layout and configuration")]
    static void GenerateLayoutAndConfiguration()
    {
        var avatarInput = Selection.activeTransform.GetComponent<AvatarOverallInput>();
        var layoutWizard = DisplayWizard<AvatarLayoutWizardEditor>("Avatar layout wizard");
        layoutWizard.LayoutName = avatarInput.name + " layout";
        layoutWizard.InputElements = new InputElement[avatarInput.inputs.Count];
        int i = 0;
        foreach (var input in avatarInput.inputs)
        {
            layoutWizard.InputElements[i] = new InputElement();
            layoutWizard.InputElements[i].name = input.Key;
            switch (input.Value.Type)
            {
                case AvatarOverallInput.VirtualInputComponent.InputType.Axis: layoutWizard.InputElements[i].type = InputElement.PresetType.Steer; break;
                case AvatarOverallInput.VirtualInputComponent.InputType.Button: layoutWizard.InputElements[i].type = InputElement.PresetType.Button; break;
                case AvatarOverallInput.VirtualInputComponent.InputType.Direction: layoutWizard.InputElements[i].type = InputElement.PresetType.Joystick; break;
            }
            i++;
        }
        layoutWizard.OnLayoutCreated.AddListener(delegate (GameObject layoutPrefab)
        {
            avatarInput.AvatarLayoutPrefab = layoutPrefab;
            var inputManager = GameObject.FindObjectOfType<ManagerInputController>();
            if (!inputManager) inputManager = new GameObject("InputManager").AddComponent<ManagerInputController>();
            var inputWizard = ManagerInputControllerEditor.ImportWizard.CreateWizard(inputManager);
            inputWizard.Input = avatarInput;
        });
    }
}
