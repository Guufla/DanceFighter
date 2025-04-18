using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[Serializable]
public class PlayerEffect
{
    public Material Material;
    public Transform EnemyTransform;
    public float EffectDecayRate;

    [HideInInspector] public bool UpdateInputVector;
    public Coroutine shaderEffectCoroutine;
}

public class EffectManager : PersistentSingleton<EffectManager>
{
    public PlayerEffect p1;
    public PlayerEffect p2;
    
    [Space(20)]
    [SerializeField] private Shader targetShader;
    [SerializeField] private CinemachineVirtualCamera vcam;
    
    [SerializeField] private float shakeAmplitude;
    [SerializeField] private float shakeFrequency;
    [SerializeField] private float shakeFalloffSpeed;
    [SerializeField] private float shakeRampupSpeed;

    private CinemachineBasicMultiChannelPerlin noise;
    private int inputVectorID;
    private int inputScale01ID;
    private Queue<float> deltaShakeQueue = new Queue<float>();
    private float ampToFreqRatio;

    private void Start()
    {
        if (p1.Material.shader.name != targetShader.name ||
            p2.Material.shader.name != targetShader.name)
        {
            ConsoleLogger.Log("Player material's shader(s) not correct.", false, true);
            Destroy(this);
            return;
        }

        inputVectorID = Shader.PropertyToID("_input_vector");
        inputScale01ID = Shader.PropertyToID("_do_effect");

        Init();
    }

    private void Update()
    {
        // update input vector
        if (p1.UpdateInputVector || p2.UpdateInputVector)
        {
            p1.Material.SetVector(inputVectorID,
                new Vector4(p1.EnemyTransform.position.x, p1.EnemyTransform.position.y, p1.EnemyTransform.position.z,
                    0));
            p2.Material.SetVector(inputVectorID,
                new Vector4(p2.EnemyTransform.position.x, p2.EnemyTransform.position.y, p2.EnemyTransform.position.z,
                    0));
        }

        ampToFreqRatio = shakeAmplitude / shakeFrequency;
        ShakeHandler();
    }

    public void OnPlayerHit(PlayerEffect player)
    {
        if (player.shaderEffectCoroutine != null)
            StopCoroutine(player.shaderEffectCoroutine);

        player.shaderEffectCoroutine = StartCoroutine(ShaderEffectRoutine(player, () =>
        {
            player.shaderEffectCoroutine = null;
            player.UpdateInputVector = false;
        }));

        StartCoroutine(EnqueueShakeDeltas(shakeFrequency / 2));
    }

    private void ShakeHandler()
    {
        if (deltaShakeQueue.Count > 0)
        {
            float delta = deltaShakeQueue.Dequeue();
            
            if (noise.m_AmplitudeGain < shakeAmplitude)
                noise.m_AmplitudeGain += delta * ampToFreqRatio;
            
            if (noise.m_FrequencyGain < shakeFrequency)
                noise.m_FrequencyGain += delta;
        }
        
        if (noise.m_AmplitudeGain > 0)
        {
            //Debug.Log("1: " + Time.deltaTime * shakeFalloffSpeed);
            noise.m_AmplitudeGain -= Time.deltaTime * ampToFreqRatio * shakeFalloffSpeed;
        }
        else
        {
            noise.m_AmplitudeGain = 0;
        }
        
        if (noise.m_FrequencyGain > 0)
        {
            //Debug.Log("2: " + Time.deltaTime * shakeFalloffSpeed * ampToFreqRatio);
            noise.m_FrequencyGain -= Time.deltaTime * shakeFalloffSpeed;
        }
        else
        {
            noise.m_FrequencyGain = 0;
        }
        //Debug.Log("amp = " + noise.m_AmplitudeGain + ", freq = " + noise.m_FrequencyGain);
    }

    private IEnumerator EnqueueShakeDeltas(float total)
    {
        float elapsedVal = 0;
        while (elapsedVal < total)
        {
            float delta = Mathf.Abs(Time.deltaTime * shakeRampupSpeed);
            elapsedVal += delta;
            deltaShakeQueue.Enqueue(delta);
            yield return null;
        }
    }


    private void Init()
    {
        p1.Material.SetFloat(inputScale01ID, 0);
        p2.Material.SetFloat(inputScale01ID, 0);
        noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private IEnumerator ShaderEffectRoutine(PlayerEffect player, Action onFinish)
    {
        p1.UpdateInputVector = true;
        p2.UpdateInputVector = true;
        
        player.UpdateInputVector = true;

        float scale01 = 1;
        while (scale01 > 0)
        {
            player.Material.SetFloat(inputScale01ID, scale01);
            scale01 -= Time.deltaTime * player.EffectDecayRate;
            yield return null;
        }
        player.Material.SetFloat(inputScale01ID, 0);

        onFinish();
    }
}