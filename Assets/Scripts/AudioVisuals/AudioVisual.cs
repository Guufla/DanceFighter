using System;
using System.Collections.Generic;
using Scriptables;
using UnityEngine;
using Utilities;

namespace AudioVisuals
{
    public interface IAudioVisual
    {
        public int TargetRenderSize { get; set; }
    }
    
    /// <summary>
    /// Base Audio Visualization class to be inherited and used by other classes. Idea is you can customize, stack, and do whatever you want
    /// to this from within the engine and inspector to create cool effects.
    /// </summary>
    public abstract class AudioVisual : MonoBehaviour, IAudioVisual, IObjectPooler
    {
        /*
         * Shapes:
         *      - Circle
         *      - Line
         *      - StaticWave
         *      - OscillatingWave
         *      - Random Noise Movement
         *
         * Effects:
         *      - ScaleSingleAxis
         *      - ScaleMultiAxes
         *      - ColorGradient
         *      - Transparency
         *
         * Code Idea/Goal:
         *      Make every shape and effect function be able to mix with each other (additive style).
         *
         * Notes:
         *      - Swapped "objectPooler != null" checks to "objectPooler is not null" checks:
         *          https://stackoverflow.com/questions/75013054/is-it-cost-expensive-to-use-if-gameobject-null
         */
        
        
        public int TargetRenderSize
        {
            get => _targetRenderSize;
            set
            {
                if (objectPooler is not null)
                    objectPooler.TargetSize = value;
                _targetRenderSize = value;
            }
        }

        public int TargetSize
        {
            get
            {
                return objectPooler is not null ? objectPooler.TargetSize : -1;
            }
            set
            {
                ConsoleLogger.Log("Don't set TargetSize here. Set TargetRenderSize instead.", true);
            }
        }
        public int ResizeThreshold
        {
            get
            {
                return objectPooler is not null ? objectPooler.ResizeThreshold : -1;
            }
            set
            {
                if (objectPooler is not null)
                {
                    objectPooler.ResizeThreshold = value;
                }
            }
        }
        public float ResizeRate
        {
            get
            {
                return objectPooler is not null ? objectPooler.ResizeRate : -1;
            }
            set
            {
                if (objectPooler is not null)
                {
                    objectPooler.ResizeRate = value;
                }
            }
        }

        public GameObject ObjectToPool
        {
            get { return objectPooler is not null ? objectPooler.ObjectToPool : visualObjectPrefab; }
            set
            {
                if (objectPooler is not null)
                {
                    objectPooler.ObjectToPool = value;
                }

                visualObjectPrefab = value;
            }
        }

        [SerializeField] protected Transform[] presetTransforms;
        [SerializeField] protected Transform pivotTransform;

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioVisualProperties_ScriptableObject audioVisualPropertiesRef;
        [SerializeField] private bool isPresetShape;
        [Range(0.0F, 1.0F)] [SerializeField]
        private float specDataCutoff; // defines what percent of the spec data to cut off from the right
        [SerializeField] private FFTWindow fftWindowToUse; // This enum values rank in ascending order of precision for spec data processing (Triangle seems fine).
        [SerializeField] private bool doPropertyValueRecheck = true; // mostly for rapid testing in editor. Set false to save operations.

        protected List<GameObject> visualObjs = new List<GameObject>();
        protected float[] usableSpectrumData; // the spec data that will actually be used in the visuals

        private Utilities.ObjectPooler objectPooler = null;
        private GameObject visualObjectPrefab;
        private int _targetRenderSize;
        private int sampleSize;
        private float[] spectrumData;
        private List<List<Color>> colorsToBlendPerObject = new List<List<Color>>();
        
        protected virtual void OnDestroy()
        {
            RemoveObjectPool();
        }

        private void Start()
        {
            SetPropertyValues();
        }

        // must call base.Update() if you are overriding this in child class
        protected virtual void Update()
        {
            if (doPropertyValueRecheck)
            {
                SetPropertyValues();
            }
            UpdateSpectrumData();
        }

