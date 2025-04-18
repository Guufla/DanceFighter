using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aura : MonoBehaviour
{
    [SerializeField] private Transform spriteTransform;
    [SerializeField] private float scaleSpeed = 1;

    private void Start()
    {
        spriteTransform.localScale = Vector3.zero;
    }

    void Update()
    {
        if((GameManager.Instance.isOffensiveP1 && gameObject.CompareTag("Player1")) || (GameManager.Instance.isOffensiveP2 && gameObject.CompareTag("Player2")))
        {
            if (spriteTransform.localScale.x < 1)
            {
                spriteTransform.localScale += Vector3.one * scaleSpeed * Time.deltaTime;
            }
        }
        else
        {
            if (spriteTransform.localScale.x > 0)
            {
                spriteTransform.localScale -= Vector3.one * scaleSpeed * Time.deltaTime;
            }
        }
    }
}
