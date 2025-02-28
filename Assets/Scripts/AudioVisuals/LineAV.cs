using System;
using TMPro;
using UnityEngine;
using Utilities;

namespace AudioVisuals
{
    public class LineAV : DebugAV
    {
        [SerializeField] private float perObjectOffset;
        [SerializeField] private bool lookAtPivot;
        [SerializeField] protected Transform lineDirFromPivot;

        protected override void Update()
        {
            base.Update();
            MakeLine(lineDirFromPivot.position, perObjectOffset, lookAtPivot);
        }
    }
}