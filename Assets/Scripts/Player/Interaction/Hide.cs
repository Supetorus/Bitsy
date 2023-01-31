using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hide : MonoBehaviour, IInteractable
{
	[SerializeField] private string _prompt;
	[SerializeField] private string feedbackText;
	[SerializeField] private AudioSource sfx;
	[SerializeField] private AudioClip opensfx;

	private bool hiding = false;


	public string InteractPrompt => _prompt;
	public string FeedbackText => feedbackText;

	public bool CanInteract => true;

	public bool Interact(Interactor interactor)
	{
		sfx.PlayOneShot(opensfx);
		Debug.Log(_prompt);
		return true;
	}
}
