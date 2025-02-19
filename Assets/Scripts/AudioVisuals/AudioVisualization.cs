using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisualization : MonoBehaviour
{
    /*
     * Notes on what values you could change based on the spectrum data:
     * 
     *      - Change velocity of constantly moving object/light/visual.
     *
     *          - Angular velocity of a circle-ish shape that also takes into
     *              account which side has offense/defense mode.
     * 
     *      - Change intensity/color of light.
     *
     *      - Size of an object.
     *
     *      - Scale alpha value of some visual.
     *
     *      - Add (or multiply?) a value on top of the original audio visuals
     *          that represents the player hits.
     */
    
    
    
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

    private void OnEnable()
    {
        Init();
    }

    private void OnDisable()
    {
        ToggleVisuals(false);
    }

    // private void Start()
    // {
    //     SetAndScaleVisuals();
    // }

    private void Update()
    {
        // first code to execute
        CheckToReInit(); // check to reset things
        UpdateSpectrumData(); // set data before using it

        SetAndScaleVisuals();

        VisualizeScale();
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

    private void VisualizeScale()
    {
        for (int i = 0; i < sampleSize; ++i)
        {
            float audioScale = spectrumData[i] * audioSampleScale;
            //visuals[i].transform.localScale = Vector3.Scale(visuals[i].transform.localScale, new Vector3(audioScale, audioScale, 1));
            visualObjects[i].transform.localScale = new Vector3(this.scale, this.scale + audioScale, 1);
        }
    }


    private void UpdateSpectrumData()
    {
        audioSource.GetSpectrumData(spectrumData, 0, fftWindowToUse);
    }
    

    private void Init()
    {
        // keep sampleSize a power of 2 and between 64 and 8192 (requirement of GetSpectrumData) (i did NOT make this equation lol)
        sampleSize = (int)Mathf.Clamp(Mathf.Pow(2, Mathf.Ceil(Mathf.Log(sampleSize) / Mathf.Log(2))), 64, 8192);
        renderingSize = Mathf.Clamp(renderingSize, 0, sampleSize);

        spectrumData = new float[sampleSize]; // allocate space for array
        
        // delete old objects if they exist
        for (int i = 0; i < visualObjects.Count; ++i)
        {
            Destroy(visualObjects[i]);
        }

        visualObjects.Clear();

        // make objects for each renderingSize
        for (int i = 0; i < renderingSize; ++i)
        {
            visualObjects.Add(GameObject.Instantiate(templateObject, this.transform));
        }
        
        ToggleVisuals(true);
    }

    private void CheckToReInit()
    {
        if (visualObjects.Count != renderingSize)
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