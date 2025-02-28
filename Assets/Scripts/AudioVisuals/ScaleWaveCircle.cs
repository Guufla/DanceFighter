using System;
using TMPro;
using UnityEngine;
using Utilities;

namespace AudioVisuals
{
    public class ScaleWaveCircle : WaveCircle
    {
        [SerializeField] private Vector3 defaultScale = Vector3.one;
        [SerializeField] private Vector3 scaledAxes;
        [SerializeField] private bool isNormalized;
        
        protected override void Update()
        {
            base.Update();

            foreach (GameObject obj in visualObjs)
            {
                obj.transform.localScale = defaultScale;
            }
            LocalScale(scaledAxes, isNormalized, true);
        }
    }
}