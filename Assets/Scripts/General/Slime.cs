using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{

	[SerializeField, Tooltip("Multiply MaxVelocity by this number"), Range(0.01f, 0.99f)]
	public float speedDecrease;
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
