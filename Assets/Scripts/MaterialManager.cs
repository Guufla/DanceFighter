using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    public bool toggleShaderEffect = false;
    
    [SerializeField] private Shader targetShader;
    [SerializeField] private Material player1Material;
    [SerializeField] private Material player2Material;
    [SerializeField] private Transform player1Transform;
    [SerializeField] private Transform player2Transform;
    [SerializeField] private float effectRate;

    private int inputVectorID;
    private int inputScale01ID;
    
    private Coroutine shaderEffectCoroutine_p1 = null;
    private Coroutine shaderEffectCoroutine_p2 = null;

    private void Start()
    {
        if (player1Material.shader.name != targetShader.name ||
            player2Material.shader.name != targetShader.name)
        {
            ConsoleLogger.Log("Player material's shader(s) not correct.", false, true);
            Destroy(this);
            return;
        }
        
        inputVectorID = Shader.PropertyToID("_input_vector");
        inputScale01ID = Shader.PropertyToID("_input_scale01");

        Init();
    }

    private void Update()
    {
        if (toggleShaderEffect)
        {
            toggleShaderEffect = false;
            OnPlayerHit(true);
            OnPlayerHit(false);
        }
        
        player1Material.SetVector(inputVectorID, new Vector4(player2Transform.position.x, player2Transform.position.y, player2Transform.position.z, 0));
        player2Material.SetVector(inputVectorID, new Vector4(player1Transform.position.x, player1Transform.position.y, player1Transform.position.z, 0));
    }
    
    public void OnPlayerHit(bool p1WasHit)
    {
        if (p1WasHit)
        {
            if (shaderEffectCoroutine_p1 != null)
                StopCoroutine(shaderEffectCoroutine_p1);
            
            shaderEffectCoroutine_p1 = StartCoroutine(ShaderEffectRoutine(player1Material, player2Transform, () =>
            {
                shaderEffectCoroutine_p1 = null;
            }));
        }
        else
        {
            if (shaderEffectCoroutine_p2 != null)
                StopCoroutine(shaderEffectCoroutine_p2);
            
            shaderEffectCoroutine_p2 = StartCoroutine(ShaderEffectRoutine(player2Material, player1Transform, () =>
            {
                shaderEffectCoroutine_p2 = null;
            }));
        }
    }

    private void Init()
    {
        
    }

    private IEnumerator ShaderEffectRoutine(Material mat, Transform t, Action onFinish)
    {
        float scale01 = 1;
        while (scale01 > 0)
        {
            mat.SetFloat(inputScale01ID, scale01);
            scale01 -= Time.deltaTime * effectRate;
            yield return null;
        }
        mat.SetFloat(inputScale01ID, scale01);

        onFinish();
    }
}