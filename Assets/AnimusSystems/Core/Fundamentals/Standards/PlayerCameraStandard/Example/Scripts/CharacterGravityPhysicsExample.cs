using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ConstantForce))]
public class CharacterGravityPhysicsExample : MonoBehaviour
{
    public float GUpdatePeriod = 3;

    private ConstantForce force;
    [Range(0, 1)] private float SlerpPower = 0.3f;
    private bool IgnoreGravity { get { return force.force.magnitude < 0.1f; } }

    private Vector3[] GDirections = new Vector3[]
    {
        new Vector3(0,0,0),
        new Vector3(1000,0,0),
        new Vector3(-1000,0,0),
        new Vector3(0,1000,0),
        new Vector3(0,-1000,0),
        new Vector3(0,0,1000),
        new Vector3(0,0,-1000)
    };

    void Start()
    {
        force = GetComponent<ConstantForce>();
        StartCoroutine(GUpdater());
    }

    IEnumerator GUpdater()
    {
        while (true)
        {
            force.force = GDirections[Random.Range(0, GDirections.Length)];
            yield return new WaitForSeconds(GUpdatePeriod);
        }
    }

    void Update()
    {
        if (!IgnoreGravity) transform.up = Vector3.Slerp(transform.up, -force.force,SlerpPower);
    }
}
