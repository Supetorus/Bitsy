using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour, IInteractable
{
	[SerializeField] private string _prompt;
	[SerializeField] private string itemKey;

	public string InteractPrompt => _prompt;

	public bool Interact(Interactor interactor)
	{

		interactor.inventory.gameData.Save(itemKey, itemKey);
		Debug.Log(_prompt);
		return true;
	}
}
