using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public class ComboData
{
	public string animationName;
	public float duration;
	public float damage;
	public float knockbackX;
	public float knockbackY;
	public float dashX;
	public float dashY;
	public bool stopMovementAll;
	public bool stopMovementY;
	public bool canCombo;
}