using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
  public string InteractPrompt { get; }

  public bool Interact(Interactor interactor);
}
