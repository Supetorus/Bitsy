using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskInteract : MonoBehaviour
{
	[SerializeField] private int objectiveIndex = 0;
	[SerializeField] private int taskIndex = 0;
	[SerializeField] private bool triggered = false;
	public void Interact(GameObject other)
	{
		if (other.CompareTag("Player") && !triggered)
		{
			other.GetComponent<ObjectiveHandler>().Progress(objectiveIndex, taskIndex);
			triggered = true;
		}
	}
}
