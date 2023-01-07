using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SkinnableProp : MonoBehaviour {
    protected SkinnedMeshRenderer[] LODS;
    protected string[][] LODBones;
    private LODGroup CurrentLODGroup;
    private PropState propState;
    protected virtual void Start()
    {
        var lods = GetComponentsInChildren(typeof(SkinnedMeshRenderer), true);
        LODS = new SkinnedMeshRenderer[lods.Length];
        for (int i = 0; i < LODS.Length; i++) LODS[i] = (SkinnedMeshRenderer)lods[i];

        LODBones = new string[LODS.Length][];
        for (int i = 0; i < LODS.Length; i++)
        {
            List<string> Names = new List<string>();
            foreach (var bone in LODS[i].bones) Names.Add(bone.name);
            LODBones[i] = Names.ToArray();
        }
        var rootBone = LODS[0].rootBone;
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (rootBone.IsChildOf(child))
            {
                Destroy(child.gameObject);
                break;
            }
        }
        UpdatePropState();
    }

    protected void OnTransformParentChanged()
    {
        UpdatePropState(true);
    }
    protected virtual void UpdatePropState(bool ForceUpdate=false)
    {
        if (transform.parent == null || !transform.parent.tag.Equals("Human"))
        {
            SetPropState(PropState.Drop,ForceUpdate);
            return;
        }
        SetPropState(PropState.Equipped,ForceUpdate);
    }
    protected virtual void SetPropState(PropState newState, bool ForceUpdate=false)
    {
        if (propState == newState && !ForceUpdate) return;
        foreach (var lod in LODS) lod.enabled = newState == PropState.Equipped;
        if (newState != PropState.Drop)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        transform.localScale = Vector3.one;
        if (newState== PropState.Equipped)
        {
            //1. Bind to skeleton
            Transform[] SkeletonBones = transform.parent.Find("Root").GetComponentsInChildren<Transform>();
            for (int i = 0; i < LODS.Length; i++)
            {
                List<Transform> bones = new List<Transform>();
                foreach (var boneName in LODBones[i])
                {
                    try
                    {
                        bones.Add(SkeletonBones.First(b => b.name.Equals(boneName)));
                    } catch { print(boneName+" is not found!"); }
                }
                LODS[i].rootBone = bones[0];
                LODS[i].bones = bones.ToArray();
              
            }
            //2. Attach lods to parent LOD group
            ReplaceLODSToLODGroup(GetComponentInParent<LODGroup>());
        }
        propState = newState;
    }
    private void ReplaceLODSToLODGroup (LODGroup group)
    {
        if (LODS.Length < 2) return;
        if (CurrentLODGroup == group) return;
        //1. Remove LODs from old LOD group
        if (CurrentLODGroup)
        {
            var ParentLODS = CurrentLODGroup.GetLODs();
            for (int i=0; i<ParentLODS.Length; i++)
            {
                var LODList = ParentLODS[i].renderers.ToList();
                foreach (var LOD in LODS.Where(lod => lod.name[lod.name.Length - 1] == i.ToString()[0])) LODList.Remove(LOD);
                ParentLODS[i].renderers = LODList.ToArray();
            }
            CurrentLODGroup.SetLODs(ParentLODS);
        }
        //2. Attach LODs to new LOD group
        if (group)
        {
            var ParentLODS = group.GetLODs();
            for (int i = 0; i < ParentLODS.Length; i++)
            {
                var LODList = ParentLODS[i].renderers.ToList();
                foreach (var LOD in LODS.Where(lod => lod.name[lod.name.Length - 1] == i.ToString()[0])) LODList.Add(LOD);
                ParentLODS[i].renderers = LODList.ToArray();
            }
            group.SetLODs(ParentLODS);
        }
        CurrentLODGroup = group;
    }
   protected enum PropState { Equipped, Drop, Inventory}
}
