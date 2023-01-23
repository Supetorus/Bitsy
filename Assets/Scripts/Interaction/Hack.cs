using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Michsky.UI.Reach;

public class Hack : MonoBehaviour, IInteractable
{
    [SerializeField] private QuestItem questItem;
    [SerializeField] private string _prompt;
    [SerializeField] private TMP_Text _promptText;

    public string InteractPrompt => _prompt;

    public bool Interact(Interactor interactor)
    {
        _promptText.text = questItem.questText;
        Debug.Log(_prompt);
        return true;
    }
}
