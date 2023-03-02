using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
	public float speedDecrease = 0.5f;
	private void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent<MovementController>(out MovementController player))
		{
			player.maxVelocity *= speedDecrease;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.TryGetComponent<MovementController>(out MovementController player))
		{
			player.maxVelocity /= speedDecrease;
		}
	}
}
