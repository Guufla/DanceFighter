using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    [SerializeField] int playerindex;
    public bool isPaused = false;
    public Button restartGamePause;
    public Button mainMenu;
    // Start is called before the first frame update
    void Start()
    {
        restartGamePause.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(false);
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
            restartGamePause.gameObject.SetActive(true);
            mainMenu.gameObject.SetActive(true);

        }
        else
        {
            Time.timeScale = 1;
        }
        
    }
}
