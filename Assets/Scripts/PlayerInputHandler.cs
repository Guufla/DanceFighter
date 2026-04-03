using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class PlayerInputHandler : MonoBehaviour
{
    public delegate void AttackInputEvent(AttackType attackType);
    public event AttackInputEvent OnAttackInput;
	public int playerIndex;

    private ComboCheck comboCheck;
	private PlayerInput playerInput;
	private PlayerMovement playerMovement;
	private PlayerDefense playerDefense;
	private Pause playerPause;

	void Awake()
	{
		//PlayerInput component assigns a unique player index so we can use it to identify the player
		playerInput = GetComponent<PlayerInput>(); 
		if(playerInput != null)
		{
			playerIndex = playerInput.playerIndex;
		}
		var movements = FindObjectsOfType<PlayerMovement>();
		var defense = FindObjectsOfType<PlayerDefense>();
		var pause = FindObjectsOfType<Pause>();
        var combo = FindObjectsOfType<ComboCheck>();
		playerMovement = movements.FirstOrDefault(m => m.GetPlayerIndex() == playerIndex);
		playerDefense = defense.FirstOrDefault(d => d.GetPlayerIndex() == playerIndex);
		playerPause = pause.FirstOrDefault(p => p.GetPlayerIndex() == playerIndex);
		comboCheck = combo.FirstOrDefault(c => c.GetPlayerIndex() == playerIndex);
	}
	
	public void OnJoin(InputAction.CallbackContext context)
	{
	    if(context.started)
	    {
            comboCheck.OnPlayerJoined(this);
            Debug.Log("Player joined: " + playerIndex);
	    }
	}

    public void OnAttackE(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Attack E Input Received");
            OnAttackInput?.Invoke(AttackType.attackE);
        }
    }
    public void OnAttackR(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Attack R Input Received");
            OnAttackInput?.Invoke(AttackType.attackR);
        }
    }
    public void OnMove(InputAction.CallbackContext context){
        Debug.Log("Move Input Received: " + context.ReadValue<Vector2>());
        playerMovement.OnMovement(context);
    }
    public void OnPause(InputAction.CallbackContext context)
    {
        playerPause.OnPause(context);
    }
    public void OnJump(InputAction.CallbackContext context){
        playerMovement.Jump(context);
    }
    public void OnDash(InputAction.CallbackContext context){
        playerMovement.Dash(context);
    }
    
    public void OnBlock(InputAction.CallbackContext context){
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

    public void OnParry(InputAction.CallbackContext context)
    {
        playerDefense.OnParryPerformed(context);
    }
	//come back for uptilt and downtilt and air attacks
    // Add similar methods for other attack types if needed
}