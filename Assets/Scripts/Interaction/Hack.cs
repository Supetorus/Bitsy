using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Hack : MonoBehaviour, IInteractable {
  [SerializeField] private string _prompt;
  [SerializeField] private TMP_Text _promptText;

  public string InteractPrompt => _prompt;

  public bool Interact(Interactor interactor) {
        _promptText.text = InteractPrompt;
    Debug.Log(_prompt);
    return true;
  }
}
