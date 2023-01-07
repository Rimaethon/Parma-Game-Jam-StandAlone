using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class CostumePropWizard : ScriptableWizard {
    public GameObject SkinnedMeshFBX;
    public GameObject StaticMesh;
    public float Mass = 2;
    public bool AutoCreateLODGroupForStaticMesh = true;

    [MenuItem("Assets/Create/Costume prop")]
    static void CreateWizard()
    {
        DisplayWizard("Costume prop wizard", typeof(CostumePropWizard));
    }
    private void OnWizardCreate()
    {
        //1. Check requirements
        if (SkinnedMeshFBX == null) Debug.LogError("Please assign skinned mesh fbx file");
        if (StaticMesh == null) Debug.LogError("Please assign static mesh");
        if (SkinnedMeshFBX == null || StaticMesh == null) return;

        //2. Create prefab based on skinned mesh
        var propPrefab = PrefabUtility.CreatePrefab(CustomEditorExtension.GetProjectWindowPath() + SkinnedMeshFBX.name + ".prefab", SkinnedMeshFBX);
        //3. Create prefab instance for modifications
        var prop = Instantiate(propPrefab);
        //4. Set mass
        prop.AddComponent<Rigidbody>().mass = Mass;
        //5. Attach static mesh to prefab instance
        var StaticInstance = Instantiate(StaticMesh).transform;
        if (StaticMesh.GetComponent<Renderer>())
        {
            var parentObject = new GameObject("Static").transform;
            parentObject.SetParent(prop.transform);
            StaticInstance.SetParent(parentObject);
        } else
            StaticInstance.SetParent(prop.transform);

        //6. Correct position of static mesh and calculate collider bounds
        var StaticBounds = StaticInstance.GetComponentInChildren<Renderer>().bounds;
        var collider = prop.AddComponent<BoxCollider>();
        StaticInstance.transform.position = -StaticBounds.center + Vector3.up * StaticBounds.size.y * 0.5f;
        collider.center = Vector3.up * StaticBounds.size.y * 0.5f;
        collider.size = StaticBounds.size;

        //7. Configure lod group
        if (AutoCreateLODGroupForStaticMesh) {
            var meshes = StaticInstance.GetComponentsInChildren<MeshRenderer>();
            var LODGroup = StaticInstance.gameObject.AddComponent<LODGroup>();
            int MaxLod = 0;
            foreach (var mesh in meshes)
            {
                var LodIndex = int.Parse(mesh.name[mesh.name.Length - 1].ToString());
                if (LodIndex>MaxLod) MaxLod = LodIndex;
            }
            var lods = new LOD[MaxLod + 1];
            for (int i=0; i<lods.Length;i++)
            {
                lods[i].screenRelativeTransitionHeight =  i==0?0.5f:i==lods.Length-1?0.02f:lods[i-1].screenRelativeTransitionHeight*0.5f;
                lods[i].renderers = meshes.Where(m => m.name[m.name.Length - 1].ToString() == i.ToString()).ToArray();
            }
            
            LODGroup.SetLODs(lods);
        }
        //8. Attach costume prop component
        //prop.AddComponent<CostumeProp>().UIPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/AnimusDigital/UI/Prefabs/Inventory System/PropUI.prefab");
        //9. Apply changes to prefab
        PrefabUtility.ReplacePrefab(prop, propPrefab, ReplacePrefabOptions.ConnectToPrefab);
        //10. Destroy prefab instance
        DestroyImmediate(prop);

    }
    private void OnWizardUpdate()
    {
        helpString = "This wizard creates SkinOnly prop from skinned mesh fbx file. SkinOnly prop can be attached to the character with the same skeleton hierarchy." +
                     "Note: LOD group auto-creation works properly only with meshes' names which ends on _lod# where # can be in range [0-9]";
    }
}
