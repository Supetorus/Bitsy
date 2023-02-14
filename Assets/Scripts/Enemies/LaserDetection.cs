using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDetection : DetectionEnemy
{
	public bool doesDamage;
	public float dps = 1;
	public List<GameObject> alarmsLight;
	public bool isStunned;

	GameObject player;

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
					//Lower the players Health
					other.GetComponent<Health>().TakeDamage(dps * Time.deltaTime);
				}
				else
				{
					//Increase the detection meter
					other.GetComponent<GlobalPlayerDetection>().ChangeDetection(0.25f, true);
					foreach (var alarm in FindObjectsOfType<Alarm>())
					{
						alarm.Play();
					}
					foreach (var light in alarmsLight)
					{
						light.transform.GetChild(1).gameObject.SetActive(true);
						light.transform.GetChild(2).gameObject.SetActive(true);
					}
				}
			}
		}
	}

	public override bool CheckSightlines()
	{
		return false;
	}

	public override void DartRespond()
	{
		//DOES NOT RESPOND TO TROJAN DART
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
