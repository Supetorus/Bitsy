using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
	public string InteractPrompt { get; }
	public string FeedbackText { get; }
	public bool CanInteract { get; }

	public bool Interact(Interactor interactor);
}
