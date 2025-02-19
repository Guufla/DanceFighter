using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Creation: Calling Utilities.ObjectPooler.ConstructObjectPool() creates new instance of class and returns a reference to monobehavior.
    /// Borrowing: Call BorrowObjects() to get list of objects.
    /// Returning: Call
    /// Deletion: Either destroy the monobehavior with Destroy(), or call ReleasePool() on the reference to this class.
    /// </summary>
    public class ObjectPooler : MonoBehaviour
    {
        // reference to all alive pools
        public static List<ObjectPooler> AllPools { get; private set; }
            
        public Stack<GameObject> Pool {get; private set;}
        public int TargetSize
        {
            get => this._targetSize;
            set
            {
                this._targetSize = Mathf.Clamp(value, 0, int.MaxValue);
                HandleObjectPool();
            } 
        }
        public int ResizeThreshold
        {
            get => this._resizeThreshold;
            set
            {
                this._resizeThreshold = Mathf.Clamp(value, 0, int.MaxValue);
                HandleObjectPool();
            } 
        }
        public float ResizeRate
        {
            get => this._resizeRate;
            set
            {
                this._resizeRate = Mathf.Clamp(value, MIN_RATE, MAX_RATE);
            } 
        }
        public GameObject ObjectToPool
        {
            get => this._objectToPool;
            set
            {
                this._objectToPool = value;
                ConsoleLogger.Log("Object To Pool was set to " + this._objectToPool);
            } 
        }
        
        // arbitrary constants so game doesn't die or try to divide by 0.
        public const float MIN_RATE = 0.01f;
        public const float MAX_RATE = 100f;

        public static float DefualtResizeRate = 2;
        public static int DefualtResizeThreshold = 25;
        
        // internal fields that are used in the properties above
        private GameObject _objectToPool;
        private int _resizeThreshold; // upper and lower threshold of pool where we should start creating/destroying objects
        private int _targetSize;
        private float _resizeRate;
        
        private List<GameObject> allInstances = new List<GameObject>(); // references to all instanced game objects that originated in the pool
        private UnityEngine.Coroutine balancingCoroutine; // routine that references any ongoing balancing process
        private List<GameObject> addToPoolBuffer = new List<GameObject>(); // buffer (idea is to reduce memory overhead) (static array might be better here)
        private List<GameObject> removeFromPoolBuffer = new List<GameObject>(); // buffer (idea is to reduce memory overhead) (static array might be better here)
        private bool properlyDestroyed = false; // to help handle deletion of pool
        private bool pastInitStage = false;

        private void OnDestroy()
        {
            if (properlyDestroyed) 
                return;
            ReleasePool();
        }

        public static ObjectPooler ConstructObjectPool(GameObject componentHostingObject, GameObject obj, int size, float resizeRate, int resizeThreshold)
        {
            ConsoleLogger.Log("Constructing ObjectPooler");
            ObjectPooler pooler = componentHostingObject.AddComponent<ObjectPooler>();
            
            if (AllPools == null)
                AllPools = new List<ObjectPooler>();
            AllPools.Add(pooler);
            
            pooler.balancingCoroutine = null;
            pooler.ObjectToPool = obj;
            pooler.TargetSize = size;
            pooler.ResizeThreshold = resizeThreshold;
            pooler.ResizeRate = resizeRate;
            pooler.Pool = new Stack<GameObject>();
            pooler.NewObjectsToPool(pooler.TargetSize);
            
            pooler.pastInitStage = true;
            pooler.HandleObjectPool();
            
            return pooler;
        }

        /// <summary>
        /// Effectively "deletes" object pool and cannot be used.
        /// </summary>
        public void ReleasePool()
        {
            foreach (GameObject obj in allInstances)
            {
                Destroy(obj);
            }

            AllPools.Remove(this);
            
            properlyDestroyed = true;
            Destroy(this);
        }

        /// <summary>
        /// Borrows GameObjects from the pool.
        /// </summary>
        public void BorrowObjects(List<GameObject> objects, int amountToTake)
        {
            int objsBorrowedCounter = 0;

            // take objects off stack
            PopFromPool(amountToTake, ref objsBorrowedCounter, objects);

            // if stack is empty
            if (objsBorrowedCounter < amountToTake)
            {
                // give stack just enough, then take those objects off the stack
                NewObjectsToPool(amountToTake - objsBorrowedCounter);
                PopFromPool(amountToTake, ref objsBorrowedCounter, objects);
            }

            HandleObjectPool();
        }
        
        /// <summary>
        /// Return GameObjects to the pool, nulling-out the array you pass in.
        /// </summary>
        public void ReturnObjects(List<GameObject> objects, int amountToReturn)
        {
            List<GameObject> objectsToReturn = new List<GameObject>();

            for (int i = 0; i < amountToReturn; i++)
            {
                objectsToReturn.Add(objects[objects.Count - 1]);
                objects.RemoveAt(objects.Count - 1);
            }
            
            foreach (GameObject obj in objectsToReturn)
            {
                if (obj == null)
                {
                    ConsoleLogger.Log("Attempt to return null object", false, true);
                    return;
                }
                if (!allInstances.Contains(obj))
                {
                    ConsoleLogger.Log("Error when trying to return object " + obj + ". Object is not in all instances reference!", false, true);
                    return;
                }

                DefualtObjectBehavior(obj);
            }
            PushToPool(objectsToReturn);
            HandleObjectPool();
        }
        
        
        
        // NOTE: Handle the pool every time the pool changes
        private void HandleObjectPool()
        {
            if (!pastInitStage)
                return;
            
            // handle starting/stopping new and old coroutines.
            // only want one coroutine running at a time.
            if (balancingCoroutine != null)
            {
                StopCoroutine(balancingCoroutine);
            }
            balancingCoroutine = StartCoroutine(BalancePoolSize());
        }

        private IEnumerator BalancePoolSize()
        {
            bool increase = false;

            while ((increase = Pool.Count < (this.TargetSize - this.ResizeThreshold)) || Pool.Count > (this.TargetSize + this.ResizeThreshold))
            {
                if (increase)
                {
                    NewObjectsToPool(1);
                }
                else
                {
                    DestroyObjects(1);
                }
                
                yield return new WaitForSeconds(1 / this.ResizeRate);
            }
            
            balancingCoroutine = null; // set to null to "reset"
        }
        


        private void NewObjectsToPool(int amount)
        {
            if (addToPoolBuffer.Count > 0)
            {
                ConsoleLogger.Log("Something went wrong, buffer should be empty...", false, true);
                return;
            }
            
            for (int i = 0; i < amount; ++i)
            {
                addToPoolBuffer.Add(InstanceObject());
            }
            
            PushToPool(addToPoolBuffer);
        }
        
        private void DestroyObjects(int amount)
        {
            if (removeFromPoolBuffer.Count > 0)
            {
                ConsoleLogger.Log("Something went wrong, buffer should be empty...", false, true);
                return;
            }
            
            int counter = 0;
            
            PopFromPool(amount, ref counter, removeFromPoolBuffer);

            foreach (GameObject obj in removeFromPoolBuffer)
            {
                allInstances.Remove(obj);
                Destroy(obj);
            }
            removeFromPoolBuffer.Clear();
        }

        private GameObject InstanceObject()
        {
            GameObject instance = GameObject.Instantiate(ObjectToPool);
            DefualtObjectBehavior(instance);
            
            allInstances.Add(instance);
                
            return instance;
        }

        // defines object behavior (should be called after instancing object)
        // TODO: public function that takes in a delegate and runs it on instancing objects
        private void DefualtObjectBehavior(GameObject obj)
        {
            obj.SetActive(false);
        }
        
        private void PushToPool(List<GameObject> objectsToAdd)
        {
            ConsoleLogger.Log("<color=green>Pushing</color> " + objectsToAdd.Count + " objects to pool...");
            
            foreach (GameObject obj in objectsToAdd)
            {
                Pool.Push(obj);
            }
            objectsToAdd.Clear();
        }

        private void PopFromPool(int amount, ref int counter, List<GameObject> objectsToRemove)
        {
            ConsoleLogger.Log("<color=red>Popping</color> " + (amount - counter) + " objects to pool...");
            
            while (counter < amount && Pool.Count > 0)
            {
                objectsToRemove.Add(Pool.Pop());
                counter++;
            }
        }
    }
}