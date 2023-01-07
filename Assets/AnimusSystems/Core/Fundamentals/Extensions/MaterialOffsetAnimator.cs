using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MaterialOffsetAnimator : MonoBehaviour
{
    public int Frames = 5;
    public float Framerate = 0.1f;
    public OffsetAxis Axis = OffsetAxis.Y;

    private Renderer renderer;
    private int currentFrame = 0;

    private void OnEnable()
    {
        renderer = GetComponent<Renderer>();
        StartCoroutine(Animation());
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    IEnumerator Animation()
    {
        while (enabled)
        {
            renderer.material.SetTextureOffset("_MainTex", (1.0f * currentFrame / Frames) * (Axis == OffsetAxis.X ? Vector2.right : Vector2.up));
            yield return new WaitForSecondsRealtime(Framerate);
            currentFrame = (currentFrame + 1) % Frames;
        }
    }
    public enum OffsetAxis { X, Y }
}
