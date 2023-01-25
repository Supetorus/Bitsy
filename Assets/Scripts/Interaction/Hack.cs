using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Michsky.UI.Reach;

public class Hack : MonoBehaviour, IInteractable
{
    [SerializeField] private QuestItem questItem;
    private PanelManager panelManager;
    [SerializeField] private string _prompt;
    [SerializeField] private TMP_Text _promptText;
    

    MenuManager menuManager;
    //[SerializeField] private ObjectiveHandler objectiveHandler;

    public string InteractPrompt => _prompt;

    private void Start()
    {
    }

    public bool Interact(Interactor interactor)
    {
        panelManager = FindObjectOfType<PanelManager>();
        //_promptText.text = questItem.questText;
        questItem.questText = "Steve";
        questItem.gameObject.SetActive(true);

        if (interactor.tag == "TestWin")
        {
            menuManager = GameObject.FindWithTag("MainMenu").GetComponent<MenuManager>();
            menuManager.ActivateMenu();
            panelManager.currentPanelIndex = 6;
        }
        if (interactor.tag == "TestLose")
        {
            panelManager.currentPanelIndex = 7;
        }

        return true;
    }

    public void UpdateQuest()
    {

    }
}
