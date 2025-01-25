using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BasicGrab : MonoBehaviour
{
    
    private Rigidbody2D otherPlayer;
    private bool grabAble = false;
    private bool player1; //bool for being player1

    private void Start() 
    {
        if(gameObject.tag == "Player1")
        {
            player1 = true;
        }
        else
        {
            player1 = false;
        }

        grabAble = true;
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        // Debug.Log("Collision");
        // if (other.gameObject.tag == "Player1" || other.gameObject.tag == "Player2") 
        // {
        //     grabAble = true;
        //     otherPlayer = other.gameObject.GetComponent<Rigidbody2D>();
        // }
    }

    private void OnCollisionExit2D(Collision2D other) 
    {
        // if (other.gameObject.tag == "Player1" || other.gameObject.tag == "Player2") 
        // {
        //     grabAble = false;
        // }
    }

    void OnGrab(InputValue value)
    {
        if (value.isPressed && grabAble)
        {
            if(player1)
            {
                GameManager.Instance.P2frozen = true;
            }
            else
            {
                GameManager.Instance.P1frozen = true;
            }

            //if other player is to the left of this player
            if(otherPlayer.transform.position.x < transform.position.x)
            {
                otherPlayer.velocity = new Vector2(-5, 3);
            }
            else
            {
                otherPlayer.velocity = new Vector2(5, 3);
            }
    
            StartCoroutine(UnfreezePlayer());
        }
    }

    IEnumerator UnfreezePlayer()
    {
        yield return new WaitForSeconds(1);
        if(player1)
        {
            GameManager.Instance.P2frozen = false;
        }
        else
        {
            GameManager.Instance.P1frozen = false;
        }
    }
}
