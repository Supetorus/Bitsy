using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hide : MonoBehaviour, IInteractable {
  [SerializeField] private string _prompt;

  public string InteractPrompt => _prompt;

  public bool Interact(Interactor interactor) {
    Debug.Log(_prompt);
    return true;
  }
}
