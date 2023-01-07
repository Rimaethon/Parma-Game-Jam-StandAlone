using System.Linq;
using UnityEngine;

public class ObjectRaycastPhysics : ObjectCollisionPhysics
{
    [Header("Ray parameters")]
    public LayerMask Mask = Physics.AllLayers;
    public QueryTriggerInteraction TriggerOption;
    public float Thickness = 0;
    public float MaxDistance = 50;
    public CastMode Mode;
    public float Distance { get => hit.collider != null ? hit.distance : MaxDistance; }

    [Header("Hit shot parameters")]
    public Transform HitShot;
    public HitDirectionMode DirectionMode;

    private Vector3 LastPosition;
    private RaycastHit hit;

    private void OnEnable()
    {
        LastPosition = transform.position;
    }

    private void OnDisable()
    {
        if (HitShot && HitShot.gameObject.activeSelf) HitShot.gameObject.SetActive(false);
        hit = default;
        Collisions.Clear();
    }

    private void FixedUpdate()
    {
        //1. Perform Raycast
        var origin = transform.position;
        var direction = transform.forward;
        var distance = MaxDistance;
        if (Mode == CastMode.TrackDeltaPosition)
        {
            origin = LastPosition;
            direction = transform.position - LastPosition;
            distance = direction.magnitude;
        }

        if (Thickness > 0) Physics.SphereCast(origin, Thickness, direction, out hit, distance,Mask,TriggerOption);
        else Physics.Raycast(origin, direction, out hit, distance, Mask, TriggerOption);

        if (hit.collider!=null && Collisions.ContainsKey(hit.collider))
        {
            Collisions[hit.collider].Distance = hit.distance;
            Collisions[hit.collider].Normal = hit.normal;
            Collisions[hit.collider].OtherCollider = hit.collider;
            Collisions[hit.collider].Point = hit.point;
        } else
        {
            if (Collisions.Count>0)
            {
                var first = Collisions.First();
                OnCollisionExitEvent.Invoke(first.Value);
                Collisions.Remove(first.Key);
            }
            if (hit.collider!=null)
            {
                var data =  new CollisionData
                {
                    Distance = hit.distance,
                    Normal = hit.normal,
                    OtherCollider = hit.collider,
                    Point = hit.point
                };
                Collisions.Add(hit.collider, data);
                OnCollisionEnterEvent.Invoke(data);
            }
        }
        
        LastPosition = transform.position;

        if (HitShot)
        {
            if (hit.collider==null && HitShot.gameObject.activeSelf)
            {
                HitShot.gameObject.SetActive(false);
                return;
            }
            if (hit.collider)
            {
                if (!HitShot.gameObject.activeSelf) HitShot.gameObject.SetActive(true);
                HitShot.position = hit.point;
                switch (DirectionMode)
                {
                    case HitDirectionMode.AlongsideNormal: HitShot.forward = hit.normal; break;
                    case HitDirectionMode.SameAsTransform: HitShot.forward = transform.forward; break;
                    case HitDirectionMode.Reflect: HitShot.forward = Vector3.Reflect(transform.forward, hit.normal); break;
                }
            }
        }
    }
    public enum CastMode { Default, TrackDeltaPosition }
    public enum HitDirectionMode { SameAsTransform, AlongsideNormal, Reflect }
}
