using System;
using TMPro;
using UnityEngine;
using Utilities;

namespace AudioVisuals
{
    public class CircleAV : DebugAV
    {
        [SerializeField] private float offset;
        [SerializeField] private bool lookAtPivot;

        protected override void Update()
        {
            base.Update();
            MakeCircle(offset, lookAtPivot);
        }
    }
}