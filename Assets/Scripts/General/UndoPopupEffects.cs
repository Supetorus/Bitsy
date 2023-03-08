using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoPopupEffects : MonoBehaviour
{
	public static void UndoEffects()
	{
		var player = GameObject.Find("Player3.0");
		player.GetComponentInChildren<AbilityController>().enabled = true;
		Time.timeScale = 1;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}
}
