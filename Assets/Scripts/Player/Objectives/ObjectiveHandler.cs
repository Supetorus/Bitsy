using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Michsky.UI.Reach;
using UnityEngine.SceneManagement;

public class ObjectiveHandler : MonoBehaviour
{
	[SerializeField] QuestItem questItem;
	public Objective objective;

	GameManager gm;
	MenuManager menuManager;
	PanelManager panelManager;


	private void Start()
	{
		DisplayObjective();
		StopCoroutine("ShowTask");
		StartCoroutine("ShowTask");
	}

	IEnumerator ShowTask()
	{
		yield return new WaitForSeconds(questItem.minimizeAfter + 1);
		DisplayTask();
	}

	public void DisplayObjective()
	{
		if (objective == null) return; //This happens when the scene is run in testing without loading from SetupScene.
		questItem.questText = objective.objectiveLabel;
		questItem.AnimateQuest();
	}
	public void DisplayTask()
	{
		questItem.questText = objective.GetCurrentTask().taskLabel;
		questItem.AnimateQuest();
	}

	public void Progress(int o_index, int t_index)
	{
		CompleteTask(o_index, t_index);
		CompleteObjective(o_index, t_index);
	}

	public bool CheckCompleteTask(int o_index, int t_index)
	{
		if (objective.index != o_index) return false;
		return objective.GetTaskAtIndex(t_index).IsComplete();
	}

	private void CompleteTask(int o_index, int t_index)
	{
		if (o_index == objective.index)
		{
			objective.CompleteTask(t_index);
		}
	}
	private void CompleteObjective(int o_index, int t_index)
	{
		if (o_index == objective.index)
		{
			if (!objective.CheckCompleteObjective())
			{
				if (t_index == objective.taskIndex - 1)
				{
					DisplayTask();
				}
				//if (objectiveIndex != objectives.Count) DisplayObjective();
			}
			else
			{
				CompleteLevel();
			}
		}
	}
	private void CompleteLevel()
	{
		//complete the level
		//Do dnot change the order to this it'll break 

		gm = FindObjectOfType<GameManager>();

		gm.hud.gameObject.SetActive(false);
		gm.mainMenu.gameObject.SetActive(true);

		panelManager = FindObjectOfType<PanelManager>();

		panelManager.OpenPanel(panelManager.panels[6].panelName);

		var sceneName = SceneManager.GetActiveScene().name;
		print(sceneName);
		switch (sceneName)
		{
			case "Tutorial":
				if (PlayerPrefs.GetString("TutorialDone") == "FALSE")
				{
					PlayerPrefs.SetInt("LevelLock", 1);
					PlayerPrefs.SetString("TutorialDone", "TRUE");
				}
				break;
			case "Alpha 1.0 Level 1":
				if (PlayerPrefs.GetString("Level1Done") == "FALSE")
				{
					PlayerPrefs.SetInt("LevelLock", 2);
					PlayerPrefs.SetString("Level1Done", "TRUE");
				}
				break;
			case "Alpha 1.0 Level 2":
				if (PlayerPrefs.GetString("Level2Done") == "FALSE")
				{
					PlayerPrefs.SetInt("LevelLock", 3);
					PlayerPrefs.SetString("Level2Done", "TRUE");
				}
				break;
		}

		gm.UnloadCurrentScene();

		//menuManager.ActivateMenu();
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		gm.menuCamera.SetActive(true);
		gm.playCamera.SetActive(false);
		print(PlayerPrefs.GetInt("LevelLock"));
		print(PlayerPrefs.GetString("TutorialDone"));
		Debug.Log("Level Complete");
	}

	public void ResetObjective()
	{
		objective.ResetObjective();
	}
}