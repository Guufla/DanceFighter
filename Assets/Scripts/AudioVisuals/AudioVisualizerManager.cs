using System;
using UnityEngine;
using System.Collections.Generic;


public class AudioVisualizerManager : MonoBehaviour
{
    
    
    
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject templateObject;

    [SerializeField] private FFTWindow fftWindowToUse;
    [SerializeField] private float offset = 5;
    [SerializeField] private float scale = 1;
    [SerializeField] private float audioSampleScale = 1;
    [SerializeField] private int sampleSize = 256;
    [SerializeField] private int renderingSize = 256;
    [SerializeField] private int poolDeltaThreshold;

    private GameObject[] objectPool;
    private GameObject[] renderedObjects;
    private float[] spectrumData;
    

    private void HandleObjectsVisibility()
    {
        
    }

    private void ShapeFormatObject(GameObject obj)
    {
        
    }
    
    private void UpdateSpectrumData()
    {
        audioSource.GetSpectrumData(spectrumData, 0, fftWindowToUse);
    }
}

