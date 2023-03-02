using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
	[SerializeField] float healAmount;
	private void OnTriggerEnter(Collider other)
	{
		if(other.TryGetComponent<Health>(out Health health))
		{
			health.Heal(healAmount);
			Destroy(gameObject);
		}
	}
}
