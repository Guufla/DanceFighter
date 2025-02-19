using System;
using UnityEngine;

namespace AudioVisuals
{
    public class BasicAudioVisual : AudioVisual
    {
        [SerializeField] private float offset;
        [SerializeField] private bool lookAtPivot;
        

        private void Update()
        {
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