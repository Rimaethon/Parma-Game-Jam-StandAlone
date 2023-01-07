using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SkinnablePropWizard : ScriptableWizard {
    public GameObject SkinnedMeshFBX;
	[MenuItem("Assets/Create/Skin only prop")]
    static void CreateWizard()
    {
        DisplayWizard("Skin only prop wizard", typeof(SkinnablePropWizard));
    }
    private void OnWizardCreate()
    {
        if (SkinnedMeshFBX==null)
        {
            Debug.LogError("Please assign skinned mesh fbx file");
            return;
        }

        PrefabUtility.CreatePrefab(CustomEditorExtension.GetProjectWindowPath() + SkinnedMeshFBX.name + " SkinOnly.prefab", SkinnedMeshFBX).AddComponent<SkinnableProp>();
    }
    private void OnWizardUpdate()
    {
        helpString = "This wizard creates SkinOnly prop from skinned mesh fbx file. SkinOnly prop can be attached to the character with the same skeleton hierarchy.";
    }
  
}
