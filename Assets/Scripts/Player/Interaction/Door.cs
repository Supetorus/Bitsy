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
	[SerializeField] private GameObject RightDoor;
	[SerializeField] private GameObject LeftDoor;
	[SerializeField] private Door OtherPanel;
	[SerializeField] private bool OpensOnX;
	[SerializeField] private bool OpensOnZ;
	[SerializeField] private bool RotateToOpen;
	[SerializeField] private float MoveAmount;

	private bool open = false;
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
		if (open) return false;
		if ((interactor.inventory.gameData.stringData.HasKey(requiredItemKey) && locked) || !locked)
		{
			open = true;
			locked = false;
			if(OtherPanel != null) {
				OtherPanel.open = true;
				OtherPanel.locked = false;
			}
			sfx.PlayOneShot(opensfx);
			//update to play open on open animation and close on close animation when animator is added
			// open door
			if (LeftDoor == null)
			{
				if (RotateToOpen)
				{
					RightDoor.transform.eulerAngles = new Vector3(RightDoor.transform.eulerAngles.x, RightDoor.transform.eulerAngles.y + MoveAmount, RightDoor.transform.eulerAngles.x);
				}
				else
				{
					RightDoor.transform.position = new Vector3(RightDoor.transform.position.x, RightDoor.transform.position.y + MoveAmount, RightDoor.transform.position.z);
				}
			}
			else if (OpensOnX)
			{
				RightDoor.transform.position = new Vector3(RightDoor.transform.position.x + MoveAmount, RightDoor.transform.position.y, RightDoor.transform.position.z);
				LeftDoor.transform.position = new Vector3(LeftDoor.transform.position.x - MoveAmount, LeftDoor.transform.position.y, LeftDoor.transform.position.z);
			}
			else if (OpensOnZ)
			{
				RightDoor.transform.position = new Vector3(RightDoor.transform.position.x, RightDoor.transform.position.y, RightDoor.transform.position.z - MoveAmount);
				LeftDoor.transform.position = new Vector3(LeftDoor.transform.position.x, LeftDoor.transform.position.y, LeftDoor.transform.position.z + MoveAmount);
			}

			return true;
		}
		else
		{
			sfx.PlayOneShot(errorsfx);
			return false;
		}
	}
}