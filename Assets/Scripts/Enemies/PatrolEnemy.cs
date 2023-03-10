using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolEnemy : DetectionEnemy
{
	[SerializeField] private DetectionEnemy detector;
	[SerializeField] private float detectionThreshhold = 75;
	[SerializeField, Tooltip("How close the player has to be before this enemy cares and will try to find them")]
	private float playerCareDistance = 20;
	[Header("Navigation")]
	[SerializeField] List<GameObject> nodes;
	[SerializeField] NavMeshAgent agent;
	[SerializeField] Animator animator;
	[SerializeField] float maxPlayerDistance;
	[Header("Shooting")]
	[SerializeField] Transform gun;
	[SerializeField] GameObject projectile;
	[SerializeField] float projSpeed;
	[SerializeField] float fireRate;
	[Header("Animation")]
	[SerializeField] Transform hipsJoint;
	[SerializeField] Transform defaultHips;
	[SerializeField] GameObject deathExplode;

	private float fireTimer;
	private GameObject targetNode;
	private int nodeIndex;
	private Vector3 feetPos { get => new Vector3(transform.position.x, transform.position.y - agent.baseOffset, transform.position.z); }
	private float minDistanceThreshhold = 0.1f;

	public override void DartRespond()
	{
		Instantiate(deathExplode, transform.position, transform.rotation);
		Destroy(gameObject, 0.5f);
	}

	void Start()
	{
		animator.SetBool("ShouldWalk", nodes.Count > 0 && (nodes.Count == 1 && Vector3.Distance(feetPos, nodes[0].transform.position) > minDistanceThreshhold));
		ChangeDestination(0);
		fireTimer = fireRate;
	}

	bool wasSeekingPlayer = false;
	void Update()
	{
		bool seekPlayer = Player.Detection.currentDetectionLevel > detectionThreshhold || (wasSeekingPlayer && Player.Detection.currentDetectionLevel > 0);
		wasSeekingPlayer = seekPlayer;
		if (Vector3.Distance(Player.Transform.position, feetPos) < playerCareDistance && seekPlayer)
		{
			Physics.Raycast(detector.transform.position, Player.Transform.position - detector.transform.position, out RaycastHit hit, float.PositiveInfinity);
			if (Vector3.Distance(gun.position, Player.Transform.position) > maxPlayerDistance || !hit.collider.CompareTag("Player"))
			{
				animator.SetBool("ShouldWalk", true);
				animator.SetBool("ShouldShoot", false);
				agent.isStopped = false;
				GetComponent<AudioSource>().UnPause();
				agent.SetDestination(Player.Transform.position);
			}
			else
			{
				animator.SetBool("ShouldWalk", false);
				animator.SetBool("ShouldShoot", true);
				agent.isStopped = true;
				GetComponent<AudioSource>().Pause();
				if (fireTimer <= 0)
				{
					GameObject bullet = Instantiate(projectile, gun.transform.position, Quaternion.LookRotation(Player.Transform.position - gun.transform.position));
					bullet.GetComponentInChildren<Rigidbody>().AddForce((Player.Transform.position - gun.transform.position).normalized * projSpeed);

					fireTimer = fireRate;
				}
			}
		}
		else
		{
			animator.SetBool("ShouldWalk", (nodes.Count == 1 && Vector3.Distance(feetPos, nodes[0].transform.position) > minDistanceThreshhold) || nodes.Count > 0);
			animator.SetBool("ShouldShoot", false);
			agent.isStopped = false;
			GetComponent<AudioSource>().UnPause();
			if (Vector3.Distance(feetPos, targetNode.transform.position) < minDistanceThreshhold)
			{
				ChangeDestination((nodeIndex + 1) % nodes.Count);
			}
			else
			{
				agent.SetDestination(nodes[nodeIndex].transform.position);
			}
		}
		fireTimer -= Time.deltaTime;
	}

	public void ChangeDestination(int nodeNum)
	{
		agent.SetDestination(nodes[nodeNum].transform.position);
		targetNode = nodes[nodeNum];
		nodeIndex = nodeNum;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(feetPos, maxPlayerDistance);
		Gizmos.color = new Color(0.95f, 0.65f, 0.25f);
		Gizmos.DrawWireSphere(feetPos, playerCareDistance);
		for (int i = 0; i < nodes.Count; i++)
		{
			if (i == nodeIndex)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawSphere(nodes[i].transform.position, minDistanceThreshhold);
				Gizmos.DrawLine(feetPos, nodes[i].transform.position);
			}
			else
			{
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(nodes[i].transform.position, minDistanceThreshhold);
			}
			Gizmos.color = Color.red;
			Gizmos.DrawLine(nodes[i].transform.position, nodes[(i + 1) % nodes.Count].transform.position);
		}
	}

	public override bool CheckSightlines()
	{
		return detector.CanSeePlayer;
	}

	public override void EMPRespond(float stunDuration, GameObject stunEffect)
	{
		StartCoroutine(GetStunnedIdiot(stunDuration, stunEffect));
	}

	IEnumerator GetStunnedIdiot(float stunDuration, GameObject stunEffect)
	{
		agent.isStopped = true;
		yield return new WaitForSeconds(stunDuration);
		agent.isStopped = false;
	}
}
