using System;
using TMPro;
using UnityEngine;
using Utilities;

namespace AudioVisuals
{
    public class ColorAV : CircleAV
    {
        [SerializeField] private Color color1;
        [SerializeField] private Color color2;
        
        protected override void Update()
        {
            base.Update();
            
            ColorGradient(color1, color2);
            HandleAllColors();
        }
    }
}