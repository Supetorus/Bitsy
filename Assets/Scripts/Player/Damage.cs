using System.Collections;
using System.Collections.Generic;
using Unity.Profiling.LowLevel;
using UnityEngine;

public class Damage : MonoBehaviour
{
	public bool instantKill = false;
	public float damagePerSecond = 1;

	private void OnCollisionStay(Collision collision)
	{
		Health health = collision.gameObject.GetComponent<Health>();
		if (health != null) DoDamage(health);
	}

	private void OnTriggerStay(Collider other)
	{
		Health health = other.GetComponent<Health>();
		if (health != null) DoDamage(health);
	}

	private void DoDamage(Health health)
	{
		if (instantKill)
		{
			health.InstantKill();
			return;
		}

		health.TakeDamage(damagePerSecond * Time.fixedDeltaTime);

	}
}