        public string GetDebugInfo()
        {
            string output = "";
            output += $"Target Render Size: {TargetRenderSize}\n";
            output += $"Sample Size: {sampleSize}\n";
            output += $"Spectrum Data Length: {spectrumData.Length}\n";
            output += $"Usable Spectrum Data Length: {usableSpectrumData.Length}\n";
            output += $"Visible Objects Count: {visualObjs.Count}\n";
            if (objectPooler is not null)
            {
                output += $"Object Pool Size: {objectPooler.TargetSize}\n";
                output += $"Object Pool Resize Rate: {objectPooler.ResizeRate}\n";
                output += $"Object Pool Resize Threshold: {objectPooler.ResizeThreshold}\n";
            }
            else
            {
                output += $"Object Pool is null\n";
            }
            output += $"Spectrum Data Cutoff: {specDataCutoff * 100}%\n";
            output += $"FFTWindow being used: {fftWindowToUse}\n";
            output += $"Is Preset Shape: {isPresetShape}\n";
            return output;
        }


        // TODO: Add actions that you can pass in to be able to run in the loop per object!
        // TODO: Add support for multiplicative steps (like how we have additive already).
        protected virtual void MakeCircle(float offsetFromPivot, bool lookTowardsPivot, bool isAdditive = false) 
        {
            HandleObjects();
            
            float radianStep = (2 * Mathf.PI) / visualObjs.Count;
            float currRadian = 0;

            for (int i = 0; i < visualObjs.Count; ++i)
            {
                Vector3 circle = new Vector3(Mathf.Cos(currRadian), Mathf.Sin(currRadian), 0);
                Vector3 circleAndOffset = circle * offsetFromPivot;
                Vector3 objPos = pivotTransform.position + circleAndOffset;

                HandleAdditivePosition(visualObjs[i].transform, objPos, isAdditive); // zero out if this is not an "additive" step
                visualObjs[i].SetActive(true);

                if (lookTowardsPivot)
                {
                    Vector3 objectToPivotDir =  pivotTransform.position - visualObjs[i].transform.position;
                    visualObjs[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, objectToPivotDir);
                    
                    // cool effect if you rotate the spotlights forward to be facing towards/away from the screen in 2D
                }
            
                currRadian += radianStep;
            }
        }

        protected virtual void MakeLine(Vector3 dirFromPivot, float perObjectOffset, bool lookTowardsPivot, bool isAdditive = false)
        {
            HandleObjects();
            
            Vector3 dir = (dirFromPivot - pivotTransform.position).normalized;
            Vector3 scaledDir = dir * perObjectOffset; // essentially the step between two objects in direction 'dir'
            float totalDist = perObjectOffset * Mathf.Clamp((visualObjs.Count - 1), 0, Int32.MaxValue);
            Vector3 incrementPos = totalDist * 0.5f * dir; // initialized to starting position
            for (int i = 0; i < visualObjs.Count; ++i)
            {
                HandleAdditivePosition(visualObjs[i].transform, incrementPos, isAdditive); // zero out if this is not an "additive" step
                visualObjs[i].SetActive(true);
                
                if (lookTowardsPivot)
                {
                    Vector3 objectToPivotDir =  pivotTransform.position - visualObjs[i].transform.position;
                    visualObjs[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, objectToPivotDir);
                }
                
                incrementPos += -scaledDir; // increment step
            }
        }

        protected virtual void MakeWave(Vector3 dirFromPivot, float perObjectOffset, bool lookTowardsPivot, float radianOffset, bool isAdditive = false)
        {
            HandleObjects();
            
            float radianStep = (2 * Mathf.PI) / visualObjs.Count;
            float currRadian = radianOffset;
            
            Vector3 dir = (dirFromPivot - pivotTransform.position).normalized;
            Vector3 scaledDir = dir * perObjectOffset; // essentially the step between two objects in direction 'dir'
            float totalDist = perObjectOffset * Mathf.Clamp((visualObjs.Count - 1), 0, Int32.MaxValue);
            Vector3 incrementPos = totalDist * 0.5f * dir; // initialized to starting position
            for (int i = 0; i < visualObjs.Count; ++i)
            {
                Vector3 pos = incrementPos + new Vector3(-incrementPos.y, incrementPos.x, incrementPos.z) * Mathf.Sin(currRadian);

                HandleAdditivePosition(visualObjs[i].transform, pos, isAdditive);
                visualObjs[i].SetActive(true);
                
                if (lookTowardsPivot)
                {
                    Vector3 objectToPivotDir =  pivotTransform.position - visualObjs[i].transform.position;
                    visualObjs[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, objectToPivotDir);
                }
                
                incrementPos += -scaledDir; // increment step
                currRadian += radianStep;
            }
        }

