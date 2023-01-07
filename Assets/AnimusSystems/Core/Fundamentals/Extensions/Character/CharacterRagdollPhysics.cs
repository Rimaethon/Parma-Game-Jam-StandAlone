using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterRagdollPhysics : MonoBehaviour
{

    public GameObject RagdollPrefab;
    [SerializeField,ReadOnlyField]private GameObject ragdollInstance;
    public string RagdollRootBonePath;
    public Transform AnimatorRootBone;
    public bool DisableCollidersWhenKinematic = true;
    private List<RagdollLink> links = new List<RagdollLink>();
    private Rigidbody RootRigidbody;

    private void Awake()
    {
        RootRigidbody = GetComponentInParent<AvatarOverallInput>()?.GetComponent<Rigidbody>();
        CreateRagdollInstance();
        links.Clear();
        var ragdollBones =  ragdollInstance.transform.FindByPath(RagdollRootBonePath).GetComponentsInChildren<Transform>();
        var animatorBones = AnimatorRootBone.GetComponentsInChildren<Transform>();
        foreach (var ragdollBone in ragdollBones)
        {
            var animatorBone = animatorBones.FirstOrDefault(b => b.name == ragdollBone.name);
            if (animatorBone)
            {
                links.Add(new RagdollLink {
                    AnimatorBone = animatorBone,
                    RagdollBone = ragdollBone,
                    Collider = ragdollBone.GetComponent<Collider>(),
                    Rigidbody = ragdollBone.GetComponent<Rigidbody>()
                });
            }
        }
    }
    public void ResetRagdoll()
    {
        Destroy(ragdollInstance);
        ragdollInstance = null;
        Awake();
    }
    public void CreateRagdollInstance()
    {
        if (!ragdollInstance) ragdollInstance = Instantiate(RagdollPrefab, transform);
    }
    void EnableRagdoll (bool active)
    {
        foreach (var link in links)
        {
            link.SetKinematic(!active, RootRigidbody ? RootRigidbody.velocity : Vector3.zero, DisableCollidersWhenKinematic);
        }
    }
    private void OnEnable()
    {
        EnableRagdoll(false);
    }
    private void OnDisable()
    {
        EnableRagdoll(true);
    }
    public void DetachCollider(Collider collider)
    {
        var detachedLinks = links.Where(l => l.RagdollBone.IsChildOf(collider.transform));
        foreach (var link in detachedLinks)
        {
            link.SetKinematic(false, Vector3.zero, DisableCollidersWhenKinematic);
        }
        links = links.Except(detachedLinks).ToList();
    }

    void LateUpdate()
    {
        for (int i=0; i<links.Count;i++)
        {
            links[i].RagdollBone.SetPositionAndRotation(links[i].AnimatorBone.position, links[i].AnimatorBone.rotation);
        }
    }
    class RagdollLink
    {
        public Transform AnimatorBone;
        public Transform RagdollBone;
        public Rigidbody Rigidbody;
        public Collider Collider;

        public void SetKinematic(bool isKinematic, Vector3 StartVelocity, bool disableKinematicCollider)
        {
            if (Rigidbody)
            {
                Rigidbody.isKinematic = isKinematic;
                Rigidbody.velocity = StartVelocity;
                if (disableKinematicCollider)
                {
                    Collider.enabled = !isKinematic;
                } else
                {
                    Collider.enabled = true;
                }
            }
        }
    }
}
