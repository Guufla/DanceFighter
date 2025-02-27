using System;
using TMPro;
using UnityEngine;
using Utilities;

namespace AudioVisuals
{
    public class DebugAV : AudioVisual
    {
        [SerializeField] private TMP_Text text;

        protected override void Update()
        {
            base.Update();
            text.text = GetDebugInfo();
        }
    }
}