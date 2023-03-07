using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskInteract : MonoBehaviour
{
	[SerializeField] public int objectiveIndex = 0;
	[SerializeField] public int taskIndex = 0;
	[SerializeField] private bool triggered = false;
	public void Interact()
	{
		if (!triggered)
		{
			Player.ObjectiveHandler.Progress(objectiveIndex, taskIndex);
			triggered = Player.ObjectiveHandler.CheckCompleteTask(objectiveIndex, taskIndex);
		}
	}
}
