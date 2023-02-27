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
	[HideInInspector] private float fireTimer;

	GameObject player;
	TurretAnimator turretAnimator;
	bool canSeePlayer;
	private float timeToRotate = 1.5f;
	private float rotTimer;
	private bool isStunned = false;

	private int currentSpawnLocation;

	private Vector3 playerDir;

	public override bool CheckSightlines()
	{
		return canSeePlayer;
	}

	public override void DartRespond()
	{
		//IMPLEMENT
		Destroy(gameObject);
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
			canSeePlayer = true;
			player.GetComponent<GlobalPlayerDetection>().ChangeDetection(150 * Time.deltaTime);
			turretAnimator.animator.enabled = false;
			weapon.rotation = Quaternion.Slerp(weapon.rotation, Quaternion.LookRotation(playerDir, transform.up), rotTimer / timeToRotate);
		}
		else
		{
			rotTimer = 0;
			canSeePlayer = false;
			turretAnimator.animator.enabled = true;
			turretAnimator.animator.SetBool("isActive", true);
		}

		if (fireTimer <= 0 && canSeePlayer)
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

	//private void OnDrawGizmosSelected()
	//{
	//	if (!Application.isPlaying) return;

	//	Physics.Raycast(transform.position, player.transform.position - transform.position, out RaycastHit hit, float.MaxValue, myMask);
	//	Gizmos.DrawLine(transform.position, hit.point);
	//}
}
