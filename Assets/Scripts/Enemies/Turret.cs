using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
	[SerializeField] float sightDist;
	[SerializeField] GameObject projectile;
	[SerializeField] Transform[] spawnLocations;
	[SerializeField] Transform weapon;
	[SerializeField] LayerMask myMask;
	[SerializeField] float projSpeed;
	[SerializeField] float fireRate;
	[HideInInspector] float fireTimer;

	GameObject player;
	TurretAnimator turretAnimator;
	bool canSeePlayer;
	private float timeToRotate = 1.5f;
	private float rotTimer;

	private int currentSpawnLocation;

	private Vector3 playerDir;

    // Start is called before the first frame update
    void Start()
    {
		turretAnimator = GetComponentInParent<TurretAnimator>();
		player = GameObject.FindGameObjectWithTag("Player");
		fireTimer = fireRate;
    }

	public bool CheckSightlines()
	{
		playerDir = (player.transform.position - transform.position).normalized;
		if (Physics.Raycast(transform.position, playerDir, out RaycastHit hit, sightDist, myMask))
		{
			if (hit.collider.gameObject == player && player.GetComponent<AbilityController>().isVisible)
			{
				rotTimer += Time.deltaTime;
				canSeePlayer = true;
				turretAnimator.animator.enabled = false;
				weapon.rotation = Quaternion.Slerp(weapon.rotation, Quaternion.LookRotation(playerDir, weapon.up), rotTimer / timeToRotate);
				return true;
			}
		}
		else
		{
			rotTimer = 0;
			canSeePlayer = false;
			turretAnimator.animator.enabled = true;
			turretAnimator.animator.SetBool("isActive", true);
		}
		return false;
	}

    // Update is called once per frame
    void Update()
    {
		if (fireTimer <= 0 && canSeePlayer && CheckSightlines())
		{
			Physics.Raycast(transform.position, playerDir, out RaycastHit hit, sightDist, myMask);
			if (hit.collider.gameObject == player && player.GetComponent<AbilityController>().isVisible && Vector3.Dot(weapon.forward, playerDir) > 0.95f)
			{
				GameObject bullet = Instantiate(projectile, spawnLocations[currentSpawnLocation].position, transform.rotation);
				bullet.transform.rotation = Quaternion.LookRotation(playerDir);
				bullet.GetComponent<Rigidbody>().AddForce((player.transform.position - spawnLocations[currentSpawnLocation].position).normalized * projSpeed);
				Destroy(bullet, 1);
				player.GetComponent<GlobalPlayerDetection>().ChangeDetection(0.25f, true);
				currentSpawnLocation = (currentSpawnLocation + 1) % spawnLocations.Length;
				fireTimer = fireRate;
			}
		}
		else
		{
			fireTimer -= Time.deltaTime;
		}
    }
}
