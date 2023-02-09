using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Michsky.UI.Reach;

public class Hack : MonoBehaviour, IInteractable
{
	//[SerializeField] private QuestItem questItem;
	//private PanelManager panelManager;
	[SerializeField] private string _prompt;
	[SerializeField] private string feedbackText;
	public GameObject openDoor1;
	public GameObject openDoor2;
	public float MoveAmount;

	[SerializeField] private AudioClip sfx;
	private bool hacked = false;

	//GameManager gm;
	//MenuManager menuManager;
	//[SerializeField] private ObjectiveHandler objectiveHandler;

	public string InteractPrompt => _prompt;

	public string FeedbackText => feedbackText;

	public bool CanInteract => !hacked;

	public bool Interact(Interactor interactor)
	{
		if (!hacked)
		{
			//panelManager = FindObjectOfType<PanelManager>();
			//gm = FindObjectOfType<GameManager>();

			//questItem.questText = "Objective Completed";
			//questItem.ExpandQuest();



			//WIN stuff - shouldn't go here
			/*gm.hud.gameObject.SetActive(false);
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
			if (interactor.tag == "TestLose")
			{
				panelManager.currentPanelIndex = 7;
			}*/

			hacked = true;
			AudioSource.PlayClipAtPoint(sfx, transform.position);

			Debug.Log("HACKED");
			openDoor1.transform.position = new Vector3(openDoor1.transform.position.x, openDoor1.transform.position.y, openDoor1.transform.position.z + MoveAmount);
			openDoor2.transform.position = new Vector3(openDoor2.transform.position.x, openDoor2.transform.position.y, openDoor2.transform.position.z - MoveAmount);

			return true;
		}
		return false;
	}
}
