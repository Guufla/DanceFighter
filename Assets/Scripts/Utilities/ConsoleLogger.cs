using UnityEngine;

// call Logger.Log("message");
public class ConsoleLogger : PersistentSingleton<ConsoleLogger>
{
    protected void Awake()
    {
        base.Awake();
    }
    
    [SerializeField] private bool logToConsole = true;
    [SerializeField] private bool logWarnings = true;
    [SerializeField] private bool logErrors = true;

    public static void Log(string message, bool isWarning = false, bool isError = false)
    {
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