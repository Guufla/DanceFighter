using System;
using TMPro;
using UnityEngine;
using Utilities;

namespace AudioVisuals
{
    public class ReactiveCombination1 : LineAV
    {
        [SerializeField] private Vector3 defaultScale = Vector3.one;
        [SerializeField] private Vector3 scaledAxes;
        [SerializeField] private bool isNormalized;
        [SerializeField] private Color color0;
        [SerializeField] private Color color1;

        protected override void Update()
        {
            base.Update();

            foreach (GameObject obj in visualObjs)
            {
                obj.transform.localScale = defaultScale;
            }
            LocalScale(scaledAxes, isNormalized, true);
            LineUpObjects(lineDirFromPivot.position);
            ColorGradient(color0, color1);
            HandleAllColors();
        }
    }
}