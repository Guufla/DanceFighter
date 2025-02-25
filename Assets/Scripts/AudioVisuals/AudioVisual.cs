using System.Collections.Generic;
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
    public abstract class AudioVisual : MonoBehaviour, IAudioVisual
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
        
        public AudioSource audioSource;

        [SerializeField] protected bool isPresetShape;
        [SerializeField] protected GameObject visualObjectPrefab;
        [SerializeField] protected Transform[] presetTransforms;
        [SerializeField] protected Transform pivotTransform;
        [SerializeField] protected float specDataCutoff; // defines what percent of the spec data to cut off from the right

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
        
        [SerializeField] private FFTWindow fftWindowToUse;

        protected List<GameObject> visualObjs = new List<GameObject>();
        protected float[] usableSpectrumData;
        protected Utilities.ObjectPooler objectPooler = null;
        
        private int _targetRenderSize;
        private int sampleSize;
        private float[] spectrumData;
        
        protected virtual void OnDestroy()
        {
            RemoveObjectPool();
        }

        // must call base.Update() if you are overriding this in child class
        protected virtual void Update()
        {
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
            output += $"Spectrum Data Cutoff %: {specDataCutoff * 100}\n";
            output += $"FFTWindow being used: {fftWindowToUse}\n";
            output += $"Is Preset Shape: {isPresetShape}\n";
            return output;
        }

        /// <summary>
        /// Method <c>MakeCircle</c> locates each gameobject in visual objects to be in a circle shape
        /// around pivotTransform.
        /// </summary>
        /// // TODO: Add actions that you can pass in to be able to run in the loop per object!
        protected virtual void MakeCircle(float offsetFromPivot, bool lookTowardsPivot) 
        {
            HandleObjects();
            
            float radianStep = (2 * Mathf.PI) / visualObjs.Count;
            float currRadian = 0;

            for (int i = 0; i < visualObjs.Count; ++i)
            {
                Vector3 circle = new Vector3(Mathf.Cos(currRadian), Mathf.Sin(currRadian), 0);
                Vector3 circleAndOffset = circle * offsetFromPivot;
                Vector3 objPos = pivotTransform.position + circleAndOffset;

                visualObjs[i].transform.position = objPos;

                if (lookTowardsPivot)
                {
                    Vector3 dir = visualObjs[i].transform.position - pivotTransform.position;
                    visualObjs[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, -dir);
                    
                    // cool effect if you rotate the spotlights forward to be facing towards/away from the screen in 2D
                }
            
                currRadian += radianStep;
            }
        }

        protected virtual void MakeLine(float perObjectOffset, float diagonalAngleFromPivot, bool lookTowardsPivot)
        {
            
        }

        protected virtual void MakeWave()
        {
        
        }

        protected virtual void MakePreset()
        {
            foreach (Transform t in presetTransforms)
            {
                visualObjs.Add(t.gameObject);
            }
            
            
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
                    Utilities.ObjectPooler.DefualtResizeRate,
                    Utilities.ObjectPooler.DefualtResizeThreshold
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

        private void RemoveObjectPool()
        {
            if (objectPooler != null)
            {
                objectPooler = objectPooler.ReleasePool();
            }
        }
    }
}