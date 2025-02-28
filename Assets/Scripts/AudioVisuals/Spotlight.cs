using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Spotlight : MonoBehaviour
{
    [SerializeField] private Transform lightSource;
    [SerializeField] private Transform pivot1;
    [SerializeField] private Transform pivot2;
    [SerializeField] private Transform end1;
    [SerializeField] private Transform end2;
    [SerializeField] private float lightOffset01;
    [SerializeField] private float maxAngle;
    [SerializeField] private float minAngle;
    [SerializeField] private float angle;

    public void SetAngle(float angle)
    {
        this.angle = Mathf.Clamp(angle, minAngle, maxAngle);
        pivot1.localRotation = Quaternion.Euler(pivot1.localRotation.x, pivot1.localRotation.y, angle); 
        pivot2.localRotation = Quaternion.Euler(pivot2.localRotation.x, pivot2.localRotation.y, -angle);
        
        Vector3 midPoint1 = Vector3.Lerp(pivot1.position, end1.position, 0.5f);
        Vector3 midPoint2 = Vector3.Lerp(pivot2.position, end2.position, 0.5f);
        Vector3 realMidPoint = Vector3.Lerp(midPoint1, midPoint2, 0.5f);
        realMidPoint = Vector3.Lerp(realMidPoint, pivot1.position, Mathf.Clamp01(lightOffset01));
        
        lightSource.position = realMidPoint;
    }

#if UNITY_EDITOR
    private void Update()
    {
        SetAngle(angle);
    }
#endif
    
}
