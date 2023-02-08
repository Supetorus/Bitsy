using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
	[SerializeField] float sightDist;
	[SerializeField] GameObject projectile;
	[SerializeField] Transform weapon;
	[SerializeField] LayerMask myMask;
	[SerializeField] float projSpeed;
	[SerializeField] float fireRate;
	[HideInInspector] float fireTimer;

	GameObject player;
	TurretAnimator turretAnimator;
	bool canSeePlayer;

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
		Vector3 direction = (player.transform.position - transform.position).normalized;
		Debug.DrawRay(transform.position, direction);
		if (Physics.Raycast(transform.position, direction, out RaycastHit hit,  sightDist, myMask))
		{
			canSeePlayer = true;
			if (hit.collider.gameObject == player)
			{
				turretAnimator.animator.enabled = false;
				weapon.LookAt(player.transform);
			}
		}
		else
		{
			canSeePlayer = false;
			print("Animator on");
			turretAnimator.animator.enabled = true;
			turretAnimator.animator.SetBool("isActive", true);
		}
		
		if (fireTimer <= 0 && canSeePlayer)
		{
			if (hit.collider.gameObject == player)
			{
				GameObject bullet = Instantiate(projectile, transform.position, transform.rotation);
				bullet.GetComponent<Rigidbody>().AddForce(direction * projSpeed);
				fireTimer = fireRate;
			}
		}
		else
		{
			fireTimer -= Time.deltaTime;
		}
    }
}
