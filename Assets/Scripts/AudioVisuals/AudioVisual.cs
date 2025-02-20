using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace AudioVisuals
{
    /// <summary>
    /// Base Audio Visualization class to be inherited and used by other classes. Idea is you can customize, stack, and do whatever you want
    /// to this from within the engine and inspector to create cool effects.
    /// </summary>
    public abstract class AudioVisual : MonoBehaviour
    {
        /*
         * Shapes:
         *      - Circle
         *      - Line
         *      - StaticWave
         *      - OscillatingWave
         *
         * Effects:
         *      - ScaleSingleAxis
         *      - ScaleMultiAxes
         *      - ColorGradient
         *      - Transparency
         *
         * Code Changes:
         *      - Make every shape and effect function be able to mix with each other
         *
         * Optimizations:
         *      Swapped "objectPooler != null" checks to "objectPooler is not null" checks:
         *          https://stackoverflow.com/questions/75013054/is-it-cost-expensive-to-use-if-gameobject-null
         */
        
        public AudioSource audioSource;

        [SerializeField] protected bool isPresetShape;
        [SerializeField] protected GameObject visualObjectPrefab;
        [SerializeField] protected Transform[] presetTransforms;
        [SerializeField] protected Transform pivotTransform;

        // both the rendering size and object pool target size if it exists
        public int TargetRenderSize
        {
            get => _targetRenderSize;
            set
            {
                if (objectPooler is not null)
                    objectPooler.TargetSize = value;
                _targetRenderSize = value;
                
                HandleSpectrumDataResizing();
            }
        }
        
        [SerializeField] private FFTWindow fftWindowToUse;
        [SerializeField] private bool compressSpecDataToRenderSize = false; // compress the larger spec data array into a smaller one with averages
        [SerializeField] private float specDataCutoff = 0.5f; // defines what percent of the spec data to cut off starting from the right

        protected List<GameObject> visualObjs = new List<GameObject>();
        protected float[] usableSpectrumData;
        
        private Utilities.ObjectPooler objectPooler = null;
        private int _targetRenderSize;
        private int sampleSize;
        private float[] spectrumData;
        
        private void OnDestroy()
        {
            RemoveObjectPool();
        }

        private void Update()
        {
            UpdateSpectrumData();
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

        protected virtual void MakeLine()
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
        
        private void UpdateSpectrumData()                   // TODO: Find way to cut down spec data from the right side, then be able to scale that new array down by averaging it out across a smaller array
        {
            audioSource.GetSpectrumData(spectrumData, 0, fftWindowToUse);
            
            int usableSize = (int)(sampleSize * (1 - specDataCutoff));
            usableSpectrumData = new float[usableSize];
            
            if (compressSpecDataToRenderSize)
            {
                float[] temp = new float[TargetRenderSize];
                
            }
        }
        
        // called only in TargetRenderSize set expression
        private void HandleSpectrumDataResizing()
        {
            // keep sampleSize as a power of 2 and between 64 and 8192 (requirement of GetSpectrumData)
            sampleSize = (int)Mathf.Clamp(Mathf.Pow(2, Mathf.Ceil(Mathf.Log(TargetRenderSize) / Mathf.Log(2))), 64, 8192);
            
            // skip setting property TargetRenderSize since it's set expression is calling this function!
            _targetRenderSize = Mathf.Clamp(TargetRenderSize, 0, sampleSize);
            
            spectrumData = new float[sampleSize];
        }
        
        private void HandleObjects()
        {
            if (isPresetShape)
            {
                ConsoleLogger.Log("HandleObjectPool was called on visual with isPresetShape == true", true);
                return;
            }
            
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