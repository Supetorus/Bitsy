using System.Collections;
using System.Collections.Generic;
using Unity.Profiling.LowLevel;
using UnityEngine;

public class Damage : MonoBehaviour
{
	[SerializeField] bool doDamageOverTime;
	[SerializeField] bool destroyOnCollide;
	public bool instantKill = false;
	public float damage = 1;

	private void OnCollisionEnter(Collision collision)
	{
		Health health = collision.gameObject.GetComponent<Health>();
		DoIt(health);
	}

	private void OnCollisionStay(Collision collision)
	{
		Health health = collision.gameObject.GetComponent<Health>();
		DoIt(health);
	}

	private void OnTriggerEnter(Collider other)
	{
		Health health = other.GetComponent<Health>();
		DoIt(health);
	}

	private void OnTriggerStay(Collider other)
	{
		Health health = other.GetComponent<Health>();
		DoIt(health);
	}

	private void DoIt(Health health)
	{
		if (health != null)
		{
			if (doDamageOverTime) DoDamageOverTime(health);
			else DoDamage(health);
		}
		if (destroyOnCollide) Destroy(gameObject);
	}

	private void DoDamage(Health health)
	{
		if (instantKill)
		{
			health.InstantKill();
			return;
		}
		health.TakeDamage(damage);
	}

	private void DoDamageOverTime(Health health)
	{
		health.TakeDamage(damage * Time.fixedDeltaTime);
	}
}
