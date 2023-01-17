using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
	[SerializeField] private string _prompt;
	[SerializeField] private string requiredItemKey;

	public string InteractPrompt => _prompt;

	public bool Interact(Interactor interactor)
	{
		if (interactor.inventory.gameData.stringData.HasKey(requiredItemKey))
		{
			Debug.Log(_prompt);
			// open door
			return true;
		}
		else
		{
			// prompt "I don't have a key for this"
			Debug.Log("I can't open that.");
			return false;
		}
	}
}