using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AudioManager : MonoBehaviour
{
    //Manager Stuff
    private static AudioManager _instance;

    public static AudioManager Instance
    {
        get
        {
            if(_instance == null)
            {
                Debug.LogError("AudioManager is Null");
            }
            return _instance;
        }
        
    }
    private void Awake()
    {
        if(_instance)
        {
            Debug.LogError("AudioManager is already in the scene");
            Destroy(gameObject);
        }
        else{
            _instance = this;
            //DontDestroyOnLoad(this); 
        }
        
    }

    //Audio Stuff
    [Header("Audio Sources")]
    [SerializeField] AudioSource musicSource;

    [Header("Audio Clips")]
    [SerializeField] AudioClip bothAggroClip;
    [SerializeField] AudioClip oneAggroClip;
    [SerializeField] AudioClip noAggroClip;

    private AudioClip newClip;

    private void Start() 
    {
        musicSource.clip = noAggroClip;
        musicSource.Play();
    }

   
    //to be called in one of the player's scripts when their aggro state changes
    public void StateChange()
    {
        if(GameManager.Instance.isOffensiveP1 && GameManager.Instance.isOffensiveP2)
        {
            //both players aggro music
            newClip = bothAggroClip;
        }
        else if (GameManager.Instance.isOffensiveP1 || GameManager.Instance.isOffensiveP2)
        {
            //one player aggro music
            newClip = oneAggroClip;
        }
        else
        {
            //no player aggro music
            newClip = noAggroClip;
        }

        if(newClip != musicSource.clip)
        {
            musicSource.clip = newClip;
            musicSource.Play();
        }
    }
}

