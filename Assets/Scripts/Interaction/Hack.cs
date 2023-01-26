using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hack : MonoBehaviour, IInteractable
{
	[SerializeField] private string _prompt;
	[SerializeField] private AudioClip sfx;
	private bool hacked = false;

	public string InteractPrompt => _prompt;

	public bool Interact(Interactor interactor)
	{
		if (!hacked)
		{
			hacked = true;
			AudioSource.PlayClipAtPoint(sfx, transform.position);
			Debug.Log(_prompt);
			return true;
		}
		return false;
	}
}
