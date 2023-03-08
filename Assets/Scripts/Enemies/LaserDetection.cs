using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDetection : DetectionEnemy
{
	[SerializeField, Tooltip("Whether this turret does damage or increases detection.")]
	private bool doesDamage;
	[SerializeField, Tooltip("How much damage is done or detection is increased per second.")]
	private float dps = 1;
	[SerializeField] private GameObject SparkVFX;

	private bool isStunned;

	private void OnTriggerStay(Collider other)
	{
		if(!isStunned)
		{
			if (other.TryGetComponent<Smoke>(out _)) return;

			if (other.CompareTag("Player"))
			{
				if (doesDamage)
				{
					SparkVFX.transform.position = new Vector3(other.transform.position.x, other.transform.position.y, other.transform.position.z);
					SparkVFX.active = true;
					Player.Health.TakeDamage(dps * Time.deltaTime);
				}
				else if (Player.AbilityController.isVisible)
				{
					Player.Detection.ChangeDetection(dps * Time.deltaTime);
				}
			}
		}
	}

	private void OnTriggerExit(Collider other) {
		if (!isStunned) {
			if (other.TryGetComponent<Smoke>(out _)) return;

			if (other.CompareTag("Player")) {
				if (doesDamage) {
					SparkVFX.SetActive(false);
				}
			}
		}
	}

	public override bool CheckSightlines()
	{
		return false;
	}

	public override void EMPRespond(float stunDuration, GameObject stunEffect)
	{
		StartCoroutine(GetStunnedIdiot(stunDuration, stunEffect));
	}

	IEnumerator GetStunnedIdiot(float stunDuration, GameObject stunEffect)
	{
		isStunned = true;
		gameObject.GetComponent<MeshRenderer>().enabled = false;
		yield return new WaitForSeconds(stunDuration);
		isStunned = false;
		gameObject.GetComponent<MeshRenderer>().enabled = true;
	}
}
