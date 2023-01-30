using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
	[SerializeField] private string _prompt;
	[SerializeField] private string requiredItemKey;
	[SerializeField] private AudioSource sfx;
	[SerializeField] private AudioClip errorsfx;
	[SerializeField] private AudioClip opensfx;
	[SerializeField] private AudioClip closesfx;
	[SerializeField] private GameObject OtherDoor;
	[SerializeField] private float MoveAmount;

	private bool opened = false;
	private bool locked = false;

	public string InteractPrompt => _prompt;

	private void Start()
	{
		if (!string.IsNullOrEmpty(requiredItemKey)) locked = true;
	}

	public bool Interact(Interactor interactor)
	{
		if (opened)
		{
			opened = false;
			sfx.PlayOneShot(closesfx);
			return true;
		}
		else if ((interactor.inventory.gameData.stringData.HasKey(requiredItemKey) && locked) || !locked)
		{
			locked = false;
			opened = true;
			Debug.Log(_prompt);
			sfx.PlayOneShot(opensfx);
			//update to play open on open animation and close on close animation when animator is added
			// open door
			if(OtherDoor != null) {
				Debug.Log(OtherDoor.transform.position);
				OtherDoor.transform.position = new Vector3(OtherDoor.transform.position.x, OtherDoor.transform.position.y + MoveAmount, OtherDoor.transform.position.z);
				Debug.Log(OtherDoor.transform.position);
			}
			return true;
		}
		else
		{
			sfx.PlayOneShot(errorsfx);
			// prompt "I don't have a key for this"
			Debug.Log("I can't open that.");
			return false;
		}
	}
}