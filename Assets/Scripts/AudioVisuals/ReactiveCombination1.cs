using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utilities;

namespace AudioVisuals
{
    public class ReactiveCombination1 : DebugAV, IColorApply
    {
        [SerializeField] private float perObjectOffset;
        [SerializeField] private bool lookAtPivot;
        [SerializeField] private Transform lineDirFromPivot;
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
                obj.transform.position = pivotTransform.position;
            }
            MakeLine(lineDirFromPivot.position, perObjectOffset, lookAtPivot, true);
            LocalScale(scaledAxes, isNormalized, true);
            LineUpObjects(lineDirFromPivot.position, true);
            ColorGradient(color0, color1);
            HandleAllColors();
        }

        public void Apply(List<Color> colors)
        {
            if (colors.Count < 2)
                return;
            color0 = colors[0];
            color1 = colors[1];
        }
    }
}