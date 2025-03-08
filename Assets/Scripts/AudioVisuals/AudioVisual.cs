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
         * Shape Ideas:
         *      - Random Noise Movement
         *
         * Effects:
         *      
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
        [Range(64, 8192)] [SerializeField]
        private int sampleSize;
        
        protected List<GameObject> visualObjs = new List<GameObject>();
        protected float[] usableSpecData; // the spec data that will actually be used in the visuals

        private Utilities.ObjectPooler objectPooler = null;
        private GameObject visualObjectPrefab;
        private int _targetRenderSize;
        private float[] specData;
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
            output += $"Spectrum Data Length: {specData.Length}\n";
            output += $"Usable Spectrum Data Length: {usableSpecData.Length}\n";
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
        
        

        protected virtual void LocalScale(Vector3 scaledAxes, bool isNormalized, bool isAdditive = false)
        {
            float[] tempData;
            if (isNormalized)
            {
                tempData = GetNormalizedSpecData();
            }
            else
            {
                tempData = usableSpecData;
            }
            
            for (int i = 0; i < visualObjs.Count; ++i)
            {
                Vector3 scale = tempData[i] * scaledAxes;
                HandleAdditiveScale(visualObjs[i].transform, scale, isAdditive);
            }
        }

        protected virtual void LineUpObjects(Vector3 dirFromPivot, bool isAdditive = false)
        {
            Transform reference = visualObjs[0].transform;
            float offset = reference.localScale.x;
            MakeLine(dirFromPivot, offset, false, isAdditive);
        }

        // just returns a color between two colors based on "percent".
        protected void ColorGradient(Color color0, Color color1)
        {
            float[] normalizedSpecData = GetNormalizedSpecData();

            for (int i = 0; i < normalizedSpecData.Length; ++i)
            {
                Color c = Color.Lerp(color0, color1, normalizedSpecData[i]);
                ConsoleLogger.Log($"({c.r} , {c.g}, {c.b}, {c.a})");
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
        
        #region Effect Combination Handlers
        
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

        private void HandleAdditiveScale(Transform transform, Vector3 scale, bool isAdditive)
        {
            if (isAdditive)
            {
                transform.localScale += scale;
            }
            else
            {
                transform.localScale = scale;
            }
        }
        
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
        
        
        #endregion
    
        /* idea for preset transforms:
            function takes in parent object, then maps each child transform to a list and then can do things to those objects
     */

        // TODO: profile this!
        private void UpdateSpectrumData()
        {
            TargetRenderSize = Mathf.Clamp(TargetRenderSize, 0, 8192);
            // keep sampleSize as a power of 2 and between 64 and 8192 (requirement of GetSpectrumData)
            sampleSize = (int)Mathf.Clamp(Mathf.Pow(2, Mathf.Ceil(Mathf.Log(sampleSize) / Mathf.Log(2))), 64, 8192);
            specDataCutoff = Mathf.Clamp01(specDataCutoff);
            
            
            int usableSize = TargetRenderSize; // should be the visual object count or TargetRenderSize (same)
            int subsetStep = 0;
            int remainder = 0;
            
            SetSampleSubsetStep(ref subsetStep, ref remainder, usableSize);
            ConsoleLogger.Log($"subsetStep = {subsetStep}, remainder = {remainder}");
            
            // init arrays
            specData = new float[sampleSize];
            usableSpecData = new float[usableSize];
            // built in Unity call to get 'spectrum data'
            audioSource.GetSpectrumData(specData, 0, fftWindowToUse);
            
            // set values of usableSpecData
            // int len = sampleSize;
            // int c = 0;
            // if (remainder != 0)
            //     len -= remainder;
            // for (int i = 0; i < len; i += subsetStep)
            // {
            //     float thisStepAvg = 0;
            //     for (int j = i; j < i + subsetStep; ++j)
            //     {
            //         thisStepAvg += specData[j];
            //     }
            //     thisStepAvg /= subsetStep;
            //     usableSpecData[c] = thisStepAvg;
            //     c++;
            //     
            //     //ConsoleLogger.Log($"subsetStep = {subsetStep}, thisStepAvg = {thisStepAvg}");
            // }
            //
            // if (remainder != 0)
            // {
            //     float thisStepAvg = 0;
            //     for (int i = len - 1; i < sampleSize; ++i)
            //     {
            //         thisStepAvg += specData[i];
            //     }
            //     thisStepAvg /= remainder;
            //     usableSpecData[c] = thisStepAvg;
            //     
            //     //ConsoleLogger.Log($"remainderStep = {remainder}, thisStepAvg = {thisStepAvg}");
            // }
            
            // temporary
            for (int i = 0; i < usableSpecData.Length; ++i)
            {
                usableSpecData[i] = specData[i];
            }
            
            
            InitializeColorsList();
        }

        private void SetSampleSubsetStep(ref int subsetStep, ref int remainderStep, int usableSize)
        {
            int sampleSizeWithCutoff = (int)(sampleSize * (1 - specDataCutoff));
            subsetStep = sampleSizeWithCutoff / usableSize;

            if (usableSize > sampleSizeWithCutoff)
            {
                sampleSize *= 2; // next power of 2 up

                if (sampleSize > 8192) // if we are already at max sampleSize (8192), we need to forcibly change the cutoff
                {
                    // brute force attempt to bring down the cutoff percent until usable size is within target size
                    while (TargetRenderSize > usableSize)
                    {
                        specDataCutoff -= 0.01f; // bring down cutoff by 1% each step (arbitrary number)
                        specDataCutoff = Mathf.Clamp01(specDataCutoff);
                        usableSize = (int)(sampleSize * (1 - specDataCutoff));
                    }
                }

                SetSampleSubsetStep(ref subsetStep, ref remainderStep, usableSize);
                return; // IMPORTANT so we don't do the last line below
            }

            remainderStep = sampleSizeWithCutoff - (subsetStep * usableSize);
        }

        private void InitializeColorsList(bool reset = false)
        {
            if (!reset && colorsToBlendPerObject.Count == usableSpecData.Length)
                return;
            
            colorsToBlendPerObject.Clear();
            for (int i = 0; i < usableSpecData.Length; ++i)
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
            for (int i = 0; i < usableSpecData.Length; ++i)
            {
                //ConsoleLogger.Log($"spectrumData[{i}] == {specData[i]}");
                //ConsoleLogger.Log($"usableSpectrumData[{i}] == {usableSpecData[i]}");
                if (usableSpecData[i] > max)
                    max = usableSpecData[i];
            }
            
            float[] normalizedSpecData = new float[usableSpecData.Length];
            for (int i = 0; i < usableSpecData.Length; ++i)
            {
                float vali = (usableSpecData[i] - min) / (max - min);

                normalizedSpecData[i] = vali;
                ConsoleLogger.Log($"normalizedSpecData[{i}] == {normalizedSpecData[i]}"); // test normalization
            }
            normalizedSpecData[usableSpecData.Length - 1] = usableSpecData[usableSpecData.Length - 1];// bandaid fix
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