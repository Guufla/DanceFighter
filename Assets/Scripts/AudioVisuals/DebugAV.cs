using System;
using TMPro;
using UnityEngine;
using Utilities;

namespace AudioVisuals
{
    public class DebugAV : AudioVisual
    {
        [SerializeField] private TMP_Text text = null;

        protected override void Update()
        {
            base.Update();
            if (text is not null)
                text.text = GetDebugInfo();
        }
    }
}