using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Objecive", menuName = "Objective/ObjectiveSO")]
public class Objective : ScriptableObject
{
	[SerializeField] public int index;
	[SerializeField] private bool complete;
	[SerializeField] public string objectiveLabel;
	[SerializeField] private List<Task> tasks;
	public int taskIndex = 0;

	[SerializeField, TextArea] public string objectiveDescription;

	public Task GetCurrentTask()
	{
		return tasks[taskIndex];
	}

	public Task GetTaskAtIndex(int t_index)
	{
		return tasks[t_index];
	}

	public bool CheckCompleteObjective()
	{
		return complete;
	}

	public void CompleteTask(int t_index)
	{
		if (t_index == taskIndex)
		{
			if (tasks[taskIndex].CompleteTask(taskIndex))
			{
				//Debug.Log("Obective " + index + ", Task " + taskIndex + " Complete");
				taskIndex++;
			}
		}
		complete = (taskIndex == tasks.Count);
	}

	public void ResetObjective()
	{
		complete = false;
		taskIndex = 0;
		foreach (var task in tasks)
		{
			task.ResetTask();
		}
	}
}