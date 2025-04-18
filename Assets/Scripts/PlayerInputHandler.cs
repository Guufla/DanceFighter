using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;
    private PlayerDefense playerDefense;
    private Pause playerPause;

    // Start is called before the first frame update
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        var movements = FindObjectsOfType<PlayerMovement>();
        var attacks = FindObjectsOfType<PlayerAttack>();
        var defense = FindObjectsOfType<PlayerDefense>();
        var index = playerInput.playerIndex;
        var pause = FindObjectsOfType<Pause>();
        playerMovement = movements.FirstOrDefault(m => m.GetPlayerIndex() == index);
        playerAttack = attacks.FirstOrDefault(m => m.GetPlayerIndex() == index);
        playerDefense = defense.FirstOrDefault(m => m.GetPlayerIndex() == index);
        playerPause = pause.FirstOrDefault(m => m.GetPlayerIndex() == index);



    }
    public void OnMove(CallbackContext context){
        playerMovement.OnMovement(context);
    }
    public void OnPause(CallbackContext context)
    {
        playerPause.OnPause(context);

    }
    public void OnJump(CallbackContext context){
        playerMovement.Jump(context);
    }
    public void OnDash(CallbackContext context){
        playerMovement.Dash(context);
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

    public void OnParry(CallbackContext context)
        {
            playerDefense.OnParryPerformed(context);
        }
    
}
