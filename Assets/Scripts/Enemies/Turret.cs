using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : DetectionEnemy
{
	[SerializeField] private GameObject projectile;
	[SerializeField] private Transform[] spawnLocations;
	[SerializeField] private Transform weapon;
	[SerializeField] private LayerMask myMask;
	[SerializeField] private float projSpeed;
	[SerializeField] private float fireRate;
	[SerializeField] GameObject deathExplode;
	[HideInInspector] private float fireTimer;

	GameObject player;
	TurretAnimator turretAnimator;
	private float timeToRotate = 1.5f;
	private float rotTimer;
	private bool isStunned = false;

	private int currentSpawnLocation;

	private Vector3 playerDir;

	public override bool CheckSightlines()
	{
		return CanSeePlayer;
	}

	public override void DartRespond()
	{
		Instantiate(deathExplode, transform.position, transform.rotation);
		Destroy(gameObject, 0.5f);
	}

	public override void EMPRespond(float stunDuration, GameObject stunEffect)
	{
		StartCoroutine(GetStunnedIdiot(stunDuration, stunEffect));
	}

	IEnumerator GetStunnedIdiot(float stunDuration, GameObject stunEffect)
	{
		GameObject stunParticles = Instantiate(stunEffect, transform.position, transform.rotation);
		isStunned = true;
		turretAnimator.animator.enabled = false;
		yield return new WaitForSeconds(stunDuration);
		isStunned = false;
		Destroy(stunParticles);
		turretAnimator.animator.enabled = true;
	}

	// Start is called before the first frame update
	void Start()
	{
		turretAnimator = GetComponentInParent<TurretAnimator>();
		player = GameObject.FindGameObjectWithTag("Player");
		fireTimer = fireRate;
	}

	// Update is called once per frame
	void Update()
	{
		if (isStunned) return;

		playerDir = (player.transform.position - transform.position).normalized;
		if (Physics.Raycast(transform.position, playerDir, out RaycastHit hit, float.MaxValue, myMask) &&
			hit.collider.gameObject == player)
		{
			rotTimer += Time.deltaTime;
			CanSeePlayer = true;
			player.GetComponent<DetectionLevel>().ChangeDetection(150 * Time.deltaTime);
			turretAnimator.animator.enabled = false;
			weapon.rotation = Quaternion.Slerp(weapon.rotation, Quaternion.LookRotation(playerDir, transform.up), rotTimer / timeToRotate);
		}
		else
		{
			rotTimer = 0;
			CanSeePlayer = false;
			turretAnimator.animator.enabled = true;
			turretAnimator.animator.SetBool("isActive", true);
		}

		if (fireTimer <= 0 && CanSeePlayer)
		{
			if (hit.collider.gameObject == player && Vector3.Dot(weapon.forward, playerDir) > 0.95f)
			{
				GameObject bullet = Instantiate(projectile, spawnLocations[currentSpawnLocation].position, transform.rotation);
				bullet.transform.rotation = Quaternion.LookRotation(playerDir);
				bullet.GetComponentInChildren<Rigidbody>().AddForce((player.transform.position - spawnLocations[currentSpawnLocation].position).normalized * projSpeed);
				currentSpawnLocation = (currentSpawnLocation + 1) % spawnLocations.Length;
				fireTimer = fireRate;
			}
		}
		fireTimer -= Time.deltaTime;
	}
}
