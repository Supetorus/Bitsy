using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
	[SerializeField] private string _prompt;
	[SerializeField] private string feedbackText;
	[SerializeField] private string requiredItemKey;
	[SerializeField] private AudioSource sfx;
	[SerializeField] private AudioClip errorsfx;
	[SerializeField] private AudioClip opensfx;
	[SerializeField] private AudioClip closesfx;
	[SerializeField] private GameObject OtherDoor1;
	[SerializeField] private GameObject OtherDoor2;
	[SerializeField] private bool OpensOnX;
	[SerializeField] private bool OpensOnZ;
	[SerializeField] private bool RotateToOpen;
	[SerializeField] private float MoveAmount;

	//private bool opened = false;
	private bool locked = false;

	public string InteractPrompt => _prompt;
	public string FeedbackText => feedbackText;

	public bool CanInteract => true;

	private void Start()
	{
		if (!string.IsNullOrEmpty(requiredItemKey)) locked = true;
	}

	public bool Interact(Interactor interactor)
	{
		/*if (opened)
		{
			opened = false;
			sfx.PlayOneShot(closesfx);
			return true;
		}
		else */if ((interactor.inventory.gameData.stringData.HasKey(requiredItemKey) && locked) || !locked)
		{
			locked = false;
			//opened = true;
			Debug.Log(_prompt);
			if (sfx != null) sfx.PlayOneShot(opensfx);
			else Debug.LogWarning("Sound effect source was null.");
			//update to play open on open animation and close on close animation when animator is added
			// open door
			if(OtherDoor2 == null) {
				if (RotateToOpen) {
					OtherDoor1.transform.eulerAngles = new Vector3(OtherDoor1.transform.eulerAngles.x, OtherDoor1.transform.eulerAngles.y + MoveAmount, OtherDoor1.transform.eulerAngles.x);
				} else {
					OtherDoor1.transform.position = new Vector3(OtherDoor1.transform.position.x, OtherDoor1.transform.position.y + MoveAmount, OtherDoor1.transform.position.z);
				}
			} else {
				if (OpensOnX) {
					OtherDoor1.transform.position = new Vector3(OtherDoor1.transform.position.x + MoveAmount, OtherDoor1.transform.position.y, OtherDoor1.transform.position.z);
					OtherDoor2.transform.position = new Vector3(OtherDoor2.transform.position.x - MoveAmount, OtherDoor2.transform.position.y, OtherDoor2.transform.position.z);
				}
				else if (OpensOnZ) {
					OtherDoor1.transform.position = new Vector3(OtherDoor1.transform.position.x, OtherDoor1.transform.position.y, OtherDoor1.transform.position.z + MoveAmount);
					OtherDoor2.transform.position = new Vector3(OtherDoor2.transform.position.x, OtherDoor2.transform.position.y, OtherDoor2.transform.position.z - MoveAmount);
				}
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