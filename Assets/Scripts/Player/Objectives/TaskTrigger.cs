using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskTrigger : MonoBehaviour
{
	[SerializeField] public int objectiveIndex = 0;
	[SerializeField] public int taskIndex = 0;
	[SerializeField] private bool triggered = false;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") && !triggered)
		{
			Player.ObjectiveHandler.Progress(objectiveIndex, taskIndex);
			triggered = Player.ObjectiveHandler.CheckCompleteTask(objectiveIndex, taskIndex);
		}
	}
}
