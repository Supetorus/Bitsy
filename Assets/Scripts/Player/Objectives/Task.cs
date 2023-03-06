using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Task", menuName = "Objective/TaskSO")]
public class Task : ScriptableObject
{
	[SerializeField] public int index;
	[SerializeField] public int o_index;
	[SerializeField] private bool optional;
	[SerializeField] private bool complete;
	[SerializeField] public string taskLabel;

	public bool CompleteTask(int t_index)
	{
		complete = t_index == index;
		return complete;
	}

	public bool IsComplete() { return complete; }

	public void ResetTask() { complete = false; }
}