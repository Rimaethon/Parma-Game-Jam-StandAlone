using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(ManagerInputController))]
public class ManagerInputControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Import from AvatarOverallInput")) ImportWizard.CreateWizard(target as ManagerInputController);
    }
    public class ImportWizard : ScriptableWizard
    {
        public AvatarOverallInput Input;
        public int PlayerIndex;
        private ManagerInputController manager;

        public static ImportWizard CreateWizard(ManagerInputController manager)
        {
            var wizard = DisplayWizard<ImportWizard>("ManagerInputController import wizard", "Import");
            wizard.manager = manager;
            return wizard;
        }
        private void OnWizardCreate()
        {
            if (Input == null)
            {
                Debug.LogError("Error: Input field is not assigned");
                return;
            }
            if (Input.inputs.Count<1)
            {
                Debug.Log("AvatarOverallInput is empty: nothing to import");
                return;
            }
            var playerInputTransform = manager.transform.Find("Player" + PlayerIndex);
            if (playerInputTransform == null)
            {
                playerInputTransform = new GameObject("Player" + PlayerIndex).transform;
                playerInputTransform.SetParent(manager.transform);
            }
            foreach (var element in Input.inputs)
            {
                if (playerInputTransform.Find(element.Key))
                {
                    Debug.Log(playerInputTransform.name + "/" + element.Key + " configuration already exists.");
                    continue;
                }
                var configuration = new GameObject(element.Key).transform;
                configuration.SetParent(playerInputTransform);
                var keyboard = new GameObject("Keyboard").transform;
                keyboard.SetParent(configuration);
                var mouse = new GameObject("Mouse").transform;
                mouse.SetParent(configuration);
                var gamepad = new GameObject("Gamepad").transform;
                gamepad.SetParent(configuration);
                var typeMap = new Dictionary<AvatarOverallInput.VirtualInputComponent.InputType, Type>
                {
                    { AvatarOverallInput.VirtualInputComponent.InputType.Axis, typeof(MechanicalAxisController) },
                    { AvatarOverallInput.VirtualInputComponent.InputType.Button, typeof(MechanicalButtonController) },
                    { AvatarOverallInput.VirtualInputComponent.InputType.Direction, typeof(MechanicalDirection2DController) }
                };
                keyboard.gameObject.AddComponent(typeMap[element.Value.Type]);
                mouse.gameObject.AddComponent(typeMap[element.Value.Type]);
                gamepad.gameObject.AddComponent(typeMap[element.Value.Type]);
            }
        }
    }
}
