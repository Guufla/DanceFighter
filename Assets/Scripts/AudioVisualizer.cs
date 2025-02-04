using UnityEngine;
using System.Collections.Generic;


public class AudioVisualizer : MonoBehaviour
{
    public enum VisualShape
    {
        Circle,
        Line
    }
    
    
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject templateObject;

    [SerializeField] private FFTWindow fftWindowToUse;
    [SerializeField] private VisualShape visualShape;
    [SerializeField] private float offset = 5;
    [SerializeField] private float scale = 1;
    [SerializeField] private float audioSampleScale = 1;
    [SerializeField] private int sampleSize = 256;
    [SerializeField] private int renderingSize = 256;

    private List<GameObject> visualObjects = new List<GameObject>();
    private float[] spectrumData;




    // only do when needed
    private void InstanceObjects()
    {
        ConsoleLogger.Log("AudioVisualizer: InstanceObjects called... Remember to limit this.", true);
        
    }
    
    private void UpdateSpectrumData()
    {
        audioSource.GetSpectrumData(spectrumData, 0, fftWindowToUse);
    }
}