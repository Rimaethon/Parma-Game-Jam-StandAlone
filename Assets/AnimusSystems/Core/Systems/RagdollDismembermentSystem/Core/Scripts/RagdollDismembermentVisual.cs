using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.Events;

public class RagdollDismembermentVisual : MonoBehaviour {
    [Header("Events")]
    public DismemberOperationEvent OnDismemberCompleted;
    public List<BodyFragment> Fragments;
    private bool isInitialized = false;
    private List<BodyFragment> operations = new List<BodyFragment>();

    private void Reset()
    {
        Fragments = new List<BodyFragment>()
        {
            new BodyFragment()
            {
                color = Color.black,
                Name = "ROOT",
                bone = transform
            }
        };
    }

    private void Awake()
    {
        DismemberManager();
    }

    void Init()
    {
        if (Fragments[0].bone == null) Fragments[0].bone = transform;
        Fragments[0].SkinnedMeshes = Fragments[0].bone.GetComponentsInChildren<SkinnedMeshRenderer>().ToList();
        foreach (var skinnedMesh in Fragments[0].SkinnedMeshes) skinnedMesh.sharedMesh = skinnedMesh.sharedMesh.Copy();
    }
    public void Dismember(string FragmentName)
    {
        var fragment = Fragments.Find(f => f.Name == FragmentName);
        if (fragment != null)
        {
            if (!isInitialized)
            {
                Init();
                isInitialized = true;
            }
            operations.Add(fragment);
        }
        else Debug.LogError("Fragment with name " + FragmentName + " not found!");
    }
    async void DismemberManager()
    {
        while (this)
        {
            if (operations.Count == 0)
            {
                await Task.Yield();
                continue;
            }
            var fragment = operations[0];
            //1. Find parent fragment
            var Parents = Fragments.FindAll(f => f!=fragment && fragment.bone.IsChildOf(f.bone) && f.SkinnedMeshes != null && f.SkinnedMeshes.Count > 0);
            if (Parents == null || Parents.Count == 0)
            {
                operations.RemoveAt(0);
                continue;
            }
            var Parent = fragment.GetNearestParent(Parents.ToArray());

            //2. Cut all parent skinned meshes
            int i = 0;
            while (i < Parent.SkinnedMeshes.Count)
            {
                if (await CutSkinnedMesh(Parent.SkinnedMeshes[i], fragment)) Parent.SkinnedMeshes.RemoveAt(i);
                else i++;
                if (this == null) return;
            }
            //3. Instantiate effects
            if (fragment.BoneEffect)
            {
                var BoneEffect = Instantiate(fragment.BoneEffect, fragment.bone).transform;
                BoneEffect.localPosition = fragment.BoneEffectPosition;
                BoneEffect.localRotation = Quaternion.Euler(fragment.BoneEffectRotation);
                BoneEffect.localScale = fragment.BoneEffectSize;
            }
            if (fragment.BoneParentEffect)
            {
                var BoneParentEffect = Instantiate(fragment.BoneParentEffect, fragment.bone.parent).transform;
                BoneParentEffect.localPosition = fragment.BoneParentEffectPosition;
                BoneParentEffect.localRotation = Quaternion.Euler(fragment.BoneParentEffectRotation);
                BoneParentEffect.localScale = fragment.BoneParentEffectSize;
            }
            //4. Create LOD group if needed
            var LODGroup = GetComponentInChildren<LODGroup>();
            if (LODGroup != null) CloneLODGroupForFragment(fragment, LODGroup);
            //5. Disconnect fragment
            fragment.bone.SetParent(Fragments[0].bone.parent);
            OnDismemberCompleted.Invoke(fragment.Name);
            operations.RemoveAt(0);
        }
    }

    void CloneLODGroupForFragment(BodyFragment fragment, LODGroup original)
    {
        var LODGroup = fragment.bone.gameObject.AddComponent<LODGroup>();
        var LODs = original.GetLODs();
        for (int i=0; i<LODs.Length; i++)
        {
            LODs[i].renderers = fragment.SkinnedMeshes.Where(m => m.name[m.name.Length - 1].ToString() == i.ToString()).ToArray();
        }
        LODGroup.SetLODs(LODs);
    }
    public bool[] SelectionMask;
    public int SelectionMaskLength;
    public void GetSelection(SkinnedMeshRenderer skinnedMesh, BodyFragment fragment, SelectionMode mode= SelectionMode.Replace)
    {
        var Mesh = skinnedMesh.sharedMesh;
        var Vertices = Mesh.vertices;
        if (SelectionMask == null || Vertices.Length > SelectionMask.Length)
        {
            SelectionMask = new bool[Vertices.Length];
            SelectionMaskLength = Vertices.Length;
        }
        else
        {
            SelectionMaskLength = Vertices.Length;
        }

        if (mode == SelectionMode.Replace) for (int i = 0; i < Vertices.Length; i++) SelectionMask[i] = false;
        if (fragment.bone == null || fragment.Size.x==0 || fragment.Size.y==0 || fragment.Size.z==0) return;
        
        var bindposeIndex = System.Array.FindIndex(skinnedMesh.bones, b => b == fragment.bone);
        if (bindposeIndex == -1) return;
        
        var Tris = Mesh.triangles;

        var M = Matrix4x4.Inverse(Matrix4x4.TRS(fragment.Position, Quaternion.Euler(fragment.Rotation), fragment.Size))*  Mesh.bindposes[bindposeIndex];
        Vector3 point;

        for (int i=0; i<Tris.Length;i+=3)
        {
            var p = (Vertices[Tris[i]]+Vertices[Tris[i+1]]+Vertices[Tris[i+2]])/3.0f;
            
            point.x = M.m00 * p.x + M.m01 * p.y + M.m02 * p.z + M.m03;
            point.y = M.m10 * p.x + M.m11 * p.y + M.m12 * p.z + M.m13;
            point.z = M.m20 * p.x + M.m21 * p.y + M.m22 * p.z + M.m23;

            if (point.x>-0.5 && point.x<0.5
                && point.y>-0.5 && point.y<0.5 &&
                point.z>-0.5 && point.z<0.5) {

                SelectionMask[Tris[i]] = true;
                SelectionMask[Tris[i + 1]] = true;
                SelectionMask[Tris[i + 2]] = true;
            }
        }
    }
    public SelectionStatus GetSelectionStatus()
    {
        var result = SelectionStatus.Empty;
        for (int i=0; i<SelectionMaskLength; i++)
        {
            if (result == SelectionStatus.Empty && SelectionMask[i]) result = SelectionStatus.Mixed;
            else if (result == SelectionStatus.Mixed && !SelectionMask[i]) return result;
            else if (result == SelectionStatus.Mixed && i == SelectionMaskLength - 1) result = SelectionStatus.Full;
        }
        return result;
    }

