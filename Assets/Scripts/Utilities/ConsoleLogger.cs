using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Essentially its Debug.Log, but it's easy to turn on and off any logs we want to
/// </summary>
public class ConsoleLogger : PersistentStaticInstance<ConsoleLogger>
{
    [SerializeField] private bool logToConsole = true;
    [SerializeField] private bool logWarnings = true;
    [SerializeField] private bool logErrors = true;

    // unordered unique set of objects that have called ConsoleLogger.Log in its lifetime
    // eventually use this to customize which classes you want to hear logs from (might need custom interactable UI for this)
    //private static HashSet<object> objectsUsingLogger = new HashSet<object>();
    
    public static void Log(/*object sender,*/ string message, bool isWarning = false, bool isError = false)
    {
        if (Instance == null)
        {
            Debug.LogWarning("No console logger found!");
            if (isWarning)
                Debug.LogWarning(message);
            else if (isError)
                Debug.LogError(message);
            else
                Debug.Log(message);
            return;
        }
        
        //objectsUsingLogger.Add(sender);
        
        if (!Instance.logToConsole)
        {
            return;
        }
        if (isWarning && Instance.logWarnings)
        {
            Debug.LogWarning(message);
        }
        else if (isError && Instance.logErrors)
        {
            Debug.LogError(message);
        }
        else
        {
            Debug.Log(message);
        }
    }
}