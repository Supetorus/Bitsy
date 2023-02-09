using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Michsky.UI.Reach;
public class ObjectiveHandler : MonoBehaviour
{
	[SerializeField] QuestItem questItem;
	public List<Objective> objectives;
	public int objectiveIndex = 0;

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
		questItem.questText = objectives.ElementAt(objectiveIndex).objectiveLabel;
		questItem.AnimateQuest();
	}
	public void DisplayTask()
	{
		questItem.questText = objectives.ElementAt(objectiveIndex).GetCurrentTask().taskLabel;
		questItem.AnimateQuest();
	}

	public void Progress(int o_index, int t_index)
	{
		CompleteTask(o_index, t_index);
		CompleteObjective(o_index, t_index);
		CheckCompleteLevel();
	}

	public bool CheckCompleteTask(int o_index, int t_index)
	{
		return objectives[o_index].GetTaskAtIndex(t_index).IsComplete();
	}

	private void CompleteTask(int o_index, int t_index)
	{
		if (o_index == objectiveIndex)
		{
			objectives[o_index].CompleteTask(t_index);
		}
	}
	private void CompleteObjective(int o_index, int t_index)
	{
		if (o_index == objectiveIndex)
		{
			if (objectives[o_index].CheckCompleteObjective())
			{
				objectiveIndex++;
				if (objectiveIndex != objectives.Count) DisplayObjective();
			}
			else if (t_index == objectives[o_index].taskIndex - 1)
			{
				DisplayTask();
			}
		}
	}
	private void CheckCompleteLevel()
	{
		if (objectiveIndex == objectives.Count)
		{
			//complete the level
			//Do dnot change the order to this it'll break 

			gm = FindObjectOfType<GameManager>();

			gm.hud.gameObject.SetActive(false);
			gm.mainMenu.gameObject.SetActive(true);

			panelManager = FindObjectOfType<PanelManager>();

			panelManager.OpenPanel(panelManager.panels[6].panelName);

			//menuManager.ActivateMenu();
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;

			gm.menuCamera.SetActive(true);
			gm.playCamera.SetActive(false);

			Debug.Log("Level Complete");
		}
	}

	public void ResetObjective()
	{
		objectiveIndex = 0;
		foreach (var objective in objectives)
		{
			objective.ResetObjective();
		}
	}
}