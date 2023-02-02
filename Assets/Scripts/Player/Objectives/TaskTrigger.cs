using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskTrigger : MonoBehaviour
{
	[SerializeField] private int objectiveIndex = 0;
	[SerializeField] private int taskIndex = 0;
	[SerializeField] private bool triggered = false;

	private void OnTriggerEnter(Collider other)
	{
		//Debug.Log("TaskTrigger collision detected with " + other.name);
		if (other.CompareTag("Player") && !triggered)
		{
			other.GetComponent<ObjectiveHandler>().Progress(objectiveIndex, taskIndex);
			triggered = other.GetComponent<ObjectiveHandler>().CheckCompleteTask(objectiveIndex, taskIndex);
		}
	}
}
