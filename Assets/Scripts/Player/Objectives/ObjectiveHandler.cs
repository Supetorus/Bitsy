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

	public void DisplayObjective()
	{
		questItem.questText = objectives.ElementAt(objectiveIndex).objectiveLabel;
	}
	public void DisplayTask()
	{
		questItem.questText = objectives.ElementAt(objectiveIndex).GetCurrentTask().taskLabel;
	}

	public void Progress(int o_index, int t_index)
	{
		CompleteTask(o_index, t_index);
		CompleteObjective(o_index);
		CheckCompleteLevel();
	}

	private void CompleteTask(int o_index, int t_index)
	{
		if (o_index == objectiveIndex)
		{
			objectives[o_index].CompleteTask(t_index);
		}
	}

	private void CompleteObjective(int o_index)
	{
		if (objectives[o_index].CheckCompleteObjective())
		{
			objectiveIndex++;
			DisplayObjective();
		}
		else
		{
			DisplayTask();
		}
	}

	private void CheckCompleteLevel()
	{
		if (objectiveIndex == objectives.Count)
		{
			//TODO complete the level
		}
	}
}