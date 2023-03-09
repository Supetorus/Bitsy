﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour, IInteractable
{
	[SerializeField] private string _prompt;
	[SerializeField] private string feedbackText;
	[SerializeField] private string itemKey;
	//[SerializeField] private AudioSource sfx;
	[SerializeField] private AudioClip pickupsfx;


	public string InteractPrompt => _prompt;

	public string FeedbackText => feedbackText;

	public bool CanInteract => true;

	public bool Interact(Interactor interactor)
	{
		AudioSource.PlayClipAtPoint(pickupsfx, transform.position);
		interactor.inventory.gameData.Save(itemKey, itemKey);
		Destroy(gameObject, 0.1f);
		return true;
	}
}
