using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class ConstraintRectUI : MonoBehaviour
{
    public RectTransform Source;
    private RectTransform target;

    private void Awake()
    {
        target = GetComponent<RectTransform>();
    }

    public void Update()
    {
        if (Source == null) return;
        target.anchoredPosition3D = Source.anchoredPosition3D;
        target.anchorMin = Source.anchorMin;
        target.anchorMax = Source.anchorMax;
        target.sizeDelta = Source.sizeDelta;
        target.localRotation = Source.localRotation;
    }
}
