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
         *      - Wave
         *
         * Effects:
         *      - ScaleSingleAxis
         *      - ScaleMultiAxes
         *      - ColorGradient
         *      - Transparency
         *
         * Code Changes:
         *      - Make every shape and effect function be able to mix with each other
         */

        [SerializeField] private bool isPresetShape;
        [SerializeField] protected GameObject visualObjectPrefab;
        [SerializeField] protected Transform[] presetTransforms;
        [SerializeField] protected Transform pivotTransform;
        [SerializeField] protected int targetAmount;

        protected List<GameObject> visualObjs = new List<GameObject>();
        
        private Utilities.ObjectPooler objectPooler = null;
        
        private void OnDestroy()
        {
            RemoveObjectPool();
        }
    
        /// <summary>
        /// Method <c>MakeCircle</c> locates each gameobject in visual objects to be in a circle shape
        /// around pivotTransform.
        /// </summary>
        protected virtual void MakeCircle(float offsetFromPivot, bool lookTowardsPivot) // TODO: Add actions that you can pass in to be able to run in the loop per object!
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

        private void HandleObjects()
        {
            if (isPresetShape)
            {
                ConsoleLogger.Log("HandleObjectPool was called on visual with isPresetShape == true", true);
                return;
            }
            
            if (objectPooler == null)
            {
                objectPooler = Utilities.ObjectPooler.ConstructObjectPool
                (
                    this.gameObject, 
                    visualObjectPrefab,
                    targetAmount,
                    Utilities.ObjectPooler.DefualtResizeRate,
                    Utilities.ObjectPooler.DefualtResizeThreshold
                );
            }
            
            objectPooler.TargetSize = targetAmount;

            int objectsDiff = visualObjs.Count - targetAmount;
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
                objectPooler.ReleasePool();
        }
    }
}