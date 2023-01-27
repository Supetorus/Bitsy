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

	[SerializeField] private AudioClip sfx;
	private bool hacked = false;

	GameManager gm;
	MenuManager menuManager;
	//[SerializeField] private ObjectiveHandler objectiveHandler;

	public string InteractPrompt => _prompt;

	public bool Interact(Interactor interactor)
	{
		panelManager = FindObjectOfType<PanelManager>();
		gm = FindObjectOfType<GameManager>();

		questItem.questText = "Objective Completed";
		questItem.gameObject.SetActive(true);
		questItem.AnimateQuest();

		gm.hud.gameObject.SetActive(false);
		gm.mainMenu.gameObject.SetActive(true);
		
		panelManager.OpenPanel(panelManager.panels[6].panelName);

		//menuManager.ActivateMenu();
		gm.playCamera.SetActive(false);
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		gm.menuCamera.SetActive(true);

		if (interactor.tag == "TestWin")
		{
			//panelManager.OpenPanelByIndex(6);
			//panelManager.ShowCurrentPanel();
		}
		/*if (interactor.tag == "TestLose")
		{
			panelManager.currentPanelIndex = 7;
		}*/

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