        protected virtual void MakePreset()
        {
            isPresetShape = true;
            
            foreach (Transform t in presetTransforms)
            {
                visualObjs.Add(t.gameObject);
            }
            
            
        }

        protected virtual void AddLocalScale(Vector3 scale, bool isAdditive = false)
        {
            
        }

        // just returns a color between two colors based on "percent".
        protected void ColorGradient(Color color1, Color color2)
        {
            float[] normalizedSpecData = GetNormalizedSpecData();

            for (int i = 0; i < normalizedSpecData.Length; ++i)
            {
                Color c = Color.Lerp(color1, color2, normalizedSpecData[i]);
                colorsToBlendPerObject[i].Add(c);
            }
        }

        // must be called to actually apply colors to the objects
        protected void HandleAllColors()
        {
            List<Color> colors = Blend(colorsToBlendPerObject);
            for (int i = 0; i < visualObjs.Count; ++i)
            {
                SpriteRenderer sr = visualObjs[i].GetComponent<SpriteRenderer>();
                sr.material.color = colors[i];
            }

            InitializeColorsList(true);
        }
        
        private List<Color> Blend(List<List<Color>> colors)
        {
            if (colors.Count != visualObjs.Count)
            {
                ConsoleLogger.Log("Mismatch between visualObjs and colors size!", false, true);
                return null;
            }
            
            List<Color> completeColors = new List<Color>();
            foreach (List<Color> c in colors)
            {
                float r = 0;
                float g = 0;
                float b = 0;
                float a = 0;

                for (int i = 0; i < c.Count; ++i)
                {
                    r += c[i].r;
                    g += c[i].g;
                    b += c[i].b;
                    a += c[i].a;
                }

                if (c.Count != 0)
                {
                    r /= c.Count;
                    g /= c.Count;
                    b /= c.Count;
                    a /= c.Count;
                }

                completeColors.Add(new Color(r, g, b, a));
            }
            
            return completeColors;
        }

        /// <summary>
        /// Handle additive step for positions (typically shapes).
        /// This is a simple function, so its more of a way to remember to do this step.
        /// </summary>
        private void HandleAdditivePosition(Transform transform, Vector3 pos, bool isAdditive)
        {
            if (isAdditive)
            {
                transform.position += pos;
            }
            else
            {
                transform.position = pos;
            }
        }

        /// <summary>
        /// Handle additive step for float values.
        /// This is a simple function, so its more of a way to remember to do this step.
        /// </summary>
        private void HandleAdditiveFloat(ref float val, float delta, bool isAdditive)
        {
            if (isAdditive)
            {
                val += delta;
            }
            else
            {
                val = delta;
            }
        }
        
        private void HandleAverageFloat(ref float val, float[] newVals)
        {
            float avg = val;
            foreach (float f in newVals)
            {
                avg += f;
            }
            avg /= newVals.Length + 1;
            val = avg;
        }
    
        /* idea for preset transforms:
            function takes in parent object, then maps each child transform to a list and then can do things to those objects
     */

        // TODO: profile this!
        private void UpdateSpectrumData()
        {
            TargetRenderSize = Mathf.Clamp(TargetRenderSize, 0, 8192);
            
            // keep sampleSize as a power of 2 and between 64 and 8192 (requirement of GetSpectrumData)
            sampleSize = (int)Mathf.Clamp(Mathf.Pow(2, Mathf.Ceil(Mathf.Log(TargetRenderSize) / Mathf.Log(2))), 64, 8192);
            
            specDataCutoff = Mathf.Clamp01(specDataCutoff);
            int usableSize = (int)(sampleSize * (1 - specDataCutoff));

            if (TargetRenderSize > usableSize)
            {
                sampleSize *= 2; // next power of 2 up

                if (sampleSize > 8192) // if we are already at max sampleSize (8192)
                {
                    // brute force attempt to bring down the cutoff percent until usable size is within target size
                    while (TargetRenderSize > usableSize)
                    {
                        specDataCutoff -= 0.01f; // bring down cutoff by 1% each step (arbitrary number)
                        specDataCutoff = Mathf.Clamp01(specDataCutoff);
                        usableSize = (int)(sampleSize * (1 - specDataCutoff));
                    }
                }
            }
            
            // init arrays
            spectrumData = new float[sampleSize];
            usableSpectrumData = new float[usableSize];
            // built in Unity call to get 'spectrum data'
            audioSource.GetSpectrumData(spectrumData, 0, fftWindowToUse);
            // copy spec data
            for (int i = 0; i < usableSize; ++i)
            {
                usableSpectrumData[i] = spectrumData[i];
            }

            AverageUsableSpectrum();
        }

