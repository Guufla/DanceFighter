using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;
    private PlayerDefense playerDefense;

    // Start is called before the first frame update
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        var movements = FindObjectsOfType<PlayerMovement>();
        var attacks = FindObjectsOfType<PlayerAttack>();
        var defense = FindObjectsOfType<PlayerDefense>();
        var index = playerInput.playerIndex;
        playerMovement = movements.FirstOrDefault(m => m.GetPlayerIndex() == index);
        playerAttack = attacks.FirstOrDefault(m => m.GetPlayerIndex() == index);
        playerDefense = defense.FirstOrDefault(m => m.GetPlayerIndex() == index);


    }
    public void OnMove(CallbackContext context){
        playerMovement.OnMovement(context);
    }
    public void OnJump(CallbackContext context){
        playerMovement.Jump(context);
    }
    public void OnAttackE(CallbackContext context){
        playerAttack.AttackE(context);
    }
    public void OnAttackR(CallbackContext context){
        playerAttack.AttackR(context);
    }
    public void OnAttackF(CallbackContext context){
        playerAttack.AttackF(context);
    }
    public void OnUpPressed(CallbackContext context){
        playerAttack.UpPressed(context);
    }
    public void OnDownPressed(CallbackContext context){
        playerAttack.DownPressed(context);
    }
    
    public void OnBlock(CallbackContext context){
        if(context.started){
            playerDefense.OnBlockStarted(context);
        }
        else if(context.performed)
        {
            playerDefense.OnBlockPerformed(context);
        }
        else if (context.canceled)
        {
            playerDefense.OnBlockCanceled(context);
        }
    }

    public void OnParryPerformed(CallbackContext context)
        {
            playerDefense.OnParryPerformed(context);
        }
    
}
