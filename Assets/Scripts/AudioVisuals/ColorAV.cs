using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

namespace AudioVisuals
{
    public class ColorAV : CircleAV
    {
        [SerializeField] private Color color0;
        [SerializeField] private Color color1;
        
        protected override void Update()
        {
            base.Update();
            
            ColorGradient(color0, color1);
            HandleAllColors();
        }
    }
}