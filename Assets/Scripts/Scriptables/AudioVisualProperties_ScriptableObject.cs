using AudioVisuals;
using UnityEngine;
using Utilities;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "AudioVisual", menuName = "AudioVisualProperties_ScriptableObject", order = 0)]
    public class AudioVisualProperties_ScriptableObject : ScriptableObject, IAudioVisual, IObjectPooler
    {
        // Audio Visual
        
        [field: SerializeField] public int TargetRenderSize { get; set; }
        
        // Object Pooler

        [field: SerializeField] public int TargetSize { get; set; }
        [field: SerializeField] public int ResizeThreshold { get; set; }
        [field: SerializeField] public float ResizeRate { get; set; }
        [field: SerializeField] public GameObject ObjectToPool { get; set; }
    }
}