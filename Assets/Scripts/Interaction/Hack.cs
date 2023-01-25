using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Michsky.UI.Reach;

public class Hack : MonoBehaviour, IInteractable
{
    [SerializeField] private QuestItem questItem;
    //[SerializeField] private PanelManager panelManager;
    [SerializeField] private string _prompt;
    [SerializeField] private TMP_Text _promptText;

    //[SerializeField] private ObjectiveHandler objectiveHandler;

    public string InteractPrompt => _prompt;

    private void Start()
    {
        
    }

    public bool Interact(Interactor interactor)
    {
        _promptText.text = questItem.questText;
        questItem.enabled = true;
        if (interactor.tag == "TestWin")
        {
            //panelManager = ;
            //panelManager.currentPanelIndex = 6;
        }
        if (interactor.tag == "TestLose")
        {
            //panelManager.currentPanelIndex = 7;
        }

        return true;
    }

    public void UpdateQuest()
    {

    }
}
