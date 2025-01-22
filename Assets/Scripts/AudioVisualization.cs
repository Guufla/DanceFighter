using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisualization : MonoBehaviour
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

    private List<GameObject> visualObjects = new List<GameObject>();
    private float[] spectrumData;

    private void OnEnable()
    {
        Init();
    }

    private void OnDisable()
    {
        ToggleVisuals(false);
    }

    private void Start()
    {
        SetAndScaleVisuals();
    }

    private void SetAndScaleVisuals()
    {
        switch (visualShape)
        {
            case VisualShape.Circle:
                CircleVisual();
                break;
            case VisualShape.Line:
                LineVisual();
                break;
        }

        for (int i = 0; i < sampleSize; ++i)
        {
            // check if above the ignore thresh
            if (spectrumData[i] > 0.1f)
            {
                visualObjects[i].SetActive(true);
            }
        }
    }
    
    private void CircleVisual()
    {
        float radianStep = (2 * Mathf.PI) / sampleSize;
        float currRadian = 0;
        
        for (int i = 0; i < sampleSize; ++i)
        {
            Vector3 circ = new Vector3(Mathf.Cos(currRadian), Mathf.Sin(currRadian), 0);
            Vector3 objPos = circ * offset + this.transform.position;
            
            visualObjects[i].transform.position = objPos;
            visualObjects[i].transform.rotation = Quaternion.LookRotation(this.transform.position - objPos);
            
            
            currRadian += radianStep;
        }
    }

    private void LineVisual()
    {
        float distanceStep = offset / sampleSize;
        float distance = 0;

        for (int i = 0; i < sampleSize; ++i)
        {
            visualObjects[i].transform.position = this.transform.position + new Vector3(distance, 0, 0);
            
            distance += distanceStep;
        }
    }

    private void Update()
    {
        // first code to execute
        CheckToInit();
        UpdateSpectrumData();
        
        SetAndScaleVisuals();
        
        VisualizeScale();
    }

    private void UpdateSpectrumData()
    {
        audioSource.GetSpectrumData(spectrumData, 0, fftWindowToUse);
    }

    private void VisualizeScale()
    {
        for (int i = 0; i < sampleSize; ++i)
        {
            float audioScale = spectrumData[i] * audioSampleScale;
            //visuals[i].transform.localScale = Vector3.Scale(visuals[i].transform.localScale, new Vector3(audioScale, audioScale, 1));
            visualObjects[i].transform.localScale = new Vector3(this.scale, this.scale + audioScale, 1);
        }
    }
    
    private void Init()
    {
        // keep sampleSize a power of 2 and between 64 and 8192 (requirement of GetSpectrumData)
        sampleSize = (int)Mathf.Clamp(Mathf.Pow(2, Mathf.Ceil(Mathf.Log(sampleSize) / Mathf.Log(2))), 64, 8192);
        
        spectrumData = new float[sampleSize]; // allocate space for array
        
        if (visualObjects.Count != sampleSize)
        {
            // delete old objects
            for (int i = 0; i < visualObjects.Count; ++i)
            {
                Destroy(visualObjects[i]);
            }
            visualObjects.Clear();
            
            // make objects for each sample
            for (int i = 0; i < sampleSize; ++i)
            {
                visualObjects.Add(GameObject.Instantiate(templateObject, this.transform));
            }
        }
        ToggleVisuals(true);
    }

    private void CheckToInit()
    {
        if (visualObjects.Count != sampleSize)
        {
            Init();
        }
    }
    
    private void ToggleVisuals(bool state)
    {
        for (int i = 0; i < sampleSize; ++i)
        {
            visualObjects[i].SetActive(state);
        }
    }
}
