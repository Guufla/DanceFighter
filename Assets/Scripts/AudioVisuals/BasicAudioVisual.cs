using System;
using UnityEngine;

namespace AudioVisuals
{
    public class BasicAudioVisual : AudioVisual
    {
        [SerializeField] private float offset;
        [SerializeField] private bool lookAtPivot;
        [SerializeField] private int debug_targetSize;

        private int debug_lastTargetSize;
        

        private void Update()
        {
            Debug();
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

        private void Debug()
        {
            if (debug_lastTargetSize != debug_targetSize)
            {
                this.TargetRenderSize = debug_targetSize;
            }
            debug_lastTargetSize = debug_targetSize;
        }
    }
}