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

	GameManager gm;
	MenuManager menuManager;
	//[SerializeField] private ObjectiveHandler objectiveHandler;

	public string InteractPrompt => _prompt;

	private void Start()
	{
	}

	public bool Interact(Interactor interactor)
	{
		panelManager = FindObjectOfType<PanelManager>();

		questItem.questText = "Objective Completed";
		questItem.gameObject.SetActive(true);

		if (interactor.tag == "TestWin" || interactor.tag == "TestLose")
		{
			gm = FindObjectOfType<GameManager>();
			gm.mainMenu.gameObject.SetActive(true);
			gm.hud.gameObject.SetActive(false);
			//menuManager.ActivateMenu();
			if (interactor.tag == "TestWin")
			{
				panelManager.OpenPanel("Mission Success");
				//panelManager.ShowCurrentPanel();

			}
			/*if (interactor.tag == "TestLose")
			{
				panelManager.currentPanelIndex = 7;
			}*/
		}

		return true;
	}

	public void UpdateQuest()
	{

	}
}