    async Task<bool> CutSkinnedMesh(SkinnedMeshRenderer skinnedMesh, BodyFragment fragment)
    {
        //1. Get all child fragments
        var ChildFragments = Fragments.FindAll(f => f.bone.IsChildOf(fragment.bone));
        //2. Get vertex selection of all child fragments
        for (int i = 0; i < ChildFragments.Count; i++)
        {
            GetSelection(skinnedMesh, ChildFragments[i], i == 0 ? SelectionMode.Replace : SelectionMode.Add);
        }
        var status = GetSelectionStatus();
        //3. If selection is empty, then don't do anything
        if (status== SelectionStatus.Empty)
        {
            ReplaceBindposes(skinnedMesh, fragment.bone, BindposeReplacementMode.ReplaceChildren);
            return false;
        }
        //4. Copy skinned mesh
        fragment.SkinnedMeshes.Add(skinnedMesh.Copy(fragment.bone.gameObject));
        fragment.SkinnedMeshes[fragment.SkinnedMeshes.Count - 1].rootBone = fragment.bone;
        //5. If selection covers entire mesh, then destroy parent skinned mesh
        if (status== SelectionStatus.Full)
        {
            Destroy(skinnedMesh);
            ReplaceBindposes(fragment.SkinnedMeshes[fragment.SkinnedMeshes.Count - 1], fragment.bone, BindposeReplacementMode.ReplaceParents);
            return true;
        }
        //6. Separate selection into fragment shared mesh
        var mesh = await skinnedMesh.sharedMesh.SimpleSplit(SelectionMask);
        if (this == null) return false;
        fragment.SkinnedMeshes[fragment.SkinnedMeshes.Count - 1].sharedMesh = mesh; 
        //7. Replace bindposes for parent mesh
        ReplaceBindposes(skinnedMesh, fragment.bone, BindposeReplacementMode.ReplaceChildren);
        //8. Replace bindposes for fragment mesh
        ReplaceBindposes(fragment.SkinnedMeshes[fragment.SkinnedMeshes.Count - 1], fragment.bone, BindposeReplacementMode.ReplaceParents);
        return false;
    }

    void ReplaceBindposes(SkinnedMeshRenderer skinnedMesh, Transform CrackedBone, BindposeReplacementMode mode)
    {
        var bones = skinnedMesh.bones;
        var bindposes = skinnedMesh.sharedMesh.bindposes;
        var index = System.Array.FindIndex(bones, b => b == (mode== BindposeReplacementMode.ReplaceParents? CrackedBone:CrackedBone.parent));
        for (int i = 0; i < bones.Length; i++)
        {
           
            switch (mode)
            {
                case BindposeReplacementMode.ReplaceChildren:
                    if (bones[i].IsChildOf(CrackedBone))
                    {
                        bones[i] = CrackedBone.parent;
                        bindposes[i] = bindposes[index];
                    }
                    break;
                case BindposeReplacementMode.ReplaceParents:
                    if (!bones[i].IsChildOf(CrackedBone))
                    {
                        bones[i] = CrackedBone;
                        bindposes[i] = bindposes[index];
                    }
                    break;
            }
        }
        skinnedMesh.bones = bones;
        skinnedMesh.sharedMesh.bindposes = bindposes;
    }
    enum BindposeReplacementMode { ReplaceParents, ReplaceChildren }
    public enum SelectionMode { Add, Replace }
    public enum SelectionStatus { Empty, Mixed, Full }
    [System.Serializable]
    public class DismemberOperationEvent : UnityEvent<string> { };
}
[System.Serializable]
public class BodyFragment
{
    public bool ShowWireframe = true;
    public bool ShowProperties;
    public string Name;
    public Transform bone;

    public bool BoundsDetails;
    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Size;

    public GameObject BoneEffect;
    public bool BoneEffectDetails;
    public Vector3 BoneEffectPosition;
    public Vector3 BoneEffectRotation;
    public Vector3 BoneEffectSize;

    public GameObject BoneParentEffect;
    public bool BoneParentEffectDetails;
    public Vector3 BoneParentEffectPosition;
    public Vector3 BoneParentEffectRotation;
    public Vector3 BoneParentEffectSize;

    public Color color;
    public List<SkinnedMeshRenderer> SkinnedMeshes;

    public BodyFragment GetNearestParent(params BodyFragment[] Parents)
    {
        var Parent = bone.parent;
        while (Parent != null)
        {
            for (int i = 0; i < Parents.Length; i++) if (Parent == Parents[i].bone) return Parents[i];
            Parent = Parent.parent;
        }
        return null;
    }
}
