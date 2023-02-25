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

	private bool isStunned;
	private GameObject player;

	private void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
	}

	private void OnTriggerStay(Collider other)
	{
		if(!isStunned)
		{
			if (other.TryGetComponent<Smoke>(out _)) return;

			if (other.gameObject == player)
			{
				if (doesDamage)
				{
					other.GetComponent<Health>().TakeDamage(dps * Time.deltaTime);
				}
				else
				{
					other.GetComponent<GlobalPlayerDetection>().ChangeDetection(dps * Time.deltaTime, true);
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
		StartCoroutine(GetStunnedIdiot(stunDuration));
	}

	IEnumerator GetStunnedIdiot(float stunDuration)
	{
		isStunned = true;
		gameObject.GetComponent<MeshRenderer>().enabled = false;
		yield return new WaitForSeconds(stunDuration);
		isStunned = false;
		gameObject.GetComponent<MeshRenderer>().enabled = true;
	}
}
