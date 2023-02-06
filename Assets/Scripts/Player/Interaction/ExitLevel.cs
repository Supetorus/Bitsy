using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitLevel : MonoBehaviour, IInteractable {

	[SerializeField] private string _prompt;
	[SerializeField] private string feedbackText;

	[SerializeField] private AudioClip sfx;
	private bool hacked = false;

	GameManager gm;
	MenuManager menuManager;
	//[SerializeField] private ObjectiveHandler objectiveHandler;
	public string InteractPrompt => _prompt;

	public string FeedbackText => feedbackText;

	public bool CanInteract => !hacked;

	public bool Interact(Interactor interactor) {
		if (!hacked) {
			return true;
		}
		return false;
	}
}