        // takes usable spectrum and brings it down to TargetSize size, averaging values between
        private void AverageUsableSpectrum()
        {
            int usableSize = usableSpectrumData.Length;
            float[] temp = new float[usableSize];
            for (int i = 0; i < usableSize; ++i)
            {
                temp[i] = usableSpectrumData[i];
            }

            usableSpectrumData = new float[TargetRenderSize];

            // solve average value for each new index of usableSpectrumData
            for (int i = 0; i < TargetRenderSize; ++i)
            {
                // traverse each subset of values in temp array to be averaged out
                float avg = 0;
                int cnt = 0;
                int start = i * TargetRenderSize;
                for (int j = start; j < (start + TargetRenderSize) && j < usableSize; ++j)
                {
                    cnt++;
                    avg += temp[j];
                }
                avg /= cnt;
                usableSpectrumData[i] = avg;
            }

            InitializeColorsList();
        }

        private void InitializeColorsList(bool reset = false)
        {
            if (!reset && colorsToBlendPerObject.Count == usableSpectrumData.Length)
                return;
            
            colorsToBlendPerObject.Clear();
            for (int i = 0; i < usableSpectrumData.Length; ++i)
            {
                
                colorsToBlendPerObject.Add(new List<Color>());
            }
        }

        private void HandleObjects()
        {
            if (objectPooler is null)
            {
                objectPooler = Utilities.ObjectPooler.ConstructObjectPool
                (
                    this.gameObject, 
                    visualObjectPrefab,
                    TargetRenderSize,
                    ResizeRate,
                    ResizeThreshold
                );
                
                TargetRenderSize = TargetRenderSize; // invokes the set (and get) expressions to properly set objectPooler.TargetSize
            }
            
            int objectsDiff = visualObjs.Count - TargetRenderSize;
            if (objectsDiff < 0)
            {
                objectPooler.BorrowObjects(visualObjs, Mathf.Abs(objectsDiff));
            }
            else if (objectsDiff > 0)
            {
                objectPooler.ReturnObjects(visualObjs, Mathf.Abs(objectsDiff));
            }
            foreach (GameObject go in visualObjs)
            {
                go.transform.parent = pivotTransform;
            }
        }

        private float[] GetNormalizedSpecData()
        {
            float min = 0;
            float max = -1;
            foreach (float f in usableSpectrumData)
            {
                if (f > max)
                    max = f;
            }
            
            float mag = max - min;
            float[] normalizedSpecData = new float[usableSpectrumData.Length];
            for (int i = 0; i < usableSpectrumData.Length; ++i)
            {
                normalizedSpecData[i] = usableSpectrumData[i] / mag;
            }
            return normalizedSpecData;
        }

        // sets property values that are in implemented interfaces in the scriptable object ref
        // would be nice to be able to "enumerate" through these values and assign automatically like that...
        private void SetPropertyValues()
        {
            TargetRenderSize = audioVisualPropertiesRef.TargetRenderSize;
            // skip target size since that is handled in TargetRenderSize
            ResizeThreshold = audioVisualPropertiesRef.ResizeThreshold;
            ResizeRate = audioVisualPropertiesRef.ResizeRate;
            ObjectToPool = audioVisualPropertiesRef.ObjectToPool;
        }

        private void RemoveObjectPool()
        {
            if (objectPooler != null)
            {
                objectPooler = objectPooler.ReleasePool();
            }
        }
    }
}