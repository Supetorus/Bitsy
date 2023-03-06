using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskLoader : MonoBehaviour
{
	[SerializeField] private List<Task> tasks;
	[SerializeField] private TaskType taskType;

	enum TaskType
	{
		Interact,
		Trigger
	}

	void Start()
	{
		foreach (var task in tasks)
		{
			if (task.o_index == Player.ObjectiveHandler.objective.index)
			{
				if (taskType == TaskType.Interact)
				{
					TaskInteract ti = gameObject.AddComponent<TaskInteract>();
					ti.objectiveIndex = task.o_index;
					ti.taskIndex = task.index;

				}
				else if (taskType == TaskType.Trigger)
				{
					TaskTrigger tt = gameObject.AddComponent<TaskTrigger>();
					tt.objectiveIndex = task.o_index;
					tt.taskIndex = task.index;
				}
				break;
			}
		}
		Destroy(this);
	}
}
