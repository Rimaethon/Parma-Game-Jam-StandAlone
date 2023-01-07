using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ObjectColorVisual : MonoBehaviour
{
    public int MaterialSlot = 0;
    public Color AlbedoColor = Color.white;
    public Color EmissionColor = Color.white;
    [Range(0, 1)] public float EmissionPower = 0;
    private Renderer renderer;
    private LineRenderer lineRenderer;

    void Start()
    {
        renderer = GetComponent<Renderer>();
        if (renderer is LineRenderer) lineRenderer = renderer as LineRenderer;
        
    }

    void Update()
    {
        if (lineRenderer)
        {
            lineRenderer.startColor = AlbedoColor;
            lineRenderer.endColor = AlbedoColor;
        }
        else
        {
            renderer.materials[MaterialSlot].color = AlbedoColor;
            renderer.materials[MaterialSlot].SetColor("_EmissionColor", EmissionColor * EmissionPower);
        }
    }
}
