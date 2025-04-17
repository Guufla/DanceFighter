using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aura : MonoBehaviour
{
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        if(anim == null)
        {
            Debug.LogError("Animator component not found");
        }
    }

    void Update()
    {
        if((GameManager.Instance.isOffensiveP1 && gameObject.CompareTag("Player1")) || (GameManager.Instance.isOffensiveP2 && gameObject.CompareTag("Player2")))
        {
            anim.SetBool("Active", true);
        }
        else
        {
            anim.SetBool("Active", false);
        }
    }
}
