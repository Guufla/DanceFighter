using System;
using TMPro;
using UnityEngine;
using Utilities;

namespace AudioVisuals
{
    public class CircleAV : DebugAudioVisual
    {
        // TODO: How to set properties in inspector

        [SerializeField] private float offset;
        [SerializeField] private bool lookAtPivot;

        protected override void Update()
        {
            base.Update();
            MakeCircle(offset, lookAtPivot);
        }

        protected override void MakeCircle(float offsetFromPivot, bool lookTowardsPivot)
        {
            base.MakeCircle(offsetFromPivot, lookTowardsPivot);

            foreach (GameObject obj in visualObjs)
            {
                obj.SetActive(true);
            }
        }
    }
}