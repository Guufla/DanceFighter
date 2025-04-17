using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class Pause : MonoBehaviour
{
    [SerializeField] int playerindex;
    public bool isPaused = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public int GetPlayerIndex()
    {
        return playerindex;
    }
    // Update is called once per frame
    void Update()
    {
        
        
    }
    public void OnPause(CallbackContext context)
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0;
            
        }
        else
        {
            Time.timeScale = 1;
        }
        
    }
}
