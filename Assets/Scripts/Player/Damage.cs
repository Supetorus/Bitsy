using System.Collections;
using System.Collections.Generic;
using Unity.Profiling.LowLevel;
using UnityEngine;
using UnityEngine.Events;

public class Damage : MonoBehaviour
{
	[SerializeField]
	bool doDamageOverTime;
	[SerializeField]
	private bool instantKill = false;
	[SerializeField]
	private float damage = 1;
	[SerializeField, Tooltip("How long after collision the object will be destroyed. 0 for instant, negative for never.")]
	private float destroyTime;
	[SerializeField, Tooltip("The object that is destroyed when the timer is up. Usually the parent part of the prefab if it is a projectile.")]
	private GameObject destroyed;
	[SerializeField] private GameObject SparkVFX;

	public UnityEvent onDamage;
	public UnityEvent onDOT;


	private void OnCollisionEnter(Collision collision)
	{
		Health health;
		if (collision.gameObject.CompareTag("Player")) {
			health = Player.Health;
			GameObject spark = Instantiate(SparkVFX, collision.gameObject.transform.position, Quaternion.identity);
			Destroy(spark, spark.GetComponent<ParticleSystem>().main.duration);
		} else health = collision.gameObject.GetComponent<Health>();
		DoIt(health);
	}

	private void OnCollisionStay(Collision collision)
	{
		Health health;
		if (collision.gameObject.CompareTag("Player")) health = Player.Health;
		else health = collision.gameObject.GetComponent<Health>();
		DoIt(health);
	}

	private void OnTriggerEnter(Collider other)
	{
		Health health;
		if (other.CompareTag("Player")) health = Player.Health;
		else health = other.GetComponent<Health>();
		DoIt(health);
	}

	private void OnTriggerStay(Collider other)
	{
		Health health;
		if (other.CompareTag("Player")) health = Player.Health;
		else health = other.GetComponent<Health>();
		DoIt(health);
	}

	private void DoIt(Health health)
	{
		if (health != null)
		{
			if (doDamageOverTime) DoDamageOverTime(health);
			else DoDamage(health);
		}
		if (destroyTime >= 0) Destroy(destroyed != null ? destroyed : gameObject, destroyTime);
	}

	private void DoDamage(Health health)
	{
		if (instantKill)
		{
			health.InstantKill();
			return;
		}
		onDamage.Invoke();
		health.TakeDamage(damage);
	}

	private void DoDamageOverTime(Health health)
	{
		onDOT.Invoke();
		health.TakeDamage(damage * Time.fixedDeltaTime);
	}
}
