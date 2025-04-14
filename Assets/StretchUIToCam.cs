using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StretchUIToCam : MonoBehaviour
{
    [SerializeField] private UnityEngine.Camera cam;
    [SerializeField] private RectTransform rectTransform;

    [SerializeField] private float referenceCameraSize;
    [SerializeField] private float referenceRectTransformScale;

    private float camToRectRatio; //367
    
    private void Start()
    {
        camToRectRatio = referenceCameraSize / referenceRectTransformScale;
    }

    private void Update()
    {
        float scale = cam.orthographicSize / camToRectRatio;
        rectTransform.localScale = new Vector3(scale, scale, 1);
    }
}
