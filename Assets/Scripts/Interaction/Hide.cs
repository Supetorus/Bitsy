using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hide : MonoBehaviour, IInteractable
{
	[SerializeField] private string _prompt;
	[SerializeField] private AudioSource sfx;
	[SerializeField] private AudioClip opensfx;

	private bool hiding = false;


	public string InteractPrompt => _prompt;

	public bool Interact(Interactor interactor)
	{
		sfx.PlayOneShot(opensfx);
		Debug.Log(_prompt);
		return true;
	}
}
