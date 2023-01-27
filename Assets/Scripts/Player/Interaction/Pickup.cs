using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour, IInteractable
{
	[SerializeField] private string _prompt;
	[SerializeField] private string itemKey;
	[SerializeField] private AudioSource sfx;


	public string InteractPrompt => _prompt;

	public bool Interact(Interactor interactor)
	{
		sfx.PlayOneShot(sfx.clip);
		interactor.inventory.gameData.Save(itemKey, itemKey);
		Debug.Log(_prompt);
		Destroy(gameObject, 1.0f);
		return true;
	}
}
