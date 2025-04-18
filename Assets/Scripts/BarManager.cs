using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BarManager : MonoBehaviour
{
    [SerializeField] private Material left;
    [SerializeField] private Material right;
    [SerializeField] private Material leftFake;
    [SerializeField] private Material rightFake;
    [SerializeField] private Material leftBack;
    [SerializeField] private Material rightBack;
    [SerializeField] private Material leftOffense;
    [SerializeField] private Material rightOffense;

    [SerializeField] private string fillPropertyName;
    [SerializeField] private float healthBarDelayTime;

    private float p1MaxHealth;
    private float p2MaxHealth;
    private float p1MaxOffense;
    private float p2MaxOffense;
    private int fillID;
    private float fillAmountDelayed;
    private float timeElapsed = 0;
    private Queue<float> pastFillsQueueL = new Queue<float>();
    private Queue<float> pastFillsQueueR = new Queue<float>();

    private void Start()
    {
        p1MaxHealth = GameManager.Instance.P1Health;
        p2MaxHealth = GameManager.Instance.P2Health;
        p1MaxOffense = GameManager.Instance.p1OffensiveBar.maxValue;
        p2MaxOffense = GameManager.Instance.p2Offensive.maxValue;
        
        fillID = Shader.PropertyToID(fillPropertyName);
    }

    private void Update()
    {
        if (GameManager.Instance.roundOver)
        {
            ResetDelayedBar(ref pastFillsQueueL);
            ResetDelayedBar(ref pastFillsQueueR);
        }
        
        float leftHealthFill = GameManager.Instance.P1Health / p2MaxHealth;
        float rightHealthFill = GameManager.Instance.P2Health / p1MaxHealth; 
        float leftOffenseFill = GameManager.Instance.p1OffensiveBar.value / p1MaxOffense;
        float rightOffenseFill = GameManager.Instance.p2Offensive.value / p2MaxOffense;
        
        left.SetFloat(fillID, leftHealthFill);
        right.SetFloat(fillID, rightHealthFill);
        leftFake.SetFloat(fillID, rightHealthFill);
        rightFake.SetFloat(fillID, rightHealthFill);
        leftOffense.SetFloat(fillID, leftOffenseFill);
        rightOffense.SetFloat(fillID, rightOffenseFill);
        
        DelayBar(leftBack, healthBarDelayTime, leftHealthFill, ref pastFillsQueueL);
        DelayBar(rightBack, healthBarDelayTime, rightHealthFill, ref pastFillsQueueR);
    }

    private void DelayBar(Material target, float delayInSeconds, float currFill, ref Queue<float> pastFills)
    {
        // call in update
        
        pastFills.Enqueue(currFill);
        if (timeElapsed >= delayInSeconds)
        {
            float d = pastFills.Dequeue();
            target.SetFloat(fillID, d);
        }
        timeElapsed += Time.deltaTime;
    }

    private void ResetDelayedBar(ref Queue<float> pastFills)
    {
        pastFills.Clear();
        timeElapsed = 0;
    }
}
