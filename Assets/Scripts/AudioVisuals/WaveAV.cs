using System;
using TMPro;
using UnityEngine;
using Utilities;

namespace AudioVisuals
{
    public class WaveAV : DebugAV
    {
        [SerializeField] private float perObjectOffset;
        [SerializeField] private bool lookAtPivot;
        [SerializeField] private float radianOffset;
        [SerializeField] private bool loop;
        [SerializeField] private float speed;
        [SerializeField] private Transform lineDirFromPivot;

        private float radOffset;

        protected override void Update()
        {
            base.Update();
            
            if (loop)
            {
                radOffset += Time.deltaTime * speed;
                radOffset %= (2 * Mathf.PI);
            }
            else
            {
                radOffset = radianOffset;
            }
            MakeWave(lineDirFromPivot.position, perObjectOffset, lookAtPivot, radOffset);
        }
    }
}