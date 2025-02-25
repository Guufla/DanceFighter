using System;
using TMPro;
using UnityEngine;
using Utilities;

namespace AudioVisuals
{
    public class DebugAudioVisual : AudioVisual
    {
        // TODO: How to set properties in inspector
        
        [SerializeField] private int debug_targetSize;
        [SerializeField] private int debug_resizeThreshold;
        [SerializeField] private float debug_resizeRate;
        [SerializeField] private TMP_Text debug_text;

        private int debug_lastTargetSize;
        private int debug_lastresizeThreshold;
        private float debug_lastresizeRate;


        protected override void Update()
        {
            base.Update();
            Debug();
        }

        private void Debug()
        {
            if (debug_lastTargetSize != debug_targetSize)
            {
                this.TargetRenderSize = debug_targetSize;
            }

            if (this.objectPooler is not null)
            {
                if (debug_lastresizeThreshold != debug_resizeThreshold)
                {
                    this.objectPooler.ResizeThreshold = debug_resizeThreshold;
                }
                if (debug_lastresizeRate != debug_resizeRate)
                {
                    this.objectPooler.ResizeRate = debug_resizeRate;
                }
            }

            debug_lastTargetSize = debug_targetSize;
            debug_lastresizeRate = debug_resizeRate;
            debug_lastresizeThreshold = debug_resizeThreshold;
            
            debug_text.text = GetDebugInfo();
        }
    }
}