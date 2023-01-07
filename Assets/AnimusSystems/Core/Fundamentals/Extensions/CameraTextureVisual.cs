using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTextureVisual : MonoBehaviour
{
    public Renderer Renderer;
    public Vector2 Resolution = new Vector2(256, 256);
    private RenderTexture texture;

    void Start()
    {
        texture = new RenderTexture((int)Resolution.x, (int)Resolution.y, 16, RenderTextureFormat.ARGB32);
        texture.Create();
        GetComponent<Camera>().targetTexture = texture;
        Renderer.material.mainTexture = texture;
    }
}
